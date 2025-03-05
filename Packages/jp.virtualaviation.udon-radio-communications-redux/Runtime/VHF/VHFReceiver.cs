using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    public class VHFReceiver : UdonSharpBehaviour
    {
        public VHF vhf;
        bool SW;
        int rxChannel;
        float gain;

        /// <summary>
        /// この受信機は、引数のチャンネルを受信中か？
        /// </summary>
        /// <param name="ch">受信中か否かを調査するチャンネル</param>
        /// <returns>受信中 = true, チャンネルが別 or 電源Off = false </returns>
        public bool isRxSelect(int ch)
        {
            if (SW == true && rxChannel == ch) return true;
            return false;
        }

        public int getRxChannel() { return rxChannel; }

        // 周波数を変更する
        // いままで電源がOnの場合：聴取するチャンネルを切り替える
        // いままで電源がOffの場合：何もしない
        public void setRxChannel(int nextChannel)
        {
            if (SW == true)
            {
                vhf.unsubscribeRxChannel(rxChannel);
                vhf.subscribeRxChannel(nextChannel);
            }
            rxChannel = nextChannel;
        }

        // Off -> On：指定チャンネルの聴取を開始する
        // On -> Off：指定チャンネルの聴取を停止する
        public void setSW(bool nextSW)
        {
            if (nextSW == true && SW == false) vhf.subscribeRxChannel(rxChannel);
            if (nextSW == false && SW == true) vhf.unsubscribeRxChannel(rxChannel);
            SW = nextSW;
        }

        /// <summary>
        /// VHFの受信音量を変更する
        /// </summary>
        #region Volume
        public void setVolume(float gain)
        {

        }
        #endregion

    }
}