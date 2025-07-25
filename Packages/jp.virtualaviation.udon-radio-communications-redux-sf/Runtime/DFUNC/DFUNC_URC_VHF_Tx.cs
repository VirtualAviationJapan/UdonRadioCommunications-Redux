
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonRadioCommunicationRedux.SaccFlight
{

    public enum URC_DFUNC_Tx_Button { Toggle, PTT };
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DFUNC_URC_VHF_Tx : UdonSharpBehaviour
    {
        public URC_DFUNC_Tx_Button buttonMode;
        public SFEXT_URC_VHF urc_vhf;
        public GameObject TxIndicator;
        public KeyCode desktopKey = KeyCode.P;
        private string triggerAxis = "Oculus_CrossPlatform_PrimaryIndexTrigger";

        private bool LeftDial = false;
        public void DFUNC_LeftDial()
        {
            LeftDial = true;
        }
        public void DFUNC_RightDial()
        {
            LeftDial = false;
        }

        public void SFEXT_L_EntityStart()
        {
            if (LeftDial == true)
            {
                triggerAxis = "Oculus_CrossPlatform_PrimaryIndexTrigger";
            }
            else
            {
                triggerAxis = "Oculus_CrossPlatform_SecondaryIndexTrigger";
            }
            gameObject.SetActive(false);
            if (TxIndicator) TxIndicator.SetActive(urc_vhf.TxPower);

        }
        public void SFEXT_O_PilotEnter()
        {
            if (!Networking.LocalPlayer.IsUserInVR()) DFUNC_Selected();
            urc_vhf.TxOff();
        }
        public void SFEXT_O_PilotExit() => DFUNC_Deselected();
        public void SFEXTP_O_UserEnter() => SFEXT_O_PilotEnter();
        public void SFEXTP_O_UserExit() => SFEXT_O_PilotExit();

        public void DFUNC_Selected()
        {
            gameObject.SetActive(true);
        }
        public void DFUNC_Deselected()
        {
            gameObject.SetActive(false);
        }
        public void OnStartTransmit()
        {
            if (TxIndicator) TxIndicator.SetActive(urc_vhf.TxPower);
        }
        public void OnStopTransmit()
        {
            if (TxIndicator) TxIndicator.SetActive(false);
        }

        private bool prevInput;
        private void Update()
        {
            var input = GetInput();
            if (buttonMode == URC_DFUNC_Tx_Button.Toggle)
            {
                if (input == true && prevInput == false) urc_vhf.ToggleTx();
            }
            else if (buttonMode == URC_DFUNC_Tx_Button.PTT)
            {
                if (input == true && prevInput == false) urc_vhf.TxOn();
                else if (input == false && prevInput == true) urc_vhf.TxOff();
            }
            prevInput = input;
        }

        private bool GetInput()
        {
            return Input.GetKey(desktopKey) || Input.GetAxisRaw(triggerAxis) > 0.75f;
        }
    }
}