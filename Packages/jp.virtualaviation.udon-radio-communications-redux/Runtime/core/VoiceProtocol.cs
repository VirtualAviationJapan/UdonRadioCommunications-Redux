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

        public UdonRadioCommunication urc;
        [HideInInspector] public int protocolPriority = -1;

        // protected abstract float validateVoiceGain(bool status);
        public virtual void OnPlayerVoiceAdded(int playerId)
        {
        }

        protected virtual void validate() { }

        // public abstract void reset();

    }
}