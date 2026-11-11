using UdonRadioCommunicationRedux;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Transmitter : UdonSharpBehaviour
{
    public VoiceBroadcastByChannel channelManager;
    [FieldChangeCallback(nameof(Channel))] protected int channel;
    [FieldChangeCallback(nameof(TransmitterPlayer))] VRCPlayerApi transmitterPlayer;
    [FieldChangeCallback(nameof(TxPower))] protected bool txPower;

    public virtual int Channel
    {
        get => channel;
        set
        {
            Debug.Log("Transceiver: channel changed");
            if (txPower == true) StopTransmit();
            channel = value;
            if (txPower == true) StartTransmit();
            OnUpdateChannel();
        }
    }

    private int TransmitterID
    {
        get{
            if(transmitterPlayer != null && transmitterPlayer.IsValid())
            {
                return transmitterPlayer.playerId;
            }
            return -1;
        }
        set
        {
            TransmitterPlayer = VRCPlayerApi.GetPlayerById(value);
        }
    }
    public virtual VRCPlayerApi TransmitterPlayer
    {
        get => transmitterPlayer;
        set
        {
            if (txPower == true) StopTransmit();
            Debug.Log("Transceiver: transmitterPlayer changed");
            transmitterPlayer = value;
            if (txPower == true) StartTransmit();
            OnUpdateTransmitterPlayer();
        }
    }

    public bool TxPower
    {
        get => txPower;
        set
        {
            txPower = value;
            if (txPower == true)
            {
                StartTransmit();
            }
            else
            {
                StopTransmit();
            }
        }
    }

    #region Interaction
    public virtual void TxOn()
    {
        TxPower = true;
    }
    public virtual void TxOff()
    {
        TxPower = false;
    }
    public virtual void SetChannel(int nextChannel)
    {
        Channel = nextChannel;
    }
    #endregion
    protected virtual void StartTransmit()
    {
        if (TransmitterID >= 0) channelManager.Publish(this, TransmitterID, channel);
    }
    protected virtual void StopTransmit()
    {
        if (TransmitterID >= 0) channelManager.Unpublish(this, TransmitterID, channel);
    }

    #region callback
    public virtual void ChannelTransmitting() { }
    public virtual void ChannelNotTransmitting() { }
    public virtual void OnUpdateChannel() { }
    public virtual void OnUpdateTransmitterPlayer() { }
    public virtual void OnUpdateGain() { }
    #endregion
}