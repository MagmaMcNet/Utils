
using System;
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;
using VRC.Udon;
using VRC.SDKBase;
namespace MagmaMc.Utils
{
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EventButton))]
public class EventButtonEditor: Editor
{
    private void OnSceneGUI()
    {
        EventButton vrcButton = (EventButton)target;

        Color handleColor = vrcButton.Enabled ? Color.green : Color.red;
        Handles.color = handleColor;
        vrcButton.gameObject.GetComponent<MeshRenderer>().material.color = handleColor;
    }
}
#endif



    public class EventButton: UdonSharpBehaviour
    {
        public UdonBehaviour ReceiverUdon;

        public string EnabledEvent;
        public string DisabledEvent;

        public bool Networked;
        public bool MasterOnly;
        public NetworkEventTarget NetworkTargets;

        public bool Enabled = false;

        public Color EnabledColor = new Color(0f, 1f, 0f, 1.0f);
        public Color DisabledColor = new Color(1f, 0f, 0f, 1.0f);

        public void Start()
        {
            bool _E = Enabled;
            UpdateButton();
            Enabled = _E;
        }
        public void FixedUpdate()
        {
            if (Enabled)
                GetComponent<MeshRenderer>().material.color = EnabledColor;
            else
                GetComponent<MeshRenderer>().material.color = DisabledColor;
        }
        public override void Interact()
        {
            Enabled = !Enabled;
            UpdateButton();
        }

        public void Enable() =>
            Enabled = true;
        public void Disable() =>
            Enabled = false;

        public void UpdateButton()
        {
            string Event = (Enabled ? EnabledEvent : DisabledEvent);

            if (MasterOnly)
                if (!Networking.LocalPlayer.isMaster)
                {
                    Enabled = !Enabled;
                    return;
                }

            ReceiverUdon.SendCustomEvent(Event);

            if (Networked)
            {
                ReceiverUdon.SendCustomNetworkEvent(NetworkTargets, Event);
                if (Enabled)
                    SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Enable));
                else
                    SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Disable));
            }

        }
    }
}