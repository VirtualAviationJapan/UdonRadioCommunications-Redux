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

        public void SetDefaultSetting(int playerId, int priority)
        {
            urc.SetVoiceGain(playerId, priority, gain);
            urc.SetVoiceNear(playerId, priority, near);
            urc.SetVoiceFar(playerId, priority, far);
            urc.SetVoiceVolumetricRadius(playerId, priority, volumetricRadius);
            urc.SetVoiceLowpass(playerId, priority, lowpass);
        }

        public override void OnPlayerVoiceAdded(int playerId)
        {
            SetDefaultSetting(playerId, protocolPriority);
            urc.EnableVoiceProtocol(playerId, protocolPriority);
        }

    }
}