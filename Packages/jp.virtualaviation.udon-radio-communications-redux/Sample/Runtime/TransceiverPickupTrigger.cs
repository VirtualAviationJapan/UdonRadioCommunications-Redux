using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;
using Utilities = VRC.SDKBase.Utilities;
using UdonRadioCommunicationRedux;

namespace UdonRadioCommunicationRedux.Sample
{
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TransceiverPickupTrigger : UdonSharpBehaviour
    {
        public HandyVHFTransceiver transceiver;
        public override void OnPickupUseDown()
        {
            if (transceiver) transceiver.TxOn();
        }
        public override void OnPickupUseUp()
        {
            if (transceiver) transceiver.TxOff();
        }
    }
}