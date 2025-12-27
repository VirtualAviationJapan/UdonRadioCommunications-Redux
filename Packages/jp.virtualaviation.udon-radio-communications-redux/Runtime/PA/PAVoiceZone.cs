
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
        [SerializeField] string exitZoneName = "";
        [SerializeField] bool enableWhenEnter;
        [SerializeField] bool disableWhenExit;
        [SerializeField] bool enableWhenActive;
        bool isInit = false;

        public void OnPlayerJoined(VRCPlayerApi player){
            if(player.isLocal) isInit = true;
        }

        void OnEnable()
        {
            if(enableWhenActive && isInit) Activate();
        }
        void OnDisable()
        {
            if(enableWhenActive && zoneName == protocol.localZone && isInit) Deactivate();
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if(enableWhenEnter && player.isLocal) Activate();
        }
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if(disableWhenExit && player.isLocal) Deactivate();
        }
        public void Activate(){protocol.OnPlayerChangeZone(zoneName);}
        public void Deactivate(){protocol.OnPlayerChangeZone(exitZoneName);}

    }
}
