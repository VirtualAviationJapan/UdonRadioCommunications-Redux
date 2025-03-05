using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using Utilities = VRC.SDKBase.Utilities;

namespace UdonRadioCommunicationRedux
{
    public class UdonRadioCommunication : UdonSharpBehaviour
    {
        public VoiceProtocol[] voiceProtocols;

        void Start()
        {
            for (int i = 0; i < voiceProtocols.Length; i++)
            {
                voiceProtocols[i].voicePriority = i;
            }
        }

    }
}