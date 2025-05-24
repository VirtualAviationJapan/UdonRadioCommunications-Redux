using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;
using UdonRadioCommunicationRedux;
using SaccFlightAndVehicles;

namespace UdonRadioCommunicationRedux.SaccFlight
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DFUNC_URC_VHF_Rx : UdonSharpBehaviour
    {
        [SerializeField] SFEXT_URC_VHF urc_vhf;
        [Header("表示系設定")]
        public GameObject RxIndicator;
        public GameObject RxRecervingIndicator;
        public TextMeshPro frequencyText;
        public string frequencyTextFormat = "000.00";

        [Header("操作系設定")]
        [Header("無線On/Offを受け付けるか否か")]
        public bool canChangeRx = true;
        [Header("着席と同時に無線をOnにするか否か")]
        public bool changeRxWhenEnter = false;
        [Header("キーアサイン")]
        public KeyCode desktopRxKey = KeyCode.L;
        public string desktopFrequencyAxis = "Mouse Wheel";
        public KeyCode desktopDecrementKey = KeyCode.None;//  KeyCode.Comma;
        public KeyCode desktopIncrementKey = KeyCode.None;// KeyCode.Period;
        public float controllerSensitivity = 100;

        private AudioSource switchFunctionSound;
        private Transform controlsRoot;


        #region DFUNC Left/Right Recognize
        // DFUNC初期化時に、左右どちらかをEntityから伝達する箇所
        private string triggerAxis = "Oculus_CrossPlatform_PrimaryIndexTrigger";
        private VRCPlayerApi.TrackingDataType trackingTarget;

        public void DFUNC_LeftDial()
        {
            triggerAxis = "Oculus_CrossPlatform_PrimaryIndexTrigger";
            trackingTarget = VRCPlayerApi.TrackingDataType.LeftHand;
        }
        public void DFUNC_RightDial()
        {
            triggerAxis = "Oculus_CrossPlatform_SecondaryIndexTrigger";
            trackingTarget = VRCPlayerApi.TrackingDataType.RightHand;
        }
        #endregion

        #region Saccflight Event
        public void SFEXT_L_EntityStart()
        {
            var entity = GetComponentInParent<SaccEntity>();
            switchFunctionSound = entity.SwitchFunctionSound;
            controlsRoot = entity.transform;

            if (RxIndicator) RxIndicator.SetActive(urc_vhf.RxPower);
            if (RxRecervingIndicator) RxRecervingIndicator.SetActive(false);

            gameObject.SetActive(false);
        }

        public void SFEXTP_L_EntityStart() => SFEXT_L_EntityStart();

        public void SFEXT_O_PilotEnter()
        {
            if (!Networking.LocalPlayer.IsUserInVR()) DFUNC_Selected();
            if (canChangeRx == false || changeRxWhenEnter == true) urc_vhf.RxOn();
        }

        public void SFEXT_O_PilotExit()
        {
            urc_vhf.RxOff();
            DFUNC_Deselected();
        }
        public void SFEXTP_O_UserEnter() => SFEXT_O_PilotEnter();
        public void SFEXTP_O_UserExit() => SFEXT_O_PilotExit();

        public void DFUNC_Selected()
        {
            prevTriggered = false;
            gameObject.SetActive(true);
        }

        public void DFUNC_Deselected()
        {
            gameObject.SetActive(false);
        }

        #endregion

        private float inputOrigin;
        private bool prevTriggered;
        private int channelTriggerStarted;

        #region Input Update
        private void LateUpdate()
        {
            // デスクトップ操作
            if (Input.GetKeyDown(desktopIncrementKey)) urc_vhf.IncreaseFrequency();
            if (Input.GetKeyDown(desktopDecrementKey)) urc_vhf.DecreaseFrequency();

            if (Input.GetKeyDown(desktopRxKey))
            {
                channelTriggerStarted = urc_vhf.Channel;
            }
            if (Input.GetKeyUp(desktopRxKey) && canChangeRx == true)
            {
                if (channelTriggerStarted == urc_vhf.Channel) urc_vhf.ToggleRx();
            }
            if (Input.GetKey(desktopRxKey))
            {
                // マウスホイールによる周波数変更
                int frqChangeInput = Mathf.RoundToInt(Input.GetAxisRaw(desktopFrequencyAxis));
                if (frqChangeInput != 0) urc_vhf.StepFrequency(frqChangeInput);
            }


            // VR操作
            bool trigger = Input.GetAxisRaw(triggerAxis) > 0.75f;
            if (trigger == true)
            {
                var inputPos = controlsRoot.InverseTransformPoint(Networking.LocalPlayer.GetTrackingData(trackingTarget).position).z;
                if (prevTriggered == false)
                {
                    inputOrigin = inputPos;
                    channelTriggerStarted = urc_vhf.Channel;
                }
                else
                {
                    int diff = Mathf.RoundToInt((inputPos - inputOrigin) * controllerSensitivity);
                    if (diff != 0)
                    {
                        urc_vhf.StepFrequency(diff);
                        inputOrigin = inputPos;
                    }
                }
            }
            else if (canChangeRx == true && prevTriggered == true && urc_vhf.Channel == channelTriggerStarted)
            {
                urc_vhf.ToggleRx();
            }
            prevTriggered = trigger;
        }
        #endregion

        #region Tx/Rx Management
        public void OnStartReceive()
        {
            if (RxIndicator) RxIndicator.SetActive(true);
            if (switchFunctionSound) switchFunctionSound.Play();
        }
        public void OnStopReceive()
        {
            if (RxIndicator) RxIndicator.SetActive(false);
            if (switchFunctionSound) switchFunctionSound.Play();
        }
        public void OnStartTransmit()
        {
            if (switchFunctionSound) switchFunctionSound.Play();
        }
        public void OnStopTransmit()
        {
            if (switchFunctionSound) switchFunctionSound.Play();
        }
        public void ChannelTransmitting()
        {
            if (RxRecervingIndicator) RxRecervingIndicator.SetActive(true);
        }
        public void ChannelNotTransmitting()
        {
            if (RxRecervingIndicator) RxRecervingIndicator.SetActive(false);
        }

        #endregion

        #region Frequency Management

        public void OnUpdateChannel()
        {
            float freqencyInVisual = urc_vhf.Channel * 0.001f;
            if (frequencyText != null) frequencyText.text = freqencyInVisual.ToString(frequencyTextFormat);
            if (switchFunctionSound) switchFunctionSound.Play();
        }
        #endregion
    }
}