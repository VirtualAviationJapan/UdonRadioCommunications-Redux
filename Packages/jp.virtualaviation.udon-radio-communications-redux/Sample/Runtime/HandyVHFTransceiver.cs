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
        [Tooltip("周波数最小値 x1000(整数で保持するため)")]
        public int minFrequency = 118000;
        [Tooltip("周波数最小値 x1000(整数で保持するため)")]

        public int maxFrequency = 136000;
        [Tooltip("周波数の変更段数（少な目）")]
        public int frequencyStep = 125;
        [Tooltip("周波数の変更段数(大き目)")]

        public int fastFrequencyStep = 1000;

        [Header("Optional")]
        [Tooltip("周波数表示部のテキスト")]
        public TextMeshPro frequencyText;
        [Tooltip("周波数表示部のフォーマット")]
        public string frequencyTextFormat = "000.00";
        [Tooltip("送信/受信状態インジゲータ用アニメータ")] public Animator animator;

        public override void TxOn()
        {
            if (RxPower != true) return;
            base.TxOn();
        }
        public override void RxOff()
        {
            if (TxPower == true) TxOff();
            base.RxOff();
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

        #region lifecycle
        void Start()
        {
            Channel = Mathf.Clamp(Channel, minFrequency, maxFrequency);
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal && txPower) TxPower = false;
            if (player.isLocal && rxPower) RxPower = false;
        }
        #endregion
    }
}