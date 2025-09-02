---
title: ベストプラクティス
sidebar_position: 6
---

# ベストプラクティス

## パフォーマンス考慮事項

1. **音声設定の更新頻度**: 音声設定の変更は必要最小限に留める
2. **チャンネル管理**: 不要なチャンネル登録は避ける
3. **イベント処理**: 重い処理はコルーチンや遅延実行を使用

## デバッグ方法

1. **ログ出力**: `Debug.Log` を使用して状態変化を追跡
2. **インスペクター監視**: 実行時にコンポーネントの値を監視
3. **VRChatログ**: VRChatのコンソールログを確認

```csharp
// ログ出力例
Debug.Log($"[URC] Channel changed: {oldChannel} -> {newChannel}");
Debug.Log($"[URC] Player {playerId} voice gain set to {gain}");
```

## エラーハンドリング

```csharp
// Null チェックの例
if (Utilities.IsValid(channelManager))
{
    channelManager.RegisterChannel(channel, this);
}
else
{
    Debug.LogError("[URC] ChannelManager is not valid");
}
```

## 貢献ガイドライン

### コード品質

1. **命名規則**: C# の標準的な命名規則に従う
2. **コメント**: 公開APIには適切なコメントを記述
3. **エラーハンドリング**: 適切なnullチェックとエラー処理
4. **パフォーマンス**: VRChat環境でのパフォーマンスを考慮

### テスト

1. **単体テスト**: 可能な限り単体テストを記述
2. **統合テスト**: VRChat環境での動作確認
3. **回帰テスト**: 既存機能の動作確認

### プルリクエスト

1. **明確な説明**: 変更内容と理由を明記
2. **小さな変更**: 1つのPRで1つの機能/修正
3. **テスト**: 十分なテストを実施
4. **ドキュメント**: 必要に応じてドキュメントを更新