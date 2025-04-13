using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    public class CastByChannel : VoiceProtocol
    {

        // public UdonRadioCommunication urc;
        // [HideInInspector] public int protocolPriority = -1;
        private DataDictionary RxStatus = new DataDictionary();
        private DataDictionary TxStatus = new DataDictionary();
        private DataDictionary HoldingChannelsByPlayer = new DataDictionary();

        [SerializeField] private float near = 99998;
        [SerializeField] private float far = 99999;
        [SerializeField] private float volumetricRadius = 10000;


        public override void OnPlayerVoiceAdded(int playerId)
        {

        }
        public void Subscribe(int channel)
        {
            // 聴取状態をtrueに変更
            RxStatus[channel] = true;
        }
        public void Publish(int playerId, int channel)
        {
            // 選択されたチャンネルにおける送信者の情報(作業用)
            DataDictionary channelTxStatus;
            // 選択されたチャンネルにおける送信者の情報を取得を、作業用領域にコピーする
            // もし存在しない場合、作業用領域は空のDataDictionaryを使用する
            if (TxStatus.TryGetValue(channel, TokenType.DataDictionary, out DataToken value))
            {
                channelTxStatus = value.DataDictionary;
            }
            else
            {
                channelTxStatus = new DataDictionary();
            }
            // 送信対象者の送信状態を追加する
            channelTxStatus[playerId] = true;
            // 送信対象者の送信状態を、作業用領域->保存領域にコピーする
            TxStatus[channel] = channelTxStatus;
        }

        public void Unpublish(int playerId, int channel)
        {
            // 選択されたチャンネルにおける送信者の情報(作業用)
            DataDictionary channelTxStatus;
            // 選択されたチャンネルにおける送信者の情報を取得を、作業用領域にコピーする
            // もし存在しない場合、作業用領域は空のDataDictionaryを使用する
            if (TxStatus.TryGetValue(channel, TokenType.DataDictionary, out DataToken value))
            {
                channelTxStatus = value.DataDictionary;
            }
            else
            {
                channelTxStatus = new DataDictionary();
            }
            // 送信対象者の送信状態を削除する
            channelTxStatus.Remove(playerId);
            // 送信対象者の送信状態を、作業用領域->保存領域にコピーする
            TxStatus[channel] = channelTxStatus;
        }

        private void CastPlayerVoice(int playerId, float gain)
        {
            if (isCast)
            {
                urc.SetVoiceGain(playerId, protocolPriority, gain);
                urc.SetVoiceNear(playerId, protocolPriority, near);
                urc.SetVoiceFar(playerId, protocolPriority, far);
                urc.SetVoiceVolumetricRadius(playerId, protocolPriority, volumetricRadius);
                urc.SetVoiceLowpass(playerId, protocolPriority, lowpass);

                urc.EnableVoiceProtocol(playerId, protocolPriority);
            }
        }
        

        private bool IsPlayerCast(int playerId)
        {
            if (HoldingChannelsByPlayer.TryGetValue(playerId, TokenType.DataDictionary, out DataToken value))
            {
                DataDictionary selectedChannels = value.DataDictionary;
                DataList RxChannels = RxStatus.GetKeys();

                for (int i = 0; i < RxChannels.Count; i++)
                {
                    if (selectedChannels.ContainsKey(RxChannels[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}