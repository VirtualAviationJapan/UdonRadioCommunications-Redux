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
    public class VoiceSettingByZone : VoiceProtocol
    {
        private VoiceZone localZone = null;
        private DataDictionary ZoneByPlayer = new DataDictionary();

        public override void OnPlayerVoiceAdded(int playerId)
        {

        }

        #region API: zone registration
        public void OnPlayerEnterZone(VRCPlayerApi player, VoiceZone zone)
        {
            if (player.isLocal)
            {
                localZone = zone;
                ValidateAllPlayerVoice();
            }
            else
            {
                ZoneByPlayer[player.playerId] = zone;
                ValidateVoice(player.playerId, zone);
            }
        }

        public void OnPlayerExitZone(VRCPlayerApi player, VoiceZone zone)
        {
            if (player.isLocal)
            {
                if (localZone != zone) return;
                localZone = null;
                ValidateAllPlayerVoice();
                return;
            }

            VoiceZone prevZone = null;
            if (ZoneByPlayer.TryGetValue(player.playerId, TokenType.Reference, out DataToken zoneToken))
            {
                prevZone = (VoiceZone)zoneToken.Reference;
            }
            if (zone == prevZone)
            {
                ZoneByPlayer.Remove(player.playerId);
                urc.DisableVoiceProtocol(player.playerId, protocolPriority);
            }
        }

        #endregion

        private void ValidateAllPlayerVoice()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);

            foreach (VRCPlayerApi player in players)
            {
                if (player.isLocal || player == null) continue;
                VoiceZone targetPlayerZone = null;
                if (ZoneByPlayer.TryGetValue(player.playerId, TokenType.Reference, out DataToken zoneToken))
                {
                    targetPlayerZone = (VoiceZone)zoneToken.Reference;
                }
                ValidateVoice(player.playerId, targetPlayerZone);
            }
        }

        private void ValidateVoice(int playerId, VoiceZone zone)
        {
            if (localZone != null && zone == localZone) // ローカルプレイヤー・話し手双方が同じゾーン内にいる
            {
                SetPlayerVoice(playerId, zone.gainInZone, zone.nearInZone, zone.farInZone, zone.volumetricRadiusInZone, zone.lowpassInZone);
            }
            else if (zone != null) // 話し手はどこかのゾーンにいる。ローカルプレイヤーは話し手のいるゾーンの外にいる
            {
                SetPlayerVoice(playerId, zone.gainFromZone, zone.nearFromZone, zone.farFromZone, zone.volumetricRadiusFromZone, zone.lowpassFromZone);
            }
            else if (localZone != null) // ローカルプレイヤーはどこかのゾーンにいる。ただし、話し手はそのゾーンの外にいる
            {
                SetPlayerVoice(playerId, localZone.gainIntoZone, localZone.nearIntoZone, localZone.farIntoZone, localZone.volumetricRadiusIntoZone, localZone.lowpassIntoZone);
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
            urc.EnableVoiceProtocol(playerId, protocolPriority);
        }

        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                localZone = null;
            }
            else
            {
                ZoneByPlayer.Remove(player.playerId);
                urc.DisableVoiceProtocol(player.playerId, protocolPriority);
            }

        }
    }
}