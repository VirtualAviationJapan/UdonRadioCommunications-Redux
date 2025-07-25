
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerTriggerEvent : UdonSharpBehaviour
    {
        [SerializeField] UdonSharpBehaviour target;
        [SerializeField] string eventOnPlayerEnter;
        [SerializeField] string eventOnPlayerExit;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal && (eventOnPlayerEnter != "")) target.SendCustomEvent(eventOnPlayerEnter);
        }
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player.isLocal && (eventOnPlayerExit != "")) target.SendCustomEvent(eventOnPlayerExit);
        }
    }
}