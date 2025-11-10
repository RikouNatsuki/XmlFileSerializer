# XmlFileSerializer

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 概要

C# 標準 XmlSerializer の制約（カプセル化不可、public限定、ルート要素名自動付与など）を、カスタム IXmlSerializable 実装と高速リフレクション拡張で解消するライブラリです。

## 特徴・主な機能

- **private/protected/internal プロパティ・フィールドもシリアライズ／デシリアライズ可能**
- **ルート要素名や属性名を柔軟に制御可能**
- **.NET 標準の XML 属性（XmlElement, XmlAttribute, XmlText, XmlIgnore, XmlRoot など）に対応**
- **式木（Expression Tree）による高速な[プロパティ](README_FastPropertyAccess.md)／[メソッドアクセス](README_FastMethodAccess.md)**
- **スレッドセーフ設計（シリアライザのキャッシュやファイルI/Oの排他制御）**
- **ファイルI/Oの安全設計（タイムスタンプ検証・一時ファイル運用）**
- **標準 XmlSerializer との互換性を維持しつつ、拡張性・保守性を重視**

> 本ライブラリは、シリアライザインスタンスのキャッシュやファイル操作においてスレッドセーフな設計を採用しています。  
> 複数スレッドから同時にシリアライズ／デシリアライズ処理を行う場合も安全に動作します。

## XML属性対応状況比較表

| 属性名（C#）                        | 説明                                               | 標準XmlSerializer | カスタムXmlSerializer |
|-------------------------------------|----------------------------------------------------|:-----------------:|:---------------------:|
| `XmlElementAttribute`               | 要素名・型・順序などを指定                         |       ✅          |         ✅            |
| `XmlAttributeAttribute`             | 属性として出力                                     |       ✅          |         ✅            |
| `XmlTextAttribute`                  | 要素のテキストノードとして出力                     |       ✅          |         ✅            |
| `XmlIgnoreAttribute`                | シリアライズ/デシリアライズ対象外にする           |       ✅          |         ✅            |
| `XmlRootAttribute`                  | ルート要素名・名前空間などを指定                   |       ✅          |         ✅            |
| `XmlArrayItemAttribute`             | 配列/コレクションの子要素名・型を指定              |       ✅          |         ✅            |
| `XmlArrayAttribute`                 | 配列/コレクションの親要素名・順序などを指定        |       ✅          |         ❌            |
| `XmlTypeAttribute`                  | 型のXML表現名・名前空間を指定                      |       ✅          |         ❌            |
| `XmlNamespaceDeclarationsAttribute` | 名前空間宣言をプロパティとして出力                 |       ✅          |         ❌            |
| `XmlAnyElementAttribute`            | 未知の要素を柔軟に受け入れる                       |       ✅          |         ❌            |
| `XmlAnyAttributeAttribute`          | 未知の属性を柔軟に受け入れる                       |       ✅          |         ❌            |
| `XmlChoiceIdentifierAttribute`      | 複数型/要素の選択肢を識別                         |       ✅          |         ❌            |
| `XmlIncludeAttribute`               | 継承型の明示的なシリアライズ対象指定              |       ✅          |         ❌            |
| `XmlDefaultValueAttribute`          | 既定値の省略制御                                   |       ✅          |         ❌            |
| `XmlEnumAttribute`                  | 列挙型の値名制御                                   |       ✅          |         ❌            |
| `XmlSerializationEvents`（各種）    | シリアライズ/デシリアライズ時のイベントフック      |       ✅          |         ❌            |

> ✅：対応済み　❌：未対応

## 目的

標準 XmlSerializer の制約を回避し、より柔軟かつ安全な XML シリアライズ／デシリアライズを実現します。

## 背景

- 保守性・技術的平易さ・実行速度を度外視し、汎用性・再利用性の追求を目的として開発しました。
- 個人の技術検証および OSS 活動の一環として公開したプロジェクトです。
- 業務で使用されたコードや設計は一切参照せず、独自に設計・実装しています。
- 技術的な最適化の結果、類似する構造が他に存在する可能性はありますが、他プロジェクトへの依拠性はありません。

## インストール方法

- NuGet未公開の場合は、`Common.XML` および `Common.Reflection` 以下の .cs ファイルをプロジェクトに直接追加してください。
- .NET Framework 3.5 以上で動作します。
- 追加依存パッケージはありません。

## 使い方

### 標準 XmlSerializer との比較例

```csharp
// 標準XmlSerializer
    // XML 書き込み
    var TestMgrNow = new TestManager();
    var serializer = new XmlSerializer(typeof(TestManager));
    using (var writer = new StreamWriter("standard.xml"))
    {
        serializer.Serialize(writer, TestMgrNow);
    }

    // XML 読み込み
    using (var reader = new StreamReader("standard.xml"))
    {
        TestManager TestMgrNew = (TestManager)serializer.Deserialize(reader);
    }

// カスタムXmlSerializer
    // XML 書き込み
    var errMsg = string.Empty;
    if (!Common.XML.XmlFileSerializer<TestManager>.TrySaveXML(data, "custom.xml", out errMsg))
        Console.WriteLine("SaveXML: " + errMsg);

    // XML 読み込み
    TestManager loaded;
    if (!Common.XML.XmlFileSerializer<TestManager>.TryLoadXML(out loaded, "custom.xml", out errMsg))
        Console.WriteLine("LoadXML: " + errMsg);

```
- 詳細なサンプルは Form1.cs を参照してください。

## 詳細ドキュメント

- [FastPropertyAccess](README_FastPropertyAccess.md)
- [FastMethodAccess](README_FastMethodAccess.md)

## 必要条件

- .NET Framework 3.5 以上
- 追加依存パッケージなし

## 既知の制約・注意事項

- 一部のXML属性（XmlType, XmlArray など）は未対応です（今後拡張予定）。
- 大規模データや多階層オブジェクトでのベンチマークは推奨します。

## ライセンス

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
- 本プロジェクトは MIT LICENSE の下で公開されています。

## 免責事項

- 本ソフトウェアは現状のまま提供され、利用に伴う責任は利用者に帰属します。
- 開発者の所属や契約形態は、本ソフトウェアの利用条件や責任に影響しません。

## 貢献方法

- 本プロジェクトは、主に個人の技術検証・公開を目的としています。恐れ入りますが、現時点では外部からのIssueやPull Requestによる貢献は受け付けておりません。ご理解のほどよろしくお願いいたします。
- 尚、対応済機能として挙げている項目についてのバグ報告は、歓迎しております。

## 作者情報

- Rikou Natsuki (夏木 梨好)
- GitHub: [Natsuki-Rikou](https://github.com/Natsuki-Rikou)
- お問い合わせは [Issues](https://github.com/Natsuki-Rikou/XmlFileSerializer/issues) まで

