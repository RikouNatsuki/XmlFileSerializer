// Copyright (c) 2025 Rikou Natsuki (夏木 梨好)
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Reflection
{
    /// <summary>
    /// 高速プロパティアクセスユーティリティ(Expression Tree 式)
    /// </summary>
    /// <remarks><code>
    /// PropertyInfo.GetValue / SetValue の代替となるデリゲートを生成・キャッシュすることで、リフレクションより高速にプロパティにアクセスする。
    /// ※本クラスは、Copilotによるコード生成物である。予期せぬ不具合に対処するため、適用側のコードのプリコンパイルで適用／不適用を切り替える。
    /// </code></remarks>
    /// <revisionHistory>
    ///     <revision date = "2025/10/13" version="1.0.0.0" auther="R.Natsuki">初版作成</revision>
    /// </revisionHistory>
    internal static class FastPropertyAccess
    {
        /// <summary>
        /// デバッグ用追跡フラグ(true: リフレクションで追跡, false: 式木適用)
        /// </summary>
        /// <remarks>
        /// ※式木を使うと VisualStudio の Debuger でステップインできなくなるため、リフレクションに切り替える。
        /// </remarks>
        public static bool DebugFallbackEnabled = false;

        // スレッドセーフなキャッシュ
        private static readonly object _getterLock = new object();
        private static readonly Dictionary<string, Func<object, object>> _getterCache = new Dictionary<string, Func<object, object>>();
        private static readonly object _setterLock = new object();
        private static readonly Dictionary<string, Action<object, object>> _setterCache = new Dictionary<string, Action<object, object>>();

        /// <summary>
        /// プロパティ値取得デリゲートの取得
        /// </summary>
        /// <remarks><code>
        /// ・リフレクション( propertyInfo.GetValue(instance) )の代替として、式ツリーを使用して高速化。
        /// ・式ツリー構成: instance => (object)((TDeclaringType)instance).PropertyName
        /// ・戻り値は object 型にボックス化されたプロパティ値。
        /// </code></remarks>
        public static Func<object, object> GetGetter(PropertyInfo propertyInfo)
        {
            if (DebugFallbackEnabled)
            {
                return (instance) => propertyInfo.GetValue(instance, null);
            }

            string key = propertyInfo.DeclaringType.FullName + "." + propertyInfo.Name;
            Func<object, object> getter;
            lock (_getterLock)
            {
                if (!_getterCache.TryGetValue(key, out getter))
                {
                    ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
                    UnaryExpression instanceCast = Expression.Convert(instance, propertyInfo.DeclaringType);

                    MemberExpression propertyAccess = Expression.Property(instanceCast, propertyInfo);

                    UnaryExpression resultCast = Expression.Convert(propertyAccess, typeof(object));
                    getter = Expression.Lambda<Func<object, object>>(resultCast, instance).Compile();
                    _getterCache[key] = getter;
                }
            }
            return getter;
        }

        /// <summary>
        /// プロパティ値設定デリゲートの取得
        /// </summary>
        /// <remarks><code>
        /// ・リフレクション( propertyInfo.SetValue(instance, value) )の代替として、式ツリーを使用して高速化。
        /// ・式ツリー構成: (instance, value) => ((TDeclaringType)instance).PropertyName = (TPropertyType)value
        /// ・value はプロパティ型にキャストされて代入される。
        /// </code></remarks>
        public static Action<object, object> GetSetter(PropertyInfo propertyInfo)
        {
            if (DebugFallbackEnabled)
            {
                return (instance, value) => propertyInfo.SetValue(instance, value, null);
            }

            string key = propertyInfo.DeclaringType.FullName + "." + propertyInfo.Name;
            Action<object, object> setter;
            lock (_setterLock)
            {
                if (!_setterCache.TryGetValue(key, out setter))
                {
                    MethodInfo setMethod = propertyInfo.GetSetMethod(true);
                    if (setMethod == null)
                        throw new InvalidOperationException("プロパティにセッターが存在しません: " + propertyInfo.Name);

                    ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
                    UnaryExpression instanceCast = Expression.Convert(instance, propertyInfo.DeclaringType);

                    ParameterExpression value = Expression.Parameter(typeof(object), "value");
                    UnaryExpression valueCast = Expression.Convert(value, propertyInfo.PropertyType);

                    MethodCallExpression call = Expression.Call(instanceCast, setMethod, valueCast);
                    setter = Expression.Lambda<Action<object, object>>(call, instance, value).Compile();
                    _setterCache[key] = setter;
                }
            }
            return setter;
        }
    }
}
