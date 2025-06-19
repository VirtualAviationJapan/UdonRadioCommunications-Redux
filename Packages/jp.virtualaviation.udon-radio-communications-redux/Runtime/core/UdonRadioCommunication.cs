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
        #endregion

        #region Voice Setting
        public void SetVoiceGain(int playerId, int priority, float gain)
        {
            if (playerVoices.TryGetValue(playerId, TokenType.Reference, out DataToken value))
            {
                PlayerVoice pv = (PlayerVoice)value.Reference;
                pv.SetVoiceGain(priority, gain);
            }
        }
        public void SetVoiceNear(int playerId, int priority, float near)
        {
            if (playerVoices.TryGetValue(playerId, TokenType.Reference, out DataToken value))
            {
                PlayerVoice pv = (PlayerVoice)value.Reference;
                pv.SetVoiceNear(priority, near);
            }
        }
        public void SetVoiceFar(int playerId, int priority, float far)
        {
            if (playerVoices.TryGetValue(playerId, TokenType.Reference, out DataToken value))
            {
                PlayerVoice pv = (PlayerVoice)value.Reference;
                pv.SetVoiceFar(priority, far);
            }
        }
        public void SetVoiceVolumetricRadius(int playerId, int priority, float radius)
        {
            if (playerVoices.TryGetValue(playerId, TokenType.Reference, out DataToken value))
            {
                PlayerVoice pv = (PlayerVoice)value.Reference;
                pv.SetVoiceVolumetricRadius(priority, radius);
            }
        }
        public void SetVoiceLowpass(int playerId, int priority, bool enabled)
        {
            if (playerVoices.TryGetValue(playerId, TokenType.Reference, out DataToken value))
            {
                PlayerVoice pv = (PlayerVoice)value.Reference;
                pv.SetVoiceLowpass(priority, enabled);
            }
        }
        public void EnableVoiceProtocol(int playerId, int priority)
        {
            if (playerVoices.TryGetValue(playerId, TokenType.Reference, out DataToken value))
            {
                PlayerVoice pv = (PlayerVoice)value.Reference;
                pv.EnableVoiceProtocol(priority);
            }
        }
        public void DisableVoiceProtocol(int playerId, int priority)
        {
            if (playerVoices.TryGetValue(playerId, TokenType.Reference, out DataToken value))
            {
                PlayerVoice pv = (PlayerVoice)value.Reference;
                pv.DisableVoiceProtocol(priority);
            }
        }
        #endregion

        #region Lifecycle
        void Start()
        {
            for (int i = 0; i < voiceProtocols.Length; i++)
            {
                if (voiceProtocols[i]) voiceProtocols[i].protocolPriority = i;
            }
        }
        #endregion

    }
}