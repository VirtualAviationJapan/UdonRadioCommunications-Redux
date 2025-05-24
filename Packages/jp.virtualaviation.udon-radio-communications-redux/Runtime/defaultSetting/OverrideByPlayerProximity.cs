
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonRadioCommunicationRedux
{
    /// <summary>
    /// プレイヤー同士が近接している場合、設定をデフォルト値に上書きする
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class OverrideByPlayerProximity : VoiceProtocol
    {
        public DefaultSetting defaultSetting;
        private VRCPlayerApi localPlayer;

        public override void OnPlayerJoined(VRCPlayerApi player)

        {
            if (player.isLocal) localPlayer = player;
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal == true)
            {
                var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
                VRCPlayerApi.GetPlayers(players);
                foreach (var p in players)
                {
                    urc.DisableVoiceProtocol(p.playerId, protocolPriority);
                }
            }
            else
            {
                urc.DisableVoiceProtocol(player.playerId, protocolPriority);
            }
        }


        public override void OnPlayerVoiceAdded(int playerId)
        {
            defaultSetting.SetDefaultSetting(playerId, protocolPriority);
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal) return;
            urc.EnableVoiceProtocol(player.playerId, protocolPriority);
        }
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player.isLocal) return;
            urc.DisableVoiceProtocol(player.playerId, protocolPriority);
        }
        void Update()
        {
            if (Utilities.IsValid(localPlayer) == true)
            {
                // 常にプレイヤーに追従させる
                VRCPlayerApi.TrackingData origin = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin);
                transform.position = origin.position;
            }
        }
    }
}
