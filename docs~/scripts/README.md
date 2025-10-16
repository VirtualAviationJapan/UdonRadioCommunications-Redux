# Documentation Scripts

## generate-api-docs.js

C#のXMLコメント（`///`）からAPIドキュメントを自動生成するスクリプト。

### 機能

- C#ソースファイル（`.cs`）を再帰的にスキャン
- XMLコメントを`xmldoc`パッケージでパース
- クラス、メソッド、プロパティ、フィールドのドキュメントをMarkdownに変換
- 名前空間ごとにグループ化されたインデックスページを生成

### 使用方法

```bash
# 単独実行
pnpm generate-api

# 自動実行（startまたはbuild時）
pnpm start
pnpm build
```

### 出力

- **出力先:** `docs/api/`
- **形式:** `{Namespace}.{ClassName}.md`
- **インデックス:** `docs/api/index.md`

### XMLコメント例

```csharp
/// <summary>
/// クラスの説明
/// </summary>
public class MyClass : UdonSharpBehaviour
{
    /// <summary>
    /// メソッドの説明
    /// </summary>
    /// <param name="value">パラメータの説明</param>
    /// <returns>戻り値の説明</returns>
    public int MyMethod(string value)
    {
        return 0;
    }
}
```

### サポートされるXMLタグ

- `<summary>` - 概要
- `<param name="...">` - パラメータの説明
- `<returns>` - 戻り値の説明
- `<remarks>` - 備考
- `<example>` - 使用例

## copy-changelog.js

ルートディレクトリの`CHANGELOG.md`をドキュメントディレクトリにコピーするスクリプト。
