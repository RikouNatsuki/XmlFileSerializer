// Copyright (c) 2025 Rikou Natsuki (夏木 梨好)
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Reflection
{
    /// <summary>
    /// 高速メソッド呼び出しユーティリティ(Expression Tree 式)
    /// </summary>
    /// <remarks><code>
    /// MethodInfo.Invoke の代替となるデリゲートを生成・キャッシュすることで、リフレクションより高速にメソッドを呼び出す。
    /// ※本クラスは、Copilotによるコード生成物である。予期せぬ不具合に対処するため、適用側のコードのプリコンパイルで適用／不適用を切り替える。
    /// </code></remarks>
    /// <revisionHistory>
    ///     <revision date = "2025/10/26" version="1.0.0.0" auther="R.Natsuki">初版作成</revision>
    /// </revisionHistory>
    internal static class FastMethodAccess
    {
        /// <summary>
        /// デバッグ用追跡フラグ(true: リフレクションで追跡, false: 式木適用)
        /// </summary>
        /// <remarks>
        /// ※式木を使うと VisualStudio の Debuger でステップインできなくなるため、リフレクションに切り替える。
        /// </remarks>
        public static bool DebugFallbackEnabled = false;

        /// <summary>
        /// 式木構築や呼び出しの解析用ログ出力（開発・デバッグ用途）
        /// </summary>
        public static bool TraceLogEnabled = false;

        /// <summary>
        /// 式木構築や呼び出しの解析用ログ出力（開発・デバッグ用途）
        /// </summary>
        private static void Debug_WriteLine(string msg) { if (TraceLogEnabled) Debug.WriteLine(msg); }

        // スレッドセーフなキャッシュ
        private static readonly object _cacheLock = new object();
        private static readonly Dictionary<MethodInfo, Func<object, object[], object>> _methodCache = new Dictionary<MethodInfo, Func<object, object[], object>>();

        /// <summary>
        /// メソッド呼び出しデリゲートの取得
        /// </summary>
        /// <param name="methodInfo">呼び出すメソッド情報</param>
        /// <returns>インスタンスと引数配列を受け取ってメソッドを呼び出すデリゲート</returns>
        public static Func<object, object[], object> GetInvoker(MethodInfo methodInfo)
        {
            if (DebugFallbackEnabled)
            {
                Debug_WriteLine(string.Format("[FastMethodAccess] DebugFallbackEnabled: Using MethodInfo.Invoke for {0}", methodInfo.Name));
                return (instance, args) => methodInfo.Invoke(instance, args);
            }

            Func<object, object[], object> invoker;
            if (_methodCache.TryGetValue(methodInfo, out invoker))
                return invoker;

            lock (_cacheLock)
            {
                if (_methodCache.TryGetValue(methodInfo, out invoker))
                    return invoker;

                Debug_WriteLine(string.Format("[FastMethodAccess] Building invoker for method: {0}", methodInfo.Name));
                Debug_WriteLine(string.Format("[FastMethodAccess] DeclaringType: {0}, IsStatic: {1}, ReturnType: {2}", methodInfo.DeclaringType, methodInfo.IsStatic, methodInfo.ReturnType));

                ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");
                ParameterExpression argsParam = Expression.Parameter(typeof(object[]), "args");

                ParameterInfo[] parameters = methodInfo.GetParameters();
                Expression[] argsExpressions = new Expression[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    Expression index = Expression.Constant(i);
                    Expression argAccess = Expression.ArrayIndex(argsParam, index);
                    Expression argCast = Expression.Convert(argAccess, parameters[i].ParameterType);
                    argsExpressions[i] = argCast;

                    Debug_WriteLine(string.Format("[FastMethodAccess] Arg[{0}] Type: {1}", i, parameters[i].ParameterType));
                }

                Expression instanceCast = methodInfo.IsStatic ? null : Expression.Convert(instanceParam, methodInfo.DeclaringType);
                MethodCallExpression call = Expression.Call(instanceCast, methodInfo, argsExpressions);

                if (methodInfo.ReturnType == typeof(void))
                {
                    // .NET 3.5 対応：Action<object, object[]> を生成し、null を返すラッパーを作成
                    var actionLambda = Expression.Lambda<Action<object, object[]>>(call, instanceParam, argsParam).Compile();
                    invoker = (instance, args) =>
                    {
                        try
                        {
                            actionLambda(instance, args);
                        }
                        catch (Exception ex)
                        {
                            Debug_WriteLine(string.Format("[FastMethodAccess] Exception during Action invoke: {0}", ex));
                            throw;
                        }
                        return null;
                    };
                }
                else
                {
                    Expression body = Expression.Convert(call, typeof(object));
                    invoker = Expression.Lambda<Func<object, object[], object>>(body, instanceParam, argsParam).Compile();
                }

                _methodCache[methodInfo] = invoker;
                return invoker;
            }
        }
    }
}
