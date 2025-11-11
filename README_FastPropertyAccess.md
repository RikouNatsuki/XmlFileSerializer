# FastPropertyAccess Utility

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 概要
- `FastPropertyAccess` は、.NET のリフレクションによるプロパティアクセス（Get/Set）を高速化するユーティリティです。  
- Expression Tree 式を用いてデリゲートを生成・キャッシュし、`PropertyInfo.GetValue` / `SetValue` よりも高いパフォーマンスでプロパティにアクセスできます。

## 主な機能
- **高速プロパティアクセス**：リフレクションのオーバーヘッドを回避
- **キャッシュ機能**：生成したデリゲートをスレッドセーフにキャッシュ
- **デバッグ・トレース**：デバッグ時はリフレクション呼び出しに切り替え可能
- **セッター未実装時の例外処理**：安全性向上

## 使い方

```csharp
using System.Reflection;
using Common.Reflection;

// PropertyInfo を取得
PropertyInfo propertyInfo = typeof(MyClass).GetProperty("MyProperty");

// Getter デリゲート取得
var getter = FastPropertyAccess.GetGetter(propertyInfo);
object value = getter(instance);

// Setter デリゲート取得
var setter = FastPropertyAccess.GetSetter(propertyInfo);
setter(instance, newValue);
```

## パラメータ

| パラメーター名        | 型    | 説明                                                                                   |
|----------------------|-------|----------------------------------------------------------------------------------------|
| DebugFallbackEnabled | bool  | `true` の場合、リフレクション（PropertyInfo.GetValue / SetValue）による呼び出しに切り替えます。<br>デバッグ時や Visual Studio のステップインを有効にしたい場合に利用します。 |

## 注意事項

- 本クラスは Copilot によるコード生成物です。予期せぬ不具合に備え、適用側コードでプリコンパイルによる切り替えを推奨します。
- キャッシュクリア機能は未実装です。長期運用時はメモリ使用量に注意してください。
- 型変換に失敗した場合、例外が発生します。呼び出し元で適切な型を渡してください。
- セッター未実装プロパティに対しては例外が発生します。

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
- GitHub: [RikouNatsuki](https://github.com/RikouNatsuki)
- お問い合わせは [Issues](https://github.com/RikouNatsuki/XmlFileSerializer/issues) まで

