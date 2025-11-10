// Copyright (c) 2025 Rikou Natsuki (夏木 梨好)
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

//#define UNUSE_FAST_PROPERTY_ACCESS      // FastPropertyAccess 非適用
//#define UNUSE_FAST_METHOD_ACCESS        // FastMethodAccess 非適用
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Common.Reflection;

namespace Common.XML
{
    /// <summary>
    /// XmlWriter拡張メソッド(オブジェクト ⇒ XML 書き込み)
    /// </summary>
    /// <revisionHistory>
    ///     <revision date = "2025/05/05" version="1.0.0.0" auther="R.Natsuki">初版作成</revision>
    ///     <revision date = "2025/10/13" version="1.0.1.0" auther="R.Natsuki">プロパティ参照方式／式ツリー適用</revision>
    ///     <revision date = "2025/10/26" version="1.0.2.0" auther="R.Natsuki">メソッド参照方式／式ツリー適用</revision>
    /// </revisionHistory>
    internal static class XmlWriterExtensions
    {
        private const string WriteElementContentAsListMethodName = "WriteElementContentAsList";
        private const string WriteElementContentAsSingleMethodName = "WriteElementContentAsSingle";

        // スレッドセーフなキャッシュ
        private static readonly object _methodCacheLock = new object();
        private static readonly Dictionary<Type, MethodInfo> _methodCache = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// XML書込／組み込み型(列挙可能型)
        /// </summary>
        public static void WriteElementContentAsList<T>(this XmlWriter writer, string localname, IEnumerable<T> memValueList)
        {
            string arrayElementName = localname;
            string itemElementName = typeof(T).Name;

            XmlArrayItemAttribute itemAttr = typeof(T).GetCustomAttributes(typeof(XmlArrayItemAttribute), true).Length > 0
                ? (XmlArrayItemAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(XmlArrayItemAttribute))
                : null;

            if (itemAttr != null && !string.IsNullOrEmpty(itemAttr.ElementName))
                itemElementName = itemAttr.ElementName;

            writer.WriteStartElement(arrayElementName);
            foreach (T item in memValueList)
            {
                writer.WriteElementContentAsSingle(itemElementName, item);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// XML書込／組み込み型
        /// </summary>
        public static void WriteElementContentAsSingle<T>(this XmlWriter writer, string localname, T memValue)
        {
            if (memValue == null)
            {
                writer.WriteElementString(localname, string.Empty);
                return;
            }

            Type type = typeof(T);

            if (type == typeof(DateTime))
            {
                writer.WriteElementString(localname, ((DateTime)(object)memValue).ToString("o"));
                return;
            }

            if (type.IsEnum || type.IsPrimitive || type == typeof(string))
            {
                writer.WriteElementString(localname, Convert.ToString(memValue));
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            {
                try
                {
                    Type itemType = typeof(object);
                    if (type.IsGenericType)
                    {
                        Type[] args = type.GetGenericArguments();
                        if (args.Length == 1)
                            itemType = args[0];
                    }

                    MethodInfo method = typeof(XmlWriterExtensions).GetMethod(WriteElementContentAsListMethodName, BindingFlags.Static | BindingFlags.Public);
                    if (method != null)
                    {
                        MethodInfo genericMethod = method.MakeGenericMethod(itemType);
#if UNUSE_FAST_METHOD_ACCESS
                        genericMethod.Invoke(null, new object[] { writer, localname, memValue });
#else
                        (FastMethodAccess.GetInvoker(genericMethod))(null, new object[] { writer, localname, memValue });
#endif
                    }
                }
                catch (Exception ex)
                {
                    // 空データ出力
                    writer.WriteElementString(localname, string.Empty);
                    Debug.WriteLine("WriteElementContentAsSingle: " + ex.ToString());
                }
                return;
            }

            writer.WriteElementContentAsUserType(localname, memValue);
        }

        /// <summary>
        /// XML書込／ユーザー型
        /// </summary>
        public static void WriteElementContentAsUserType<T>(this XmlWriter writer, string localname, T memValue)
        {
            if (memValue == null)
            {
                writer.WriteElementString(localname, string.Empty);
                return;
            }

            Type type = typeof(T);
            XmlRootAttribute rootAttr = (XmlRootAttribute)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute));
            string elementName = (rootAttr != null && !string.IsNullOrEmpty(rootAttr.ElementName)) ? rootAttr.ElementName : localname;

            writer.WriteStartElement(elementName);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (PropertyInfo pi in properties)
            {
                if (!pi.CanRead) continue;
                if (Attribute.IsDefined(pi, typeof(XmlIgnoreAttribute))) continue;

                object value = null;
                try
                {
#if UNUSE_FAST_PROPERTY_ACCESS
                    value = pi.GetValue(memValue, null);
#else
                    value = (FastPropertyAccess.GetGetter(pi))(memValue);
#endif
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("WriteElementContentAsUserType: " + localname + "." + pi.Name + ": " + ex.ToString());
                    continue;
                }

                if (Attribute.IsDefined(pi, typeof(XmlAttributeAttribute)))
                {
                    writer.WriteAttributeString(pi.Name, Convert.ToString(value));
                    continue;
                }

                if (Attribute.IsDefined(pi, typeof(XmlTextAttribute)))
                {
                    writer.WriteString(Convert.ToString(value));
                    continue;
                }

                string xmlElementName = pi.Name;
                XmlElementAttribute attr = (XmlElementAttribute)Attribute.GetCustomAttribute(pi, typeof(XmlElementAttribute));
                if (attr != null && !string.IsNullOrEmpty(attr.ElementName))
                    xmlElementName = attr.ElementName;

                try
                {
                    MethodInfo method;
                    lock (_methodCacheLock)
                    {
                        if (!_methodCache.TryGetValue(pi.PropertyType, out method))
                        {
                            method = typeof(XmlWriterExtensions).GetMethod(WriteElementContentAsSingleMethodName, BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(pi.PropertyType);
                            _methodCache[pi.PropertyType] = method;
                        }
                    }
#if UNUSE_FAST_METHOD_ACCESS
                    method.Invoke(null, new object[] { writer, xmlElementName, value });
#else
                    (FastMethodAccess.GetInvoker(method))(null, new object[] { writer, xmlElementName, value });
#endif
                }
                catch (Exception ex)
                {
                    // 空データ出力
                    writer.WriteElementString(xmlElementName, string.Empty);
                    Debug.WriteLine("WriteElementContentAsUserType: " + localname + "." + pi.Name + ": " + ex.ToString());
                }
            }

            writer.WriteEndElement();
        }
    }
}
