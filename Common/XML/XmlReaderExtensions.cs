// Copyright (c) 2025 Rikou Natsuki (夏木 梨好)
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

//#define UNUSE_FAST_PROPERTY_ACCESS      // FastPropertyAccess 非適用
//#define UNUSE_FAST_METHOD_ACCESS        // FastMethodAccess 非適用
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Common.Reflection;

namespace Common.XML
{
    /// <summary>
    /// XmlReader拡張メソッド(XML ⇒ オブジェクト 読み取り)
    /// </summary>
    /// <revisionHistory>
    ///     <revision date = "2025/05/05" version="1.0.0.0" auther="R.Natsuki">初版作成</revision>
    ///     <revision date = "2025/10/13" version="1.0.1.0" auther="R.Natsuki">プロパティ参照方式／式ツリー適用</revision>
    ///     <revision date = "2025/10/26" version="1.0.2.0" auther="R.Natsuki">メソッド参照方式／式ツリー適用</revision>
    /// </revisionHistory>
    internal static class XmlReaderExtensions
    {
        private const string ReadElementContentAsListMethodName = "ReadElementContentAsList";
        private const string ReadElementContentAsSingleMethodName = "ReadElementContentAsSingle";

        /// <summary>
        /// 整形済XMLの先頭空白をスキップ
        /// </summary>
        public static void MoveToNextElement(this XmlReader reader)
        {
            while (reader.NodeType != XmlNodeType.Element && !reader.EOF)
                reader.Read();
        }

        /// <summary>
        /// XML開始要素読取
        /// </summary>
        public static bool ReadStartElementExt(this XmlReader reader, string name, string namespaceURI)
        {
            reader.MoveToNextElement();
            if (reader.LocalName != name || reader.NamespaceURI != namespaceURI)
                throw new XmlException(string.Format("要素名不一致。要素<{0}>を、要素<{1}>として読み取ろうとしています。", reader.LocalName, name));

            bool isRead = !reader.IsEmptyElement;
            if (isRead) reader.ReadStartElement(name, namespaceURI);
            else reader.Read();
            return isRead;
        }

        /// <summary>
        /// XML読取／組み込み型(列挙可能型)
        /// </summary>
        public static IEnumerable<T> ReadElementContentAsList<T>(this XmlReader reader, string localname, IEnumerable<T> defValueList, string namespaceURI) where T : new()
        {
            List<T> itemList = new List<T>();
            if (reader.ReadStartElementExt(localname, namespaceURI))
            {
                string itemName = typeof(T).Name;
                XmlArrayItemAttribute attr = (XmlArrayItemAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(XmlArrayItemAttribute));
                if (attr != null && !string.IsNullOrEmpty(attr.ElementName))
                    itemName = attr.ElementName;

                while (reader.IsStartElement(itemName, namespaceURI))
                {
                    T defItem = new T();
                    T readItem = reader.ReadElementContentAsSingle(itemName, defItem, namespaceURI);
                    itemList.Add(readItem);
                }
                reader.ReadEndElement();
            }
            return itemList;
        }

        /// <summary>
        /// XML読取／組み込み型
        /// </summary>
        public static T ReadElementContentAsSingle<T>(this XmlReader reader, string localname, T defValue, string namespaceURI)
        {
            reader.MoveToNextElement();
            if (reader.LocalName != localname || reader.NamespaceURI != namespaceURI)
                throw new XmlException(string.Format("要素名不一致。要素<{0}>を、要素<{1}>として読み取ろうとしています。", reader.LocalName, localname));

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return defValue;
            }

            try
            {
                Type type = typeof(T);
                if (type.IsPrimitive || type == typeof(string) || type.IsEnum || type == typeof(DateTime))
                {
                    string content = reader.ReadElementContentAsString(localname, namespaceURI);
                    if (type.IsEnum)
                        return (T)Enum.Parse(type, content, true);

                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if (converter != null && converter.CanConvertFrom(typeof(string)))
                        return (T)converter.ConvertFromInvariantString(content);
                }

                return (T)reader.ReadElementContentAsUserType(localname, defValue, namespaceURI);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(localname + ": " + ex);
                return defValue;
            }
        }

        /// <summary>
        /// XML読取／ユーザー型
        /// </summary>
        public static T ReadElementContentAsUserType<T>(this XmlReader reader, string localname, T defValue, string namespaceURI)
        {
            reader.ReadStartElementExt(localname, namespaceURI);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (PropertyInfo pi in properties)
            {
                if (!pi.CanWrite || Attribute.IsDefined(pi, typeof(XmlIgnoreAttribute)))
                    continue;

                string xmlElementName = pi.Name;
                XmlElementAttribute elementAttr = (XmlElementAttribute)Attribute.GetCustomAttribute(pi, typeof(XmlElementAttribute));
                if (elementAttr != null && !string.IsNullOrEmpty(elementAttr.ElementName))
                    xmlElementName = elementAttr.ElementName;

                try
                {
                    object defConv = pi.GetValue(defValue, null);
                    if (defConv == null)
                    {
                        try
                        {
                            defConv = Activator.CreateInstance(pi.PropertyType, true);
                        }
                        catch
                        {
                            defConv = null;
                        }
                    }

                    object propertyValue = null;

                    if (Attribute.IsDefined(pi, typeof(XmlAttributeAttribute)))
                    {
                        string attrName = pi.Name;
                        XmlAttributeAttribute attrAttr = (XmlAttributeAttribute)Attribute.GetCustomAttribute(pi, typeof(XmlAttributeAttribute));
                        if (attrAttr != null && !string.IsNullOrEmpty(attrAttr.AttributeName))
                            attrName = attrAttr.AttributeName;

                        string attrValue = reader.GetAttribute(attrName);
                        if (attrValue != null)
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(pi.PropertyType);
                            if (converter != null && converter.CanConvertFrom(typeof(string)))
                                propertyValue = converter.ConvertFromInvariantString(attrValue);
#if UNUSE_FAST_PROPERTY_ACCESS
                            pi.SetValue(defValue, propertyValue, null);
#else
                            (FastPropertyAccess.GetSetter(pi))(defValue, propertyValue);
#endif
                        }
                        continue;
                    }

                    if (Attribute.IsDefined(pi, typeof(XmlTextAttribute)))
                    {
                        string textValue = reader.ReadString();
                        TypeConverter converter = TypeDescriptor.GetConverter(pi.PropertyType);
                        if (converter != null && converter.CanConvertFrom(typeof(string)))
                            propertyValue = converter.ConvertFromInvariantString(textValue);
#if UNUSE_FAST_PROPERTY_ACCESS
                        pi.SetValue(defValue, propertyValue, null);
#else
                        (FastPropertyAccess.GetSetter(pi))(defValue, propertyValue);
#endif
                        continue;
                    }

                    if (typeof(IEnumerable).IsAssignableFrom(pi.PropertyType) && pi.PropertyType != typeof(string))
                    {
                        Type itemType = typeof(object);
                        if (pi.PropertyType.IsGenericType)
                        {
                            itemType = pi.PropertyType.GetGenericArguments()[0];
                        }

                        MethodInfo method = typeof(XmlReaderExtensions).GetMethod(ReadElementContentAsListMethodName, BindingFlags.Static | BindingFlags.Public);
                        if (method != null)
                        {
                            MethodInfo genericMethod = method.MakeGenericMethod(itemType);
                            object defList = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
#if UNUSE_FAST_METHOD_ACCESS
                            propertyValue = genericMethod.Invoke(null, new object[] { reader, xmlElementName, defList, namespaceURI });
#else
                            propertyValue = (FastMethodAccess.GetInvoker(genericMethod))(null, new object[] { reader, xmlElementName, defList, namespaceURI });
#endif
                        }
                    }
                    else
                    {
                        MethodInfo method = typeof(XmlReaderExtensions).GetMethod(ReadElementContentAsSingleMethodName, BindingFlags.Static | BindingFlags.Public);
                        if (method != null)
                        {
                            MethodInfo genericMethod = method.MakeGenericMethod(pi.PropertyType);
#if UNUSE_FAST_METHOD_ACCESS
                            propertyValue = genericMethod.Invoke(null, new object[] { reader, xmlElementName, defConv, namespaceURI });
#else
                            propertyValue = (FastMethodAccess.GetInvoker(genericMethod))(null, new object[] { reader, xmlElementName, defConv, namespaceURI });
#endif
                        }
                    }

#if UNUSE_FAST_PROPERTY_ACCESS
                    pi.SetValue(defValue, propertyValue, null);
#else
                    (FastPropertyAccess.GetSetter(pi))(defValue, propertyValue);
#endif
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(xmlElementName + ": " + ex);
                }
            }

            reader.ReadEndElement();
            return defValue;
        }
    }
}
