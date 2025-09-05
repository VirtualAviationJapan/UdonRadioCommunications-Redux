---
title: コアシステム
sidebar_position: 2
---

# コアシステム

## UdonRadioCommunication

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

## VoiceProtocol

Runtime/coreの機能を用い、音声設定を変更する機能を作成する際は、`VoiceProtocol`を継承したコンポーネントを作成してください。
`VoiceProtocol`は、以下を提供します。

- Runtime/core に対し、音声設定の優先度を指定
- Runtime/coreに対し、プレイヤーが新規登録された際のイベント関数呼び出し

## PlayerVoice

`PlayerVoice`は個別プレイヤーの音声設定を管理するコンポーネントです。