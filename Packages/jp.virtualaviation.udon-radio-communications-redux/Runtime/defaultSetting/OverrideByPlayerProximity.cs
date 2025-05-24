
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
        private float applyingDistance = 5;
        private SphereCollider sc;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isLocal) { localPlayer = player; }
        }
        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player.isLocal == true)
            {
                var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
                VRCPlayerApi.GetPlayers(players);
                foreach (var p in players)
                {
                    if (CheckProximity(p))
                        urc.EnableVoiceProtocol(p.playerId, protocolPriority);
                    else
                        urc.DisableVoiceProtocol(p.playerId, protocolPriority);
                }
            }
            else
            {
                if (CheckProximity(player))
                    urc.EnableVoiceProtocol(player.playerId, protocolPriority);
                else
                    urc.DisableVoiceProtocol(player.playerId, protocolPriority);
            }
        }


        public override void OnPlayerVoiceAdded(int playerId)
        {
            defaultSetting.SetDefaultSetting(playerId, protocolPriority);
            // 近くにプレイヤーがいる場合、有効化
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerId);
            if (CheckProximity(player))
            {
                urc.EnableVoiceProtocol(playerId, protocolPriority);
            }
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

        void Start()
        {
            sc = (SphereCollider)gameObject.GetComponent<SphereCollider>();
            if (sc) applyingDistance = sc.radius;
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

        /// <summary>
        ///  特定のプレイヤーと自プレイヤーの距離を判定し、近接するならtrueを返す
        /// </summary>
        /// <param name="player">距測対象のプレイヤー</param>
        /// <returns></returns>
        private bool CheckProximity(VRCPlayerApi player)
        {
            if (Utilities.IsValid(player))
            {
                VRCPlayerApi.TrackingData origin = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin);
                float distance = Vector3.Distance(origin.position, transform.position);
                if (distance <= applyingDistance) return true;
            }
            return false;
        }
    }
}
