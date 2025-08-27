---
title: Developer Manual
sidebar_position: 3
---

# 開発者向けマニュアル

このマニュアルでは、UdonRadioCommunications-Redux の内部構造と拡張方法について説明します。

## アーキテクチャ概要

UdonRadioCommunications-Redux は、VRChat の UdonSharp を使用したモジュラー設計の音声通信システムです。システムは以下の主要コンポーネントで構成されています：

### コアシステム

- **UdonRadioCommunication**: システム全体の中央管理
- **VoiceProtocol**: 音声処理プロトコルの基底クラス
- **PlayerVoice**: 個別プレイヤーの音声設定管理

### チャンネルシステム

- **Transceiver**: 送受信両対応の無線機
- **Receiver**: 受信専用の無線機
- **VoiceBroadcastByChannel**: チャンネル別音声配信管理
- **LocalChannelObject**: ローカルチャンネルオブジェクト

### ゾーンシステム

- **VoiceZone**: 音声ゾーンの定義
- **VoiceSettingByZone**: ゾーン別音声設定

### SaccFlight統合

- **SFEXT_URC_VHF**: SaccFlight拡張メインクラス
- **DFUNC_URC_VHF_Rx/Tx**: 航空機用無線制御UI

## 主要クラス詳細

### UdonRadioCommunication

システムの中心となるクラスです。すべてのプレイヤー音声を管理し、音声プロトコルとの連携を行います。

```csharp
public class UdonRadioCommunication : UdonSharpBehaviour
{
    public VoiceProtocol[] voiceProtocols;
    
    // プレイヤー音声の登録
    public void RegisterPlayerVoice(int playerId, PlayerVoice pv)
    
    // 音声設定の変更
    public void SetVoiceGain(int playerId, int priority, float gain)
    public void SetVoiceNear(int playerId, int priority, float near)
    public void SetVoiceFar(int playerId, int priority, float far)
}
```

### Transceiver

送受信機能を持つ基底クラスです。チャンネル管理と送受信状態の制御を行います。

```csharp
public class Transceiver : UdonSharpBehaviour
{
    public VoiceBroadcastByChannel channelManager;
    
    // プロパティ
    public virtual int Channel { get; set; }
    public bool RxPower { get; set; }
    public bool TxPower { get; set; }
    public float Gain { get; set; }
    
    // 継承クラスで実装するメソッド
    public virtual void OnUpdateChannel() { }
    public virtual void ChannelTransmitting() { }
}
```

### VoiceBroadcastByChannel

チャンネル別の音声配信を管理するクラスです。

```csharp
public class VoiceBroadcastByChannel : VoiceProtocol
{
    // チャンネル登録/解除
    public void RegisterChannel(int channelId, UdonSharpBehaviour transceiver)
    public void UnregisterChannel(int channelId, UdonSharpBehaviour transceiver)
    
    // 送信開始/停止
    public void StartTransmit(int channelId, UdonSharpBehaviour transceiver)
    public void StopTransmit(int channelId, UdonSharpBehaviour transceiver)
}
```

## 開発環境セットアップ

### 必要な環境

- Unity 2022.3 LTS
- VRChat SDK3 - Worlds
- UdonSharp
- SaccFlightAndVehicles (SaccFlight統合を使用する場合)

### パッケージ構成

```
Packages/
├── jp.virtualaviation.udon-radio-communications-redux/    # メインパッケージ
│   ├── Runtime/
│   │   ├── core/           # コアシステム
│   │   ├── channel/        # チャンネルシステム
│   │   ├── zone/           # ゾーンシステム
│   │   └── utilities/      # ユーティリティ
│   ├── Editor/             # エディタ拡張
│   ├── Sample/             # サンプルシーン
│   └── Tests/              # テスト
└── jp.virtualaviation.udon-radio-communications-redux-sf/ # SaccFlight統合
    └── Runtime/
        ├── DFUNC/          # SaccFlight DFUNC コンポーネント
        └── SFEXT_URC_VHF.cs # SaccFlight拡張
```

## 拡張方法

### カスタムTransceiverの作成

独自の送受信機を作成するには、`Transceiver` クラスを継承します：

```csharp
using UdonRadioCommunicationRedux;

public class CustomTransceiver : Transceiver
{
    // チャンネル変更時に呼ばれる
    public override void OnUpdateChannel()
    {
        // カスタム処理
        Debug.Log($"Channel changed to: {Channel}");
    }
    
    // 送信中に呼ばれる
    public override void ChannelTransmitting()
    {
        // 送信中の処理
    }
}
```

### カスタムVoiceProtocolの作成

独自の音声プロトコルを作成するには、`VoiceProtocol` クラスを継承します：

```csharp
using UdonRadioCommunicationRedux;

public class CustomVoiceProtocol : VoiceProtocol
{
    public override void OnPlayerVoiceAdded(int playerId)
    {
        // プレイヤー追加時の処理
    }
    
    public override void OnPlayerVoiceRemoved(int playerId)
    {
        // プレイヤー削除時の処理
    }
}
```

### SaccFlight DFUNC の作成

SaccFlight用のカスタムDFUNCを作成する例：

```csharp
using UdonRadioCommunicationRedux.SaccFlight;
using SaccFlightAndVehicles;

public class DFUNC_CustomRadio : UdonSharpBehaviour
{
    public SFEXT_URC_VHF urc_vhf;
    
    public void SFEXT_L_EntityStart()
    {
        // 初期化処理
    }
    
    public void DFUNC_Selected()
    {
        // 選択時の処理
        gameObject.SetActive(true);
    }
    
    public void DFUNC_Deselected()
    {
        // 選択解除時の処理
        gameObject.SetActive(false);
    }
}
```

## 設定とベストプラクティス

### パフォーマンス考慮事項

1. **音声設定の更新頻度**: 音声設定の変更は必要最小限に留める
2. **チャンネル管理**: 不要なチャンネル登録は避ける
3. **イベント処理**: 重い処理はコルーチンや遅延実行を使用

### デバッグ方法

1. **ログ出力**: `Debug.Log` を使用して状態変化を追跡
2. **インスペクター監視**: 実行時にコンポーネントの値を監視
3. **VRChatログ**: VRChatのコンソールログを確認

```csharp
// ログ出力例
Debug.Log($"[URC] Channel changed: {oldChannel} -> {newChannel}");
Debug.Log($"[URC] Player {playerId} voice gain set to {gain}");
```

### エラーハンドリング

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

## API リファレンス

詳細なAPIリファレンスについては、各クラスのソースコードとコメントを参照してください。主要なクラスは以下の場所にあります：

- `Runtime/core/` - コアシステム
- `Runtime/channel/` - チャンネルシステム  
- `Runtime/zone/` - ゾーンシステム
- `Runtime/utilities/` - ユーティリティ

## さらなる情報

- [GitHub リポジトリ](https://github.com/VirtualAviationJapan/UdonRadioCommunications-Redux)
- [VPM パッケージ](https://vpm.virtualaviation.jp/)
- [Discord コミュニティ](https://discord.com/invite/Fpw7UeVnXZ)