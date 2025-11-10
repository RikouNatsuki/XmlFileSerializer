// Copyright (c) 2025 Rikou Natsuki (夏木 梨好)
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Common.XML
{
    /// <summary>
    /// XMLデータラッパー
    /// </summary>
    /// <remarks>
    /// 任意型のラッピングを行う。
    /// </remarks>
    /// <revisionHistory>
    ///     <revision date = "2025/05/05" version="1.0.0.0" auther="R.Natsuki">初版作成</revision>
    /// </revisionHistory>
    public class XmlDataWrapper<T> : IXmlSerializable where T : class, new()
    {
        /// <summary>
        /// 任意クラスデータ
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// スキーマ取得
        /// </summary>
        /// <remarks>スキーマ検証は行わないため、nullリタン</remarks>
        public XmlSchema GetSchema() { return null; }

        /// <summary>
        /// XML書込
        /// </summary>
        /// <remarks>
        /// ■■■ XmlRootName 指定について ■■■
        /// ※標準XmlSerializer の IXmlSerializable による、
        ///   書き込み時(WriteXml) は XMLルートタグが勝手に書き込まれるが、
        ///   読み込み時(ReadXml)  は XMLルートタグを明示的に読み込まなくてはならず、非対称である。
        /// ※書き込み時の指定は(<see cref="Common.XML.XmlFileSerializer.GetSerializer"/>)。
        ///   読み込み時の指定は(<see cref="Common.XML.XmlDataWrapper.ReadXml"/>)。
        /// </remarks>
        public void WriteXml(XmlWriter writer)
        {
            try
            {
                // xmlRootName WriteStartElement
                {
                    string elementName = GetElementName<T>();
                    writer.WriteElementContentAsUserType(elementName, Data);
                }
                // xmlRootName WriteEndElement
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WriteXml Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// XML読取
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            try
            {
                // xmlRootName ReadStartElement
                string xmlRootName = typeof(T).Name;
                string namespaceURI = reader.NamespaceURI;
                reader.ReadStartElementExt(xmlRootName, namespaceURI);

                string elementName = GetElementName<T>();
                T instance = new T();
                instance = reader.ReadElementContentAsUserType(elementName, instance, namespaceURI);
                Data = instance;

                // xmlRootName ReadEndElement
                reader.ReadEndElement(); // xmlRootName
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ReadXml Error: " + ex.ToString());
            }
        }

        /// <summary>
        /// 要素名取得
        /// </summary>
        private static string GetElementName<U>()
        {
            var type = typeof(U);
            object[] attrs = type.GetCustomAttributes(typeof(XmlRootAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                XmlRootAttribute rootAttr = attrs[0] as XmlRootAttribute;
                if (rootAttr != null && !string.IsNullOrEmpty(rootAttr.ElementName))
                    return rootAttr.ElementName;
            }

            return type.Name;
        }
    }
}
