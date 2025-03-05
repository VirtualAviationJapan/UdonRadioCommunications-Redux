using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    public abstract class VoiceProtocol : UdonSharpBehaviour
    {

        public UdonRadioCommunication manager;
        [HideInInspector] public int voicePriority = -1;

        protected virtual void startShiftVoice(VRCPlayerApi player, float gain) { }
        protected virtual void stopShiftVoice(VRCPlayerApi player) { }

        // protected abstract float validateVoiceGain(bool status);
        public virtual void OnPlayerJoined(VRCPlayerApi player)
        {
            validate();
        }

        protected virtual void validate() { }

    }
}