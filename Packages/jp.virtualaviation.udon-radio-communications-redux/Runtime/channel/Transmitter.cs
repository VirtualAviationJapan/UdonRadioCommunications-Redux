using UdonRadioCommunicationRedux;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Transmitter : UdonSharpBehaviour
{
    public VoiceBroadcastByChannel channelManager;
    [FieldChangeCallback(nameof(Channel))] protected int channel;
    [FieldChangeCallback(nameof(TransmitterID))] protected int transmitterID;
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
        get => transmitterID;
        set
        {
            TransmitterPlayer = VRCPlayerApi.GetPlayerById(value);
        }
    }
    public virtual VRCPlayerApi TransmitterPlayer
    {
        get{
            VRCPlayerApi transmitterPlayer = VRCPlayerApi.GetPlayerById(transmitterID);
            return transmitterPlayer;
        }
        set
        {
            if (txPower == true) StopTransmit();
            if (value != null)
            {
                Debug.Log("Transceiver: transmitterPlayer changed");
                transmitterID = value.playerId;
            }
            else
            {
                transmitterID = -1;
            }
            if (txPower == true) StartTransmit();
            OnUpdateTransmitter();
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
        channelManager.Publish(this, transmitterID, channel);
    }
    protected virtual void StopTransmit()
    {
        channelManager.Unpublish(this, transmitterID, channel);
    }

    #region callback
    public virtual void ChannelTransmitting() { }
    public virtual void ChannelNotTransmitting() { }
    public virtual void OnUpdateChannel() { }
    public virtual void OnUpdateTransmitter() { }
    public virtual void OnUpdateGain() { }
    #endregion
}