
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Receiver : UdonSharpBehaviour
    {
        public VoiceBroadcastByChannel channelManager;
        [FieldChangeCallback(nameof(Channel))] protected int channel;
        [FieldChangeCallback(nameof(RxPower))] protected bool rxPower;
        [FieldChangeCallback(nameof(Gain))] protected float gain;

        public virtual int Channel
        {
            get => channel;
            set
            {
                if (rxPower == true) StopReceive();
                channel = value;
                if (rxPower == true) StartReceive();
                OnUpdateChannel();
            }
        }
        public bool RxPower
        {
            get => rxPower;
            set
            {
                rxPower = value;
                if (rxPower == true)
                {
                    StartReceive();
                }
                else
                {
                    StopReceive();
                }
            }
        }
        public float Gain
        {
            get => gain;
            set
            {
                gain = value;
                if (rxPower == true) StartReceive();
                OnUpdateGain();
            }
        }

        #region Interaction
        public virtual void RxOn()
        {
            RxPower = true;
        }
        public virtual void RxOff()
        {
            RxPower = false;
        }
        public virtual void SetChannel(int nextChannel)
        {
            Channel = nextChannel;
        }
        public virtual void SetGain(int nextGain)
        {
            Gain = nextGain;
        }
        #endregion

        protected virtual void StartReceive()
        {
            channelManager.Subscribe(this, channel, gain);
        }
        protected virtual void StopReceive()
        {
            channelManager.UnSubscribe(this, channel);
        }
        #region callback
        public virtual void ChannelTransmitting() { }
        public virtual void ChannelNotTransmitting() { }
        public virtual void OnUpdateChannel() { }
        public virtual void OnUpdateGain() { }
        #endregion

    }
}