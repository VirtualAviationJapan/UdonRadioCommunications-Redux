using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    [RequireComponent(typeof(VRCPlayerObject))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerVoice : UdonSharpBehaviour
    {
        public UdonRadioCommunication urc;

        private int currentSelectedProtocol = 0;
        private bool[] enableProtocol;
        private float[] protocolGainSetting;
        private float[] protocolNearSetting;
        private float[] protocolFarSetting;
        private float[] protocolVolumetricRadiusSetting;
        private bool[] protocolLowpassSetting;

        private VRCPlayerApi owner;
        private int ownerPlayerId;


        #region Voice Setting
        public void SetVoiceGain(int protocolPriority, float gain)
        {
            protocolGainSetting[protocolPriority] = gain;
        }
        public void SetVoiceNear(int protocolPriority, float near)
        {
            protocolNearSetting[protocolPriority] = near;
        }
        public void SetVoiceFar(int protocolPriority, float far)
        {
            protocolFarSetting[protocolPriority] = far;
        }
        public void SetVoiceVolumetricRadius(int protocolPriority, float radius)
        {
            protocolVolumetricRadiusSetting[protocolPriority] = radius;
        }
        public void SetVoiceLowpass(int protocolPriority, bool enabled)
        {
            protocolLowpassSetting[protocolPriority] = enabled;
        }

        public void EnableVoiceProtocol(int protocolPriority)
        {
            enableProtocol[protocolPriority] = true;
            if (currentSelectedProtocol <= protocolPriority)
            {
                currentSelectedProtocol = protocolPriority;
                owner.SetVoiceGain(protocolGainSetting[protocolPriority]);
                owner.SetVoiceDistanceNear(protocolNearSetting[protocolPriority]);
                owner.SetVoiceDistanceFar(protocolFarSetting[protocolPriority]);
                owner.SetVoiceVolumetricRadius(protocolVolumetricRadiusSetting[protocolPriority]);
                owner.SetVoiceLowpass(protocolLowpassSetting[protocolPriority]);
            }
        }

        public void DisableVoiceProtocol(int protocolPriority)
        {
            enableProtocol[protocolPriority] = false;
            if (currentSelectedProtocol <= protocolPriority)
            {
                for (int i = protocolPriority - 1; i >= 0; i--)
                {
                    if (enableProtocol[i] == true) EnableVoiceProtocol(i);
                }
            }
        }

        #endregion

        #region Lifecycle
        void Start()
        {
            enableProtocol = new bool[urc.voiceProtocols.Length];
            protocolGainSetting = new float[urc.voiceProtocols.Length];
            protocolNearSetting = new float[urc.voiceProtocols.Length];
            protocolFarSetting = new float[urc.voiceProtocols.Length];
            protocolVolumetricRadiusSetting = new float[urc.voiceProtocols.Length];
            protocolLowpassSetting = new bool[urc.voiceProtocols.Length];

            owner = Networking.GetOwner(this.gameObject);
            ownerPlayerId = owner.playerId;
            urc.RegisterPlayerVoice(ownerPlayerId, this);
        }

        void OnDestroy()
        {
            urc.UnregisterPlayerVoice(ownerPlayerId);
        }
        #endregion
    }
}