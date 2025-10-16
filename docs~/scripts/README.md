# Documentation Scripts

## generate-api-docs.js

C#のXMLコメント（`///`）からAPIドキュメントを自動生成するスクリプト。

### 機能

- 両方のパッケージ（main + SaccFlight addon）のC#ソースファイル（`.cs`）を再帰的にスキャン
- XMLコメントを`xmldoc`パッケージでパース
- **publicクラス、メソッド、プロパティ、フィールドを自動的にドキュメント化**
  - XMLコメントがある場合：その説明を表示
  - XMLコメントがない場合：「（説明なし）」と表示
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
- **形式:** `{Namespace}/{ClassName}.md`（ネームスペースごとにフォルダ分け）
- **インデックス:** `docs/api/index.md`
- **対象:** publicクラスのpublicメンバーのみ

**ディレクトリ構造例:**
```
docs/api/
├── index.md
├── UdonRadioCommunicationRedux/
│   ├── DefaultSetting.md
│   ├── VoiceBroadcastByChannel.md
│   └── ...
└── UdonRadioCommunicationRedux.SaccFlight/
    ├── SFEXT_URC_VHF.md
    └── ...
```

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

    // XMLコメントがなくてもpublicであればドキュメント化される
    public void AnotherMethod()
    {
        // 「（説明なし）」と表示される
    }
}
```

### サポートされるXMLタグ

- `<summary>` - 概要
- `<param name="...">` - パラメータの説明
- `<returns>` - 戻り値の説明
- `<remarks>` - 備考
- `<example>` - 使用例

### 注意事項

- **XMLコメントがなくてもOK**: publicメンバーは自動的にリスト化されます
- **privateメンバーは除外**: private/protected/internalメンバーはドキュメント化されません
- **複数パッケージ対応**: メインパッケージとSaccFlightアドオンの両方を処理します

## copy-changelog.js

ルートディレクトリの`CHANGELOG.md`をドキュメントディレクトリにコピーするスクリプト。
