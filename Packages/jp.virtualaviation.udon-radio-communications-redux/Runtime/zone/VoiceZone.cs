
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VoiceZone : UdonSharpBehaviour
    {
        public VoiceSettingByZone protocol;

        [Header("話し手・聞き手が同じゾーン内にいるとき")]
        public float gainInZone = 15;
        public float nearInZone = 0;
        public float farInZone = 25;
        public float volumetricRadiusInZone = 0;
        public bool lowpassInZone = true;

        [Header("話し手がゾーン内・聞き手がゾーン外にいるとき")]
        public float gainFromZone = 0;
        public float nearFromZone = 0;
        public float farFromZone = 0;
        public float volumetricRadiusFromZone = 0;
        public bool lowpassFromZone = true;

        [Header("話し手がゾーン外・聞き手がゾーン内にいるとき")]
        [Header("ただし、話し手が別のどこかのゾーンにいる場合、そのゾーンの設定が優先される")]
        // ※ 上記は、防音したいゾーンの外に声が漏れ出るのを防ぐため
        public float gainIntoZone = 0;
        public float nearIntoZone = 0;
        public float farIntoZone = 0;
        public float volumetricRadiusIntoZone = 0;
        public bool lowpassIntoZone = true;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            protocol.OnPlayerEnterZone(player, this);
        }
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            protocol.OnPlayerExitZone(player, this);
        }

    }
}