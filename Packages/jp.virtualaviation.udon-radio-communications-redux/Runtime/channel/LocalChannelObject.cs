
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace UdonRadioCommunicationRedux
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LocalChannelObject : UdonSharpBehaviour
    {
        public VoiceBroadcastByChannel channelManager;
        [SerializeField] private int channel;
        [SerializeField] private bool registerOnStart = true;


        #region API
        public virtual void Register()
        {
            channelManager.AddLocalObject(channel, gameObject);
            gameObject.SetActive(false);
        }
        public virtual void Unregister()
        {
            channelManager.RemoveLocalObject(channel, gameObject);
            gameObject.SetActive(false);
        }
        #endregion


        void Start()
        {
            if (registerOnStart) Register();
        }
        void OnDestroy()
        {
            Unregister();
        }
    }
}