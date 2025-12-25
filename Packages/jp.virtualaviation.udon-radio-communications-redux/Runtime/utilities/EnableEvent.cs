
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class EnableEvent : UdonSharpBehaviour
    {
        [SerializeField] UdonSharpBehaviour target;
        [SerializeField] string eventOnEnable;
        [SerializeField] string eventOnDisable;

        void OnEnable()
        {
            target.SendCustomEvent(eventOnEnable);
        }
        void OnPlayerTriggerExit()
        {
            target.SendCustomEvent(eventOnDisable);
        }
    }
}