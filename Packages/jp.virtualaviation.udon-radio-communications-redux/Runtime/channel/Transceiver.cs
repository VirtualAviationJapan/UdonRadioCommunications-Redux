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
        [UdonSynced, FieldChangeCallback(nameof(Channel))] public int channel;
        [UdonSynced, FieldChangeCallback(nameof(TxPower))] public bool txPower;
        [FieldChangeCallback(nameof(RxPower))] public bool rxPower;

        public int Channel
        {
            get => channel;
            set
            {
                channel = value;
                if (rxPower == true)
                {
                    StartReceive();
                }
                if (rxPower == true && txPower == true)
                {
                    StartTransmit();
                }
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
                RequestSerialization();
            }
        }

        public float gain = 17;

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
        #endregion

        #region event
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isLocal && txPower) StartTransmit();
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal && txPower) TxPower = false;
        }
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (player.isLocal && txPower) TxPower = false;
        }
        #endregion
    }
}