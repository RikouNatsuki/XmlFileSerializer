# FastMethodAccess  Utility

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 概要
- `FastMethodAccess` は、.NET のリフレクションによるメソッド呼び出しを高速化するユーティリティです。  
- Expression Tree 式を用いてデリゲートを生成・キャッシュし、`MethodInfo.Invoke` よりも高いパフォーマンスでメソッドを呼び出せます。

## 主な機能
- **高速メソッド呼び出し**：リフレクションのオーバーヘッドを回避
- **キャッシュ機能**：生成したデリゲートをスレッドセーフにキャッシュ
- **デバッグ・トレース**：デバッグ時はリフレクション呼び出しに切り替え可能、ログ出力対応
- **void戻り値対応**：.NET 3.5互換のため、voidメソッドも安全に呼び出し可能

## 使い方

```csharp
using System.Reflection;
using Common.Reflection;

// MethodInfo を取得
MethodInfo methodInfo = typeof(MyClass).GetMethod("MyMethod");

// デリゲート取得
var invoker = FastMethodAccess.GetInvoker(methodInfo);

// メソッド呼び出し
object result = invoker(instance, new object[] { arg1, arg2 });
```

## パラメータ

| パラメーター名             | 型      | 説明                                                                                   |
|---------------------------|---------|----------------------------------------------------------------------------------------|
| DebugFallbackEnabled      | bool    | `true` の場合、リフレクション（MethodInfo.Invoke）による呼び出しに切り替えます。<br>デバッグ時や Visual Studio のステップインを有効にしたい場合に利用します。 |
| TraceLogEnabled           | bool    | `true` の場合、式木構築や呼び出し時の解析用ログを出力します（Debug.WriteLine）。<br>開発・デバッグ用途向けです。 |

## 注意事項

- 本クラスは Copilot によるコード生成物です。予期せぬ不具合に備え、適用側コードでプリコンパイルによる切り替えを推奨します。
- キャッシュクリア機能は未実装です。長期運用時はメモリ使用量に注意してください。
- 引数型変換に失敗した場合、例外が発生します。呼び出し元で適切な型を渡してください。

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

