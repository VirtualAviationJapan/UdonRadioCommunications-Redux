---
title: チャンネルシステム
sidebar_position: 3
---

# チャンネルシステム

## Transceiver

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

## VoiceBroadcastByChannel

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