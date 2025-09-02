---
title: 統合例とトラブルシューティング
sidebar_position: 7
---

# 統合例とトラブルシューティング

## 統合例

### 基本的な無線システムの実装

```csharp
// 1. UdonRadioCommunication を配置
// 2. VoiceBroadcastByChannel を配置し、UdonRadioCommunication に登録
// 3. Transceiver を配置し、VoiceBroadcastByChannel を参照
// 4. 必要に応じてゾーンシステムを追加
```

### SaccFlightとの統合

```csharp
// 1. SaccEntity にSFEXT_URC_VHF を追加
// 2. DFUNC_URC_VHF_Rx/Tx を追加
// 3. SaccEntityのDial FunctionsにDFUNCを登録
```

## トラブルシューティング

### よくある問題

1. **音声が聞こえない**
   - チャンネルマネージャーの登録確認
   - 受信機の電源状態確認
   - プレイヤー音声の登録確認

2. **送信できない**
   - 送信機の電源状態確認
   - チャンネル登録の確認
   - 権限設定の確認

3. **SaccFlightで動作しない**
   - SFEXT_URC_VHF の正しい配置確認
   - DFUNCの登録確認
   - 依存関係の確認