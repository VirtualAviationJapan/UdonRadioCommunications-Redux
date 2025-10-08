---
title: 拡張方法
sidebar_position: 5
---

# 拡張方法

## カスタムTransceiverの作成

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

## カスタムVoiceProtocolの作成

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

## SaccFlight DFUNC の作成

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