// Copyright (c) 2025 Rikou Natsuki (夏木 梨好)
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Common.XML
{
    /// <summary>
    /// XMLファイル シリアライザー
    /// </summary>
    /// <remarks><code>
    /// ・汎用XMLファイルRWクラス。
    /// 
    /// ■■■ .NET 標準 XmlSerializer との機能差分 ■■■
    /// ・XMLに変換するクラスは、引数なしコンストラクタを備えること。    ※標準準拠。
    /// ・XMLに変換するプロパティは、任意のアクセス修飾子を指定できる。  ※標準は public 限定。
    /// ・XML属性指定は、多重定義不可。                                  ※標準準拠。
    /// </code></remarks>
    /// <revisionHistory>
    ///     <revision date = "2025/05/05" version="1.0.0.0" auther="R.Natsuki">初版作成</revision>
    /// </revisionHistory>
    internal class XmlFileSerializer<T> where T : class, new()
    {
        private static readonly Dictionary<string, XmlSerializer> _serializerCache = new Dictionary<string, XmlSerializer>();
        private static readonly object _serializerLock = new object();

        private static XmlSerializer GetSerializer(Type type, string xmlRootName)
        {
            string key = type.FullName;
            lock (_serializerLock)
            {
                XmlSerializer serializer;
                if (!_serializerCache.TryGetValue(key, out serializer))
                {
                    var xmlRoot = new XmlRootAttribute(xmlRootName);
                    serializer = new XmlSerializer(type, xmlRoot);
                    _serializerCache[key] = serializer;
                }
                return serializer;
            }
        }

        /// <summary>
        /// XML書込
        /// </summary>
        /// <remarks><code>
        /// ■利用例
        ///     var MemData = new UserClass();
        ///     var errMsg = string.Empty;
        ///     if (!Common.XML.XmlFileSerializer&lt;UserClass&gt;.TrySaveXML(MemData, "fileName.xml", out errMsg)) Console.WriteLine("SaveXML: " + errMsg);
        /// </code></remarks>
        public static bool TrySaveXML(T data, string xmlFilePath, out string error)
        {
            string tmpPath = xmlFilePath + ".w.tmp";
            error = string.Empty;

            try
            {
                Encoding encoding = new UTF8Encoding(true); // BOM付きUTF-8
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineHandling = NewLineHandling.Entitize,
                    Encoding = encoding
                };

                using (XmlWriter writer = XmlWriter.Create(tmpPath, settings))
                {
                    var xmlData = new XmlDataWrapper<T> { Data = data };
                    GetSerializer(typeof(XmlDataWrapper<T>), typeof(T).Name).Serialize(writer, xmlData);
                }

                try
                {
                    if (File.Exists(xmlFilePath)) File.Delete(xmlFilePath);
                }
                catch (Exception delEx)
                {
                    Debug.WriteLine("既存ファイル削除失敗: " + delEx.ToString());
                }

                File.Move(tmpPath, xmlFilePath);

                return true;
            }
            catch (Exception ex)
            {
                error = "XmlFileSerializer.TrySaveXML: " + ex.ToString();
                Debug.WriteLine(error);

                if (File.Exists(tmpPath))
                {
                    try { File.Delete(tmpPath); }
                    catch (Exception delEx) { Debug.WriteLine("一時ファイル削除失敗: " + delEx.ToString()); }
                }

                return false;
            }
        }

        /// <summary>
        /// XML読取
        /// </summary>
        /// <remarks><code>
        /// ■利用例
        ///     var errMsg = string.Empty;
        ///     UserClass XmlData;
        ///     if (!Common.XML.XmlFileSerializer&lt;UserClass&gt;.TryLoadXML(out XmlData, "fileName.xml", out errMsg)) Console.WriteLine("LoadXML: " + errMsg);
        /// </code></remarks>
        public static bool TryLoadXML(out T data, string xmlFilePath, out string error)
        {
            data = null;
            error = string.Empty;

            int retryCount = 0;
            const int maxRetries = 3;

            while (retryCount < maxRetries)
            {
                try
                {
                    if (!File.Exists(xmlFilePath))
                        throw new FileNotFoundException("XMLファイルが見つかりません。: " + xmlFilePath);

                    FileInfo fileInfoBefore = new FileInfo(xmlFilePath);
                    DateTime timestampBefore = fileInfoBefore.LastWriteTimeUtc;

                    string tmpPath = xmlFilePath + ".r.tmp";
                    File.Copy(xmlFilePath, tmpPath, true);

                    using (FileStream fs = new FileStream(tmpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (XmlReader reader = XmlReader.Create(fs))
                    {
                        var loaded = (XmlDataWrapper<T>)GetSerializer(typeof(XmlDataWrapper<T>), typeof(T).Name).Deserialize(reader);
                        data = loaded.Data;
                    }

                    FileInfo fileInfoAfter = new FileInfo(xmlFilePath);
                    DateTime timestampAfter = fileInfoAfter.LastWriteTimeUtc;

                    try { File.Delete(tmpPath); }
                    catch (Exception delEx) { Debug.WriteLine("一時ファイル削除失敗: " + delEx.ToString()); }

                    if (timestampBefore == timestampAfter)
                    {
                        return true;
                    }

                    System.Threading.Thread.Sleep(50);
                    retryCount++;
                }
                catch (Exception ex)
                {
                    error = "XmlFileSerializer.TryLoadXML: " + ex.ToString();
                    Debug.WriteLine(error);
                    return false;
                }
            }

            error = "XMLファイルの読取に失敗しました（タイムスタンプが安定しません）。";
            return false;
        }
    }
}
