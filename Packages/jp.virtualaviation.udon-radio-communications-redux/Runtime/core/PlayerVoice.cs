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

        [SerializeField] private int currentSelectedProtocol = 0;
        [SerializeField] private bool[] enableProtocol;
        [SerializeField] private float[] protocolGainSetting;
        [SerializeField] private float[] protocolNearSetting;
        [SerializeField] private float[] protocolFarSetting;
        [SerializeField] private float[] protocolVolumetricRadiusSetting;
        [SerializeField] private bool[] protocolLowpassSetting;

        private VRCPlayerApi owner;
        [SerializeField] int ownerPlayerId;


        #region Voice Setting
        public void SetVoiceGain(int protocolPriority, float gain)
        {
            protocolGainSetting[protocolPriority] = gain;
            if (currentSelectedProtocol == protocolPriority) owner.SetVoiceGain(gain);
        }
        public void SetVoiceNear(int protocolPriority, float near)
        {
            protocolNearSetting[protocolPriority] = near;
            if (currentSelectedProtocol == protocolPriority) owner.SetVoiceDistanceNear(near);
        }
        public void SetVoiceFar(int protocolPriority, float far)
        {
            protocolFarSetting[protocolPriority] = far;
            if (currentSelectedProtocol == protocolPriority) owner.SetVoiceDistanceFar(far);
        }
        public void SetVoiceVolumetricRadius(int protocolPriority, float radius)
        {
            protocolVolumetricRadiusSetting[protocolPriority] = radius;
            if (currentSelectedProtocol == protocolPriority) owner.SetVoiceVolumetricRadius(radius);
        }
        public void SetVoiceLowpass(int protocolPriority, bool enabled)
        {
            protocolLowpassSetting[protocolPriority] = enabled;
            if (currentSelectedProtocol == protocolPriority) owner.SetVoiceLowpass(enabled);
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
            if (currentSelectedProtocol > protocolPriority) return;
            for (int i = currentSelectedProtocol - 1; i >= 0; i--)
            {
                if (enableProtocol[i] == true)
                {
                    currentSelectedProtocol = i;
                    EnableVoiceProtocol(i);
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

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            // 誰かが入場したとき、音声設定を再度解決する
            for (int i = enableProtocol.Length - 1; i >= 0; i--)
            {
                if (enableProtocol[i] == true)
                {
                    EnableVoiceProtocol(i);
                    return;
                }
            }
        }
        #endregion
    }
}