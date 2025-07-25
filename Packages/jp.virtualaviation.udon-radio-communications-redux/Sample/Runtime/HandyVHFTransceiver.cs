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
        [Header("Frequency")]

        [Tooltip("周波数最小値 単位:kHz(整数)")]
        [SerializeField] int minFrequency = 118000;

        [Tooltip("周波数最小値  単位:kHz(整数)")]
        [SerializeField] int maxFrequency = 136000;

        [Tooltip("周波数の変更段数（少な目）")]
        [SerializeField] int frequencyStep = 125;

        [Tooltip("周波数の変更段数(大き目)")]
        [SerializeField] int fastFrequencyStep = 1000;

        [Header("Volume")]

        [SerializeField]
        float[] volummeArray = { 0, 12, 15, 17 };
        [SerializeField] int currentVolumeIndex = 0;
        [SerializeField] int defaultVolumeIndex = 2;

        [Header("Optional")]
        [Tooltip("周波数表示部のテキスト")]
        [SerializeField] TextMeshPro frequencyText;

        [Tooltip("周波数表示部のフォーマット")]
        [SerializeField] string frequencyTextFormat = "000.00";

        [Tooltip("インジゲータ用アニメータ")]
        [SerializeField] Animator animator;

        public override void TxOn()
        {
            if (RxPower != true) return;
            base.TxOn();
        }
        public override void TxOff()
        {
            base.TxOff();
        }
        public override void RxOn()
        {
            if (Gain == 0) SetVolume(defaultVolumeIndex);
            base.RxOn();
        }
        public override void RxOff()
        {
            if (Networking.IsOwner(gameObject) == true && TxPower == true) TxOff();
            base.RxOff();
        }

        public void ToggleRx()
        {
            if (RxPower) RxOff();
            else RxOn();
        }
        public void ToggleTx()
        {
            if (TxPower) TxOff();
            else TxOn();
        }


        protected override void StartReceive()
        {
            base.StartReceive();
            // インジゲーターを更新
            animator.SetBool("PowerOn", true);
        }
        protected override void StopReceive()
        {
            base.StopReceive();
            // インジゲーターを更新
            animator.SetBool("PowerOn", false);
            animator.SetBool("Talking", false);
            animator.SetBool("Receiving", false);
        }
        protected override void StartTransmit()
        {
            base.StartTransmit();
            // インジゲーターを更新
            animator.SetBool("Talking", true);
        }
        protected override void StopTransmit()
        {
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
        public override void OnUpdateChannel()
        {
            float freqencyInVisual = channel * 0.001f;
            if (frequencyText != null) frequencyText.text = freqencyInVisual.ToString(frequencyTextFormat);
        }
        public override void OnUpdateGain()
        {
            animator.SetInteger("Volume", currentVolumeIndex);
        }
        #endregion

        #region Frequency
        public void IncreaseFrequency() { SetChannel(channel + frequencyStep); }
        public void DecreaseFrequency() { SetChannel(channel - frequencyStep); }
        public void IncreaseFrequencyFast() { SetChannel(channel + fastFrequencyStep); }
        public void DecreaseFrequencyFast() { SetChannel(channel - fastFrequencyStep); }
        public override void SetChannel(int nextFrequency)
        {
            Channel = Mathf.Clamp(nextFrequency, minFrequency, maxFrequency);
            if (Networking.IsOwner(gameObject) == false) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
        #endregion

        #region Volume
        public void IncreaseVolume() { SetVolume(currentVolumeIndex + 1); }
        public void DecreaseVolume() { SetVolume(currentVolumeIndex - 1); }
        public void SetVolume(int index)
        {
            currentVolumeIndex = Mathf.Clamp(index, 0, volummeArray.Length - 1);
            Gain = volummeArray[currentVolumeIndex];

            if (RxPower == true && Gain == 0) RxOff();
            if (RxPower == false && Gain != 0) RxOn();
        }
        #endregion

        #region lifecycle
        void Start()
        {
            Channel = Mathf.Clamp(Channel, minFrequency, maxFrequency);
            Gain = volummeArray[currentVolumeIndex];
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal && txPower) TxPower = false;
            if (player.isLocal && rxPower) RxPower = false;
        }
        #endregion
    }
}