using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    [DefaultExecutionOrder(-1000)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonRadioCommunication : UdonSharpBehaviour
    {
        public VoiceProtocol[] voiceProtocols;
        private DataDictionary playerVoices = new DataDictionary();

        #region PlayerVoice Management
        public void RegisterPlayerVoice(int playerId, PlayerVoice pv)
        {
            playerVoices[playerId] = pv;
            for (int i = 0; i < voiceProtocols.Length; i++)
            {
                voiceProtocols[i].OnPlayerVoiceAdded(playerId);
            }
        }
        public void UnregisterPlayerVoice(int playerId)
        {
            DataToken pidToken = playerId;
            playerVoices.Remove(pidToken);
        }
        public void ValidatePlayerVoices()
        {
            DataList registeredPlayerIdList = playerVoices.GetKeys();
            for (int i = 0; i < registeredPlayerIdList.Count; i++)
            {
                DataToken pidToken = registeredPlayerIdList[i];
                DataToken pvToken = playerVoices[pidToken];
                if (!Utilities.IsValid(pvToken.Reference)) playerVoices.Remove(pidToken);
            }
        }
        #endregion

        #region Voice Setting
        public void SetVoiceGain(int playerId, int priority, float gain)
        {
            PlayerVoice pv = (PlayerVoice)playerVoices[playerId].Reference;
            pv.SetVoiceGain(priority, gain);
        }
        public void SetVoiceNear(int playerId, int priority, float near)
        {
            PlayerVoice pv = (PlayerVoice)playerVoices[playerId].Reference;
            pv.SetVoiceNear(priority, near);
        }
        public void SetVoiceFar(int playerId, int priority, float far)
        {
            PlayerVoice pv = (PlayerVoice)playerVoices[playerId].Reference;
            pv.SetVoiceFar(priority, far);
        }
        public void SetVoiceVolumetricRadius(int playerId, int priority, float radius)
        {
            PlayerVoice pv = (PlayerVoice)playerVoices[playerId].Reference;
            pv.SetVoiceFar(priority, radius);
        }
        public void SetVoiceLowpass(int playerId, int priority, bool enabled)
        {
            PlayerVoice pv = (PlayerVoice)playerVoices[playerId].Reference;
            pv.SetVoiceLowpass(priority, enabled);
        }
        public void EnableVoiceProtocol(int playerId, int priority)
        {
            PlayerVoice pv = (PlayerVoice)playerVoices[playerId].Reference;
            pv.EnableVoiceProtocol(priority);
        }
        public void DisableVoiceProtocol(int playerId, int priority)
        {
            PlayerVoice pv = (PlayerVoice)playerVoices[playerId].Reference;
            pv.DisableVoiceProtocol(priority);
        }
        #endregion

        #region Lifecycle
        void Start()
        {
            for (int i = 0; i < voiceProtocols.Length; i++)
            {
                voiceProtocols[i].protocolPriority = i;
            }
        }
        #endregion

    }
}