using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using Utilities = VRC.SDKBase.Utilities;
using TMPro;
using UdonRadioCommunicationRedux;


namespace UdonRadioCommunicationRedux.Sample
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class HandyVHFTransceiver : Transceiver
    {
        public int minFrequency = 108000;
        public int maxFrequency = 118000;
        public int frequencyStep = 125;
        public int fastFrequencyStep = 1000;

        [Header("Optional")]
        public TextMeshPro frequencyText;
        public string frequencyTextFormat = "000.00";
        [Tooltip("Drives bool parameters \"PowerOn\" and \"Talking\"")] public Animator animator;

        #region Interaction
        public void RxOn()
        {
            RxPower = true;
        }
        public void RxOff()
        {
            RxPower = false;
        }
        public void TxOn()
        {
            if (RxPower == false) return;
            if (Networking.IsOwner(gameObject) == false)
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            TxPower = true;
            RequestSerialization();
        }
        public void TxOff()
        {
            TxPower = false;
            RequestSerialization();
        }

        #endregion


        protected override void StartReceive()
        {
            ValidateFrequency();
            base.StartReceive();
            // インジゲーターを更新
            animator.SetBool("PowerOn", true);
        }
        protected override void StopReceive()
        {
            ValidateFrequency();
            base.StopReceive();
            // インジゲーターを更新
            animator.SetBool("PowerOn", false);
        }
        protected override void StartTransmit()
        {
            ValidateFrequency();
            base.StartTransmit();
            // インジゲーターを更新
            animator.SetBool("Talking", true);
        }
        protected override void StopTransmit()
        {
            ValidateFrequency();
            base.StopTransmit();
            // インジゲーターを更新
            animator.SetBool("Talking", false);
        }

        #region callback
        public override void ChannelTransmitting()
        {
            base.ChannelNotTransmitting();
            // インジゲーターを更新
            animator.SetBool("Receiving", true);
        }
        public override void ChannelNotTransmitting()
        {
            base.ChannelNotTransmitting();
            // インジゲーターを更新
            animator.SetBool("Receiving", false);

        }
        #endregion

        #region appearance
        public void UpdateFrequencyText()
        {
            float freqencyInVisual = channel * 0.001f;
            if (frequencyText != null) frequencyText.text = freqencyInVisual.ToString(frequencyTextFormat);
        }
        #endregion

        #region Frequency
        public void IncreaseFrequency() { channel += frequencyStep; ValidateFrequency(); }
        public void DecreaseFrequency() { channel -= frequencyStep; ValidateFrequency(); }
        public void IncreaseFrequencyFast() { channel += fastFrequencyStep; ValidateFrequency(); }
        public void DecreaseFrequencyFast() { channel -= fastFrequencyStep; ValidateFrequency(); }
        private void ValidateFrequency()
        {
            if (channel < minFrequency) channel = minFrequency;
            if (channel > maxFrequency) channel = maxFrequency;
            UpdateFrequencyText();
        }
        #endregion
    }
}