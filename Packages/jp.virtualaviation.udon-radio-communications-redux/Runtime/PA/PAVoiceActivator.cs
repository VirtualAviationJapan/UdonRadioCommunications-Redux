
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PAVoiceActivator : UdonSharpBehaviour
    {
        public PAVoiceSetting protocol;
        [SerializeField] bool activateWhenEnable;

        [UdonSynced, FieldChangeCallback(nameof(UsingPlayerId))] private int usingPlayerId = -1;
        public int UsingPlayerId
        {
            set 
            { 
                OnChanged(usingPlayerId, value);
                usingPlayerId = value;
            }
            get => usingPlayerId;
        }


        [Header("聞き手が指定した識別子のゾーン内にいるとき")]
        public string zoneName;
        public float gain = 20;
        public float near = 99999;
        public float far = 100000;
        public float volumetricRadius = 9999;
        public bool lowpass = true;

        void OnEnable()
        {
            if(activateWhenEnable) Activate();
        }
        void OnDisable()
        {
            if(activateWhenEnable) Deactivate();
        }

        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if(UsingPlayerId == Networking.LocalPlayer.playerId) Deactivate();
        }


        public void Activate()
        {
            UsingPlayerId = Networking.LocalPlayer.playerId;
            RequestSerialization();
        }
        public void Deactivate()
        {
            UsingPlayerId = -1;
            RequestSerialization();
        }
        private void OnChanged(int prevPID, int nextPID)
        {
            if(prevPID >= 0) protocol.OnPlayerReleasePA(prevPID, this);
            if(nextPID >= 0) protocol.OnPlayerTakePA(nextPID, this);
        }

    }
}