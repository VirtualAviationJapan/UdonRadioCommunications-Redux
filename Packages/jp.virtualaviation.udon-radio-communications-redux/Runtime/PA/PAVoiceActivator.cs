
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
        [UdonSynced, FieldChangeCallback(nameof(UsingPlayerId))] private int usingPlayerId = -1;
        public int UsingPlayerId
        {
            set 
            { 
                OnChanged(usingPlayerId, value);
                usingPlayerId = value;
                foreach(var i in onEnalbeObject) i.SetActive(usingPlayerId >= 0);
            }
            get => usingPlayerId;
        }
        [SerializeField]GameObject[] onEnalbeObject;


        [Header("聞き手が指定した識別子のゾーン内にいるとき")]
        public string zoneName;
        public float gain = 20;
        public float near = 99999;
        public float far = 100000;
        public float volumetricRadius = 9999;
        public bool lowpass = true;

        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if(UsingPlayerId == Networking.LocalPlayer.playerId) Deactivate();
        }


        public void Activate()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            UsingPlayerId = Networking.LocalPlayer.playerId;
            RequestSerialization();
        }
        public void Deactivate()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            UsingPlayerId = -1;
            RequestSerialization();
        }
        public void Toggle()
        {
            if(usingPlayerId == Networking.LocalPlayer.playerId) Deactivate();
            else Activate();
        }

        private void OnChanged(int prevPID, int nextPID)
        {
            if(prevPID >= 0) protocol.OnPlayerReleasePA(prevPID, this);
            if(nextPID >= 0) protocol.OnPlayerTakePA(nextPID, this);
        }

    }
}