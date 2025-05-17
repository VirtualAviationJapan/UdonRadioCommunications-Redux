using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    public class VoiceBroadcastByChannel : VoiceProtocol
    {

        // public UdonRadioCommunication urc;
        // [HideInInspector] public int protocolPriority = -1;
        private DataDictionary RxChannelState = new DataDictionary();
        private DataDictionary TxChannelState = new DataDictionary();
        private DataDictionary TxPlayerState = new DataDictionary();

        // 制約条件として、near, far, volumetricradiusは実行中固定値扱いとする
        [SerializeField] private float near = 99998;
        [SerializeField] private float far = 99999;
        [SerializeField] private float volumetricRadius = 10000;
        [SerializeField] private bool lowpass = true;

        public override void OnPlayerVoiceAdded(int playerId)
        {
            // near, far, volumetricRadiusの設定を波及させる
            InitializePlayerVoice(playerId);
        }

        #region public api
        /// <summary>
        /// ローカルプレイヤーが、あるチャンネルを追加で聴取する
        /// </summary>
        /// <param name="channel">聴取対象に追加したいチャンネルに紐づくID</param>
        public void Subscribe(int channel, int instanceId, float gain)
        {
            // 1. あるチャンネルに、受信機の情報(受信機に固有のID,受信機の音量)を追加する
            // 2. ローカルプレイヤーについて、あるチャンネルの聴取音量を更新する
            // 3. 1.2.の結果を 受信チャンネルのデータベースに保存する
            // 4. 2.の結果から、プレイヤーの拡声音量を更新する

            // 選択されたチャンネルにおける受信機の情報(作業用)
            DataDictionary channelRxStatus = GetChildrenFromDictionary(RxChannelState, channel);
            // 1.「ある受信機は、gainの音量をもつ」を作業用領域に追加
            channelRxStatus[instanceId] = gain;
            // 3. 作業用領域->保存領域にコピー
            RxChannelState[channel] = channelRxStatus;

            // 2. あるチャンネルに備わるゲインから、最大のゲインを選択する
            float nextChannelGain = GetChannelRxGain(channelRxStatus);
            // 4. プレイヤーの拡声音量を更新する
            // 変更したチャンネルにおいて、送信中のプレイヤーのリストを取得する
            DataList transmittingPlayers = GetTransmittingPlayerList(channel);
            // 今回設定を変更したチャンネルにて送信中の各プレイヤーの音声設定を更新する
            foreach (int playerId in transmittingPlayers)
            {
                TrySetPlayerVoiceGain(playerId, channel, nextChannelGain);
            }
        }

        /// <summary>
        /// ローカルプレイヤーが、あるチャンネルの聴取を取りやめる
        /// </summary>
        /// <param name="channel">聴取対象に追加したいチャンネルに紐づくID</param>
        public void UnSubscribe(int channel, int instanceId)
        {
            DataDictionary channelRxStatus = GetChildrenFromDictionary(RxChannelState, channel);
            // 1.「ある受信機は、gainの音量をもつ」を作業用領域に追加
            channelRxStatus.Remove(instanceId);
            // 3. 作業用領域->保存領域にコピー
            RxChannelState[channel] = channelRxStatus;

            // 2. あるチャンネルに備わるゲインから、最大のゲインを選択する
            float nextChannelGain = GetChannelRxGain(channelRxStatus);
            // 4. プレイヤーの拡声音量を更新する
            // 変更したチャンネルにおいて、送信中のプレイヤーのリストを取得する
            DataList transmittingPlayers = GetTransmittingPlayerList(channel);
            // 今回設定を変更したチャンネルにて送信中の各プレイヤーの音声設定を更新する
            foreach (int playerId in transmittingPlayers)
            {
                TrySetPlayerVoiceGain(playerId, channel, nextChannelGain);
            }

        }

        public void Publish(int channel, int instanceId, int playerId)
        {
            // 選択されたチャンネルにおける送信者の情報(作業用)
            DataDictionary channelTxStatus = GetChildrenFromDictionary(TxChannelState, channel);
            DataDictionary channelplayerTxStatus = GetChildrenFromDictionary(channelTxStatus, playerId);
            // 送信対象者の送信状態を追加する
            channelplayerTxStatus[instanceId] = true;
            // 送信対象者の送信状態を、作業用領域->保存領域にコピーする
            TxChannelState[channel] = channelTxStatus;

            float channelGain = GetChannelRxGain(channel);


            if (TxChannelState.TryGetValue(0, TokenType.Float, out DataToken channelGainToken))
            {
                TrySetPlayerVoiceGain(playerId, channel, channelGainToken.Float);
            }
        }

        public void Unpublish(int channel, int instanceId, int playerId)
        {
            // 選択されたチャンネルにおける送信者の情報を取得を、作業用領域にコピーする
            // もし存在しない場合、作業用領域は空のDataDictionaryを使用する
            DataDictionary channelTxStatus = GetChildrenFromDictionary(TxChannelState, channel);
            DataDictionary channelPlayerTxStatus = GetChildrenFromDictionary(channelTxStatus, playerId);
            channelPlayerTxStatus.Remove(instanceId);
            setOrDeleteChildren(channelTxStatus, channelPlayerTxStatus, playerId);
            setOrDeleteChildren(TxChannelState, channelTxStatus, channel);

            DataDictionary playerTxStatus = GetChildrenFromDictionary(TxPlayerState, playerId);
            DataDictionary playerChannelTxStatus = GetChildrenFromDictionary(playerTxStatus, channel);
            playerChannelTxStatus.Remove(instanceId);
            setOrDeleteChildren(playerTxStatus, playerChannelTxStatus, channel);
            setOrDeleteChildren(TxPlayerState, playerTxStatus, playerId);
        }
        #endregion

        #region voice manipulation
        private void InitializePlayerVoice(int playerId)
        {
            urc.SetVoiceNear(playerId, protocolPriority, near);
            urc.SetVoiceFar(playerId, protocolPriority, far);
            urc.SetVoiceVolumetricRadius(playerId, protocolPriority, volumetricRadius);
            urc.SetVoiceLowpass(playerId, protocolPriority, lowpass);
            urc.DisableVoiceProtocol(playerId, protocolPriority);
        }

        private void IniializeAllPlayerVoice()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[100];
            VRCPlayerApi.GetPlayers(players);

            foreach (VRCPlayerApi player in players)
            {
                if (player == null) continue;
                InitializePlayerVoice(player.playerId);
            }
        }

        #endregion

        /// <summary>
        /// そのチャンネルにおける、現在の受信音量を取得する
        /// そのチャンネルを聴取中の受信機がない場合、0を返す
        /// </summary>
        /// <param name="channelRxStatus">現在のチャンネルに紐づいた受信機の状態</param>
        /// <returns>受信音量の最大値</returns>
        private float GetChannelRxGain(DataDictionary channelRxStatus)
        {
            DataList channnelGainList = channelRxStatus.GetValues();
            if (channnelGainList.Count == 0) return 0;
            channnelGainList.Sort();
            return nextChannelGain;
        }

        /// <summary>
        /// 現在、指定したチャンネルから送信中のプレイヤーの一覧を取得する
        /// </summary>
        /// <param name="channel">検索対象のチャンネル</param>
        /// <returns>送信中プレイヤー一覧</returns>
        private DataList GetTransmittingPlayerList(int channel)
        {
            if (TxChannelState.TryGetValue(channel, TokenType.DataDictionary, out DataToken value))
            {
                return value.DataDictionary.GetKeys();
            }
            return new DataList();
        }

        private static DataDictionary GetChildrenFromDictionary(DataDictionary dict, int id)
        {
            DataDictionary newDictionary;
            if (dict.TryGetValue(id, TokenType.DataDictionary, out DataToken value))
            {
                newDictionary = value.DataDictionary;
            }
            else
            {
                newDictionary = new DataDictionary();
            }
            return newDictionary;
        }

        private static void setOrDeleteChildren(DataDictionary dict, DataDictionary child, int id)
        {
            if (child.Count == 0)
            {
                dict.Remove(id);
            }
            else
            {
                dict[id] = child;
            }
        }

        /// <summary>
        /// あるプレイヤー、あるチャンネルにおける拡声音量を更新する
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="channel"></param>
        /// <param name="gain"></param>
        /// <returns></returns>
        private void TrySetPlayerVoiceGain(int playerId, int channel, float gain)
        {
            DataDictionary playerTxState;
            if (TxPlayerState.TryGetValue(playerId, TokenType.DataDictionary, out DataToken value))
            {
                playerTxState = value.DataDictionary;
            }
            else { return; }

            playerTxState[channel] = gain;
            playerTxGainList = playerTxState.GetValues();
            playerTxGainList.Sort();
            float nextGain = playerTxGainList[playerTxGainList.Count - 1].Float;

            urc.SetVoiceGain(playerId, protocolPriority, gain);
            if (nextGain > 0) urc.EnableVoiceProtocol(playerId, protocolPriority);
            else urc.DisableVoiceProtocol(playerId, protocolPriority);

        }
    }
}