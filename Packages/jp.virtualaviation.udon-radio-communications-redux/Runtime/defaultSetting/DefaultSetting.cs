using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DefaultSetting : VoiceProtocol
    {
        [SerializeField] private float gain = 15;
        [SerializeField] private float near = 0;
        [SerializeField] private float far = 25;
        [SerializeField] private float volumetricRadius = 0;
        [SerializeField] private bool lowpass = true;
        // protected abstract float validateVoiceGain(bool status);
        public override void OnPlayerVoiceAdded(int playerId)
        {
            urc.SetVoiceGain(playerId, protocolPriority, gain);
            urc.SetVoiceNear(playerId, protocolPriority, near);
            urc.SetVoiceFar(playerId, protocolPriority, far);
            urc.SetVoiceVolumetricRadius(playerId, protocolPriority, volumetricRadius);
            urc.SetVoiceLowpass(playerId, protocolPriority, lowpass);

            urc.EnableVoiceProtocol(playerId, protocolPriority);
        }


    }
}