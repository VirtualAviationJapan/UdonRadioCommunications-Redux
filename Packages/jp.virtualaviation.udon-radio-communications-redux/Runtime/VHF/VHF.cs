using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    public class VHF : VoiceProtocol
    {
        // public VHFReceiver[] receivers;
        // public PlayerVHF[] playerVHFs;
        public int channelLength = 720;
        // チャンネルが指定された数
        public int[] rxChannelCount;

        #region Speaking
        public void TxOn(VRCPlayerApi player, int channel)
        {

        }
        public void TxOff(VRCPlayerApi player, int channel)
        {

        }
        #endregion

        #region Listening
        // ローカルプレイヤーが聴取するチャンネルを追加する
        public void subscribeRxChannel(int channel)
        {
            rxChannelCount[channel] += 1;
        }
        public void unsubscribeRxChannel(int channel)
        {
            if (rxChannelCount[channel] > 0) rxChannelCount[channel] -= 1;
        }
        #endregion

        protected override void validate()
        {
            rxChannelCount = new int[channelLength];
        }
    }
}