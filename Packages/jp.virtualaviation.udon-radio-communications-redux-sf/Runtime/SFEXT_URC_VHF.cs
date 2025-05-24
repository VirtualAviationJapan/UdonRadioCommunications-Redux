using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using UdonRadioCommunicationRedux;
using SaccFlightAndVehicles;

namespace UdonRadioCommunicationRedux.SaccFlight
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]

    public class SFEXT_URC_VHF : Transceiver
    {
        [Header("周波数範囲")]
        [Tooltip("周波数最小値 x1000(整数で保持するため)")]
        public int minFrequency = 118000;
        [Tooltip("周波数最小値 x1000(整数で保持するため)")]

        public int maxFrequency = 136000;
        [Tooltip("周波数の変更ステップ")]
        public int frequencyStep = 125;

        [Header("表示系設定")]
        public UdonSharpBehaviour[] CallbackBehaviours;

        #region Saccflight Event
        public void SFEXT_L_EntityStart()
        {
            channel = Mathf.Clamp(channel, minFrequency, maxFrequency);
            OnUpdateChannel();
        }

        #endregion

        private float inputOrigin;
        private bool prevTriggered;
        private int channelTriggerStarted;

        #region Tx/Rx Management

        public void ToggleRx()
        {
            if (rxPower == false) RxOn();
            else RxOff();
        }
        public void ToggleTx()
        {
            if (txPower == false && rxPower == true) TxOn();
            else TxOff();
        }

        public override void TxOn()
        {
            if (rxPower == true) base.TxOn();
        }
        public override void RxOff()
        {
            if (txPower == true) TxOff();
            base.RxOff();
        }


        protected override void StartReceive()
        {
            base.StartReceive();
            OnStartReceive();
        }
        protected override void StopReceive()
        {
            base.StopReceive();
            OnStopReceive();
        }
        protected override void StartTransmit()
        {
            base.StartTransmit();
            OnStartTransmit();
        }
        protected override void StopTransmit()
        {
            base.StopTransmit();
            OnStopTransmit();
        }

        #endregion

        #region Frequency Management
        public void StepFrequency(int step) => SetChannel(channel + step * frequencyStep);
        public void IncreaseFrequency() => SetChannel(channel + frequencyStep);
        public void DecreaseFrequency() => SetChannel(channel - frequencyStep);

        public override void SetChannel(int nextChannel)
        {
            Channel = Mathf.Clamp(nextChannel, minFrequency, maxFrequency);
            if (Networking.IsOwner(gameObject) == false) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
        #endregion

        #region Appearance Callback
        private void Callback(string callbackEvent)
        {
            foreach (var b in CallbackBehaviours)
            {
                if (Utilities.IsValid(b))
                {
                    b.SendCustomEvent(callbackEvent);
                }
            }
        }

        private void OnStartReceive()
        {
            Callback(nameof(OnStartReceive));
        }
        private void OnStopReceive()
        {
            Callback(nameof(OnStopReceive));
        }
        private void OnStartTransmit()
        {
            Callback(nameof(OnStartTransmit));
        }
        private void OnStopTransmit()
        {
            Callback(nameof(OnStopTransmit));
        }

        public override void ChannelTransmitting()
        {
            Callback(nameof(ChannelTransmitting));
        }
        public override void ChannelNotTransmitting()
        {
            Callback(nameof(ChannelNotTransmitting));
        }
        public override void OnUpdateChannel()
        {
            Callback(nameof(OnUpdateChannel));
        }
        #endregion
    }
}