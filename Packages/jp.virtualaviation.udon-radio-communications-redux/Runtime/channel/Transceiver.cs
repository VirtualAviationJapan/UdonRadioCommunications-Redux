using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using Utilities = VRC.SDKBase.Utilities;


namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Transceiver : UdonSharpBehaviour
    {
        public VoiceBroadcastByChannel channelManager;
        [UdonSynced, FieldChangeCallback(nameof(Channel))] protected int channel;
        [UdonSynced, FieldChangeCallback(nameof(TxPower))] protected bool txPower;
        [FieldChangeCallback(nameof(RxPower))] protected bool rxPower;
        [FieldChangeCallback(nameof(Gain))] protected float gain;

        public virtual int Channel
        {
            get => channel;
            set
            {
                Debug.Log("Transceiver: channel changed");
                if (rxPower == true) StopReceive();
                if (txPower == true) StopTransmit();
                channel = value;
                if (rxPower == true) StartReceive();
                if (txPower == true) StartTransmit();
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
                    StopTransmit();
                }
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
        public virtual void TxOn()
        {
            if (Networking.IsOwner(gameObject) == false) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            TxPower = true;
            RequestSerialization();
        }
        public virtual void TxOff()
        {
            if (Networking.IsOwner(gameObject) == false) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            TxPower = false;
            RequestSerialization();
        }
        public virtual void SetChannel(int nextChannel)
        {
            Channel = nextChannel;
            if (Networking.IsOwner(gameObject) == false) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
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
        protected virtual void StartTransmit()
        {
            channelManager.Publish(this, Networking.GetOwner(gameObject).playerId, channel);
        }
        protected virtual void StopTransmit()
        {
            channelManager.Unpublish(this, Networking.GetOwner(gameObject).playerId, channel);
        }

        #region callback
        public virtual void ChannelTransmitting() { }
        public virtual void ChannelNotTransmitting() { }
        public virtual void OnUpdateChannel() { }
        public virtual void OnUpdateGain() { }
        #endregion

        #region event
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (player.isLocal && txPower) TxPower = false;
        }
        #endregion
    }
}