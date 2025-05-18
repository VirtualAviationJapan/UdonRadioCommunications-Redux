using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VoiceBroadcastByChannel : VoiceProtocol
    {

        // public UdonRadioCommunication urc;
        // [HideInInspector] public int protocolPriority = -1;

        /// <summary>
        /// あるチャンネルにおける、ある受信機の設定音量
        /// </summary>
        private DataDictionary RxChannelGainState = new DataDictionary();
        /// <summary>
        /// あるチャンネルにおける、ある受信機の実体
        /// </summary>
        private DataDictionary RxChannelReceivers = new DataDictionary();

        /// <summary>
        /// あるチャンネルにおける、あるプレイヤーが保持する送信機
        /// </summary>
        private DataDictionary TxChannelPlayerState = new DataDictionary();
        /// <summary>
        /// あるプレイヤーの、あるチャンネルにおける、受信側の音量
        /// </summary>
        private DataDictionary TxPlayerChannelGain = new DataDictionary();

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
        /// 受信機の受信状態・音量を追加/更新する
        /// </summary>
        /// <param name="receriver">受信機</param>
        /// <param name="channel">聴取対象に追加したいチャンネルID</param>
        /// <param name="gain">受信機の音量</param>
        public void Subscribe(UdonSharpBehaviour receriver, int channel, float gain)
        {
            // 受信機のインスタンスIDを取得する
            int instanceId = receriver.GetInstanceID();

            //　「ある受信機は、gainの音量をもつ」を作業用領域に追加
            DataDictionary channelRxStatus = GetChildrenFromDictionary(RxChannelGainState, channel);
            channelRxStatus[instanceId] = gain;
            RxChannelGainState[channel] = channelRxStatus;

            // 受信機の実体を保存する
            DataDictionary channelRxReceivers = GetChildrenFromDictionary(RxChannelReceivers, channel);
            channelRxReceivers[instanceId] = receriver;
            RxChannelReceivers[channel] = channelRxReceivers;

            // 今回設定を変更したチャンネルにおける、最大のゲインを選択する
            float nextChannelGain = GetChannelRxGain(channelRxStatus);
            // プレイヤーの拡声音量を更新する
            // 変更したチャンネルにて送信中のプレイヤーのリストを取得する
            DataList transmittingPlayers = GetTransmitingPlayers(TxChannelPlayerState, channel);
            // 今回設定を変更したチャンネルにて送信中の各プレイヤーの音声設定を更新する
            foreach (int playerId in transmittingPlayers.ToArray())
            {
                if (TxPlayerChannelGain.TryGetValue(playerId, TokenType.DataDictionary, out DataToken value))
                {
                    DataDictionary playerTxGain = value.DataDictionary;
                    playerTxGain[channel] = nextChannelGain;
                    TxPlayerChannelGain[playerId] = playerTxGain;
                    UpdatePlayerVoiceGain(playerId, playerTxGain);
                }
            }
            // 受信機に、送信中の送信機の有無を通知する
            if (transmittingPlayers.Count > 0)
            {
                receriver.SendCustomEvent("ChannelTransmitting");
            }
            else
            {
                receriver.SendCustomEvent("ChannelNotTransmitting");
            }
        }

        /// <summary>
        /// 受信機の受信状態を停止する
        /// </summary>
        /// <param name="receriver">受信機</param>
        /// <param name="channel">変更するチャンネルID</param>
        /// 
        public void UnSubscribe(UdonSharpBehaviour receriver, int channel)
        {
            // 受信機のインスタンスIDを取得する
            int instanceId = receriver.GetInstanceID();
            // 選択されたチャンネルにおける受信機の情報(作業用)
            DataDictionary channelRxStatus = GetChildrenFromDictionary(RxChannelGainState, channel);
            // 受信機を削除
            channelRxStatus.Remove(instanceId);
            // 作業用領域->保存領域にコピー
            RxChannelGainState[channel] = channelRxStatus;

            // 受信機の実体を削除する
            DataDictionary channelRxReceivers = GetChildrenFromDictionary(RxChannelReceivers, channel);
            channelRxReceivers.Remove(instanceId);
            RxChannelReceivers[channel] = channelRxReceivers;

            // あるチャンネルに備わるゲインから、最大のゲインを選択する
            float nextChannelGain = GetChannelRxGain(channelRxStatus);
            // プレイヤーの拡声音量を更新する
            // 変更したチャンネルにて送信中のプレイヤーのリストを取得する
            DataList transmittingPlayers = GetTransmitingPlayers(TxChannelPlayerState, channel);
            // 今回設定を変更したチャンネルにて送信中の各プレイヤーの音声設定を更新する
            foreach (int playerId in transmittingPlayers.ToArray())
            {
                if (TxPlayerChannelGain.TryGetValue(playerId, TokenType.DataDictionary, out DataToken value))
                {
                    DataDictionary playerTxGain = value.DataDictionary;
                    playerTxGain[channel] = nextChannelGain;
                    TxPlayerChannelGain[playerId] = playerTxGain;
                    UpdatePlayerVoiceGain(playerId, playerTxGain);
                }
            }
        }

        /// <summary>
        /// あるチャンネルにおける送信状態を追加する
        /// </summary>
        /// <param name="transmitter">送信機</param>
        /// <param name="playerId">送信対象のプレイヤーID</param>
        /// <param name="channel">送信対象のチャンネルID</param>
        public void Publish(UdonSharpBehaviour transmitter, int playerId, int channel)
        {
            // 送信対象が自分の場合、処理を行わない
            // if (playerId == Networking.LocalPlayer.playerId) return;
            // 受信機のインスタンスIDを取得する
            int instanceId = transmitter.GetInstanceID();
            // 選択されたチャンネルにおける送信者の情報(作業用)
            DataDictionary channelTxStatus = GetChildrenFromDictionary(TxChannelPlayerState, channel);
            DataDictionary channelplayerTxStatus = GetChildrenFromDictionary(channelTxStatus, playerId);
            // 送信対象者の送信状態を追加する
            channelplayerTxStatus[instanceId] = true;
            // 送信対象者の送信状態を、作業用領域->保存領域にコピーする
            channelTxStatus[playerId] = channelplayerTxStatus;
            TxChannelPlayerState[channel] = channelTxStatus;

            float nextChannelGain = GetChannelRxGain(GetChildrenFromDictionary(RxChannelGainState, channel));

            DataDictionary playerTxGain = GetChildrenFromDictionary(TxPlayerChannelGain, playerId);

            playerTxGain[channel] = nextChannelGain;
            TxPlayerChannelGain[playerId] = playerTxGain;
            UpdatePlayerVoiceGain(playerId, playerTxGain);
            // 受信機に、送信中の送信機の有無を通知する
            CallbackRxTransmittingState(channel);
        }

        /// <summary>
        /// あるチャンネルにおける送信状態を停止する
        /// </summary>
        /// <param name="transmitter">送信機</param>
        /// <param name="playerId">送信対象のプレイヤーID</param>
        /// <param name="channel">送信対象のチャンネルID</param>
        public void Unpublish(UdonSharpBehaviour transmitter, int playerId, int channel)
        {
            // 送信対象が自分の場合、処理を行わない
            // if (playerId == Networking.LocalPlayer.playerId) return;
            // 受信機のインスタンスIDを取得する
            int instanceId = transmitter.GetInstanceID();
            // 選択されたチャンネルにおける送信者の情報を取得を、作業用領域にコピーする
            // もし存在しない場合、作業用領域は空のDataDictionaryを使用する
            DataDictionary channelTxStatus = GetChildrenFromDictionary(TxChannelPlayerState, channel);
            DataDictionary channelPlayerTxStatus = GetChildrenFromDictionary(channelTxStatus, playerId);
            channelPlayerTxStatus.Remove(instanceId);
            setOrDeleteChildren(channelTxStatus, channelPlayerTxStatus, playerId);
            setOrDeleteChildren(TxChannelPlayerState, channelTxStatus, channel);

            // 当該プレイヤーの、状態更新後の音量を更新する
            if (TxPlayerChannelGain.TryGetValue(playerId, TokenType.DataDictionary, out DataToken value))
            {
                DataDictionary playerTxGain = value.DataDictionary;
                playerTxGain.Remove(channel);
                TxPlayerChannelGain[playerId] = playerTxGain;

                UpdatePlayerVoiceGain(playerId, playerTxGain);
            }
            // 受信機に、送信中の送信機の有無を通知する
            CallbackRxTransmittingState(channel);
        }

        #endregion

        #region voice manipulation
        private void InitializePlayerVoice(int playerId)
        {
            urc.SetVoiceNear(playerId, protocolPriority, near);
            urc.SetVoiceFar(playerId, protocolPriority, far);
            urc.SetVoiceVolumetricRadius(playerId, protocolPriority, volumetricRadius);
            urc.SetVoiceLowpass(playerId, protocolPriority, lowpass);
            // urc.DisableVoiceProtocol(playerId, protocolPriority);
        }

        private void IniializeAllPlayerVoice()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
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
            return channnelGainList[channnelGainList.Count - 1].Float;
        }

        private void CallbackRxTransmittingState(int channel)
        {
            DataDictionary channelRxReceivers = GetChildrenFromDictionary(RxChannelReceivers, channel);
            DataList transmittingPlayers = GetTransmitingPlayers(TxChannelPlayerState, channel);
            if (transmittingPlayers.Count > 0)
            {
                foreach (DataToken instance in channelRxReceivers.GetValues().ToArray())
                {
                    UdonSharpBehaviour usb = (UdonSharpBehaviour)instance.Reference;
                    usb.SendCustomEvent("ChannelTransmitting");
                }
            }
            else
            {
                foreach (DataToken instance in channelRxReceivers.GetValues().ToArray())
                {
                    UdonSharpBehaviour usb = (UdonSharpBehaviour)instance.Reference;
                    usb.SendCustomEvent("ChannelNotTransmitting");
                }
            }
        }

        private DataList GetTransmitingPlayers(DataDictionary TxChannelPlayerState, int channel)
        {
            if (TxChannelPlayerState.TryGetValue(channel, TokenType.DataDictionary, out DataToken channelTxState))
            {
                return channelTxState.DataDictionary.GetKeys();
            }
            return new DataList();
        }

        private void UpdatePlayerVoiceGain(int playerId, DataDictionary playerTxState)
        {
            DataList playerTxGainList = playerTxState.GetValues();
            playerTxGainList.Sort();
            float nextGain = 0;

            if (playerTxGainList.Count > 0)
            {
                nextGain = playerTxGainList[playerTxGainList.Count - 1].Float;
            }
            urc.SetVoiceGain(playerId, protocolPriority, nextGain);
            if (nextGain > 0)
            {
                urc.EnableVoiceProtocol(playerId, protocolPriority);
            }
            else
            {
                urc.DisableVoiceProtocol(playerId, protocolPriority);
            }
        }

        private static DataDictionary GetChildrenFromDictionary(DataDictionary dict, int id)
        {
            if (dict.TryGetValue(id, TokenType.DataDictionary, out DataToken value))
            {
                return value.DataDictionary;
            }
            return new DataDictionary();
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

        #region event
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                IniializeAllPlayerVoice();
            }
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            int playerId = player.playerId;
            if (TxPlayerChannelGain.TryGetValue(playerId, TokenType.DataDictionary, out DataToken value))
            {
                DataList deleteChannel = value.DataDictionary.GetKeys();
                TxPlayerChannelGain.Remove(playerId);
                foreach (int channel in deleteChannel.ToArray())
                {
                    DataDictionary channelTxStatus = GetChildrenFromDictionary(TxChannelPlayerState, channel);
                    channelTxStatus.Remove(playerId);
                    TxChannelPlayerState[channel] = channelTxStatus;
                    // 受信機に、送信中の送信機の有無を通知する
                    CallbackRxTransmittingState(channel);
                }
            }
        }
        #endregion
    }
}