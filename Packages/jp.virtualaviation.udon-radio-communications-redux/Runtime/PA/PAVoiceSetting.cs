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
    public class PAVoiceSetting : VoiceProtocol
    {
        [System.NonSerialized]public string localZone = "";
        private DataDictionary PlayerPAAssign = new DataDictionary();

        public override void OnPlayerVoiceAdded(int playerId){ }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal) localZone = "";
            ValidateAll();
        }

        #region API: PA registration
        public void OnPlayerTakePA(int playerId, PAVoiceActivator pa)
        {
            PlayerPAAssign[playerId] = pa.zoneName;
            SetPlayerVoice(playerId, pa.gain, pa.near, pa.far, pa.volumetricRadius, pa.lowpass);
            ValidateAll();
        }
        public void OnPlayerReleasePA(int playerId, PAVoiceActivator pa)
        {
            PlayerPAAssign[playerId] = "";
            ValidateAll();
        }
        #endregion
        #region API: zone registration
        public void OnPlayerChangeZone(string zoneName)
        {
            localZone = zoneName;
            ValidateAll();
        }
        #endregion

        private void ValidateAll()
        {
            DataList keys = PlayerPAAssign.GetKeys();
            for (int i = 0; i < keys.Count; i++)
            {
                DataToken pid = keys[i];
                ValidateVoice(pid.Int, PlayerPAAssign[pid].String);
            }
        }

        private void ValidateVoice(int playerId, string zoneName)
        {
            if(playerId < 0) return;
            if (zoneName != "" && zoneName == localZone) // ローカルプレイヤー・話し手双方が同じゾーン内にいる
            {
                // SetPlayerVoice(playerId, zone.gainInZone, zone.nearInZone, zone.farInZone, zone.volumetricRadiusInZone, zone.lowpassInZone);
                urc.EnableVoiceProtocol(playerId, protocolPriority);
            }
            else // 話し手・ローカルプレイヤーともにゾーン外
            {
                urc.DisableVoiceProtocol(playerId, protocolPriority);
            }

        }

        private void SetPlayerVoice(int playerId, float gain, float near, float far, float volumetricRadius, bool lowpass)
        {
            urc.SetVoiceGain(playerId, protocolPriority, gain);
            urc.SetVoiceNear(playerId, protocolPriority, near);
            urc.SetVoiceFar(playerId, protocolPriority, far);
            urc.SetVoiceVolumetricRadius(playerId, protocolPriority, volumetricRadius);
            urc.SetVoiceLowpass(playerId, protocolPriority, lowpass);
            // urc.EnableVoiceProtocol(playerId, protocolPriority);
        }

    }
}