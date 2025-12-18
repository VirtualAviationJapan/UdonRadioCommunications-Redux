
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PAVoiceZone : UdonSharpBehaviour
    {
        public PAVoiceSetting protocol;
        [SerializeField] string zoneName = "";
        [SerializeField] bool enableWhenEnter;
        [SerializeField] bool enableWhenActive;

        void OnEnable()
        {
            if(enableWhenActive) protocol.OnPlayerChangeZone(zoneName);
        }
        void OnDisable()
        {
            if(enableWhenActive && zoneName == protocol.localZone) protocol.OnPlayerChangeZone("");
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if(enableWhenEnter && player.isLocal) protocol.OnPlayerChangeZone(zoneName);
        }
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if(enableWhenEnter && player.isLocal) protocol.OnPlayerChangeZone("");
        }
    }
}
