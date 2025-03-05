using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    public class PlayerVoice : UdonSharpBehaviour
    {
        UdonRadioCommunication urc;

        // (よくないけど)処理高速化のため、配列をpublicにしている
        public bool[] enableVoiceProtocols;

        public void reset()
        {
            enableVoiceProtocols = new bool[urc.voiceProtocols.Length];
        }

        /// <summary>
        /// 有効な音量調整プロトコルのうち、最も優先度の高いものを返す
        /// 存在しない場合、nullを返す
        /// </summary>
        /// <returns></returns>
        public VoiceProtocol getCurrentAffectedProtocol()
        {
            // 最も優先度の高い音量調整プロトコルを選択する
            for (int i = enableVoiceProtocols.Length - 1; i >= 0; i--)
            {
                if (enableVoiceProtocols[i] == true) return urc.voiceProtocols[i];
            }
            return null;
        }

    }
}