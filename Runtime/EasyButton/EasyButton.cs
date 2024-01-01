using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

using VRC.Udon;
#if MAGMAMC_PERMISSIONMANAGER
using PermissionSystem;
#endif
namespace MagmaMc.Utils
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
#if MAGMAMC_PERMISSIONMANAGER
    public class EasyButton : PermissionManagerRef
#else
    public class EasyButton: UdonSharpBehaviour
#endif
    {
        public string InteractText;

        
        public bool ObjectsList = true;
        public bool Events = false;

        [Tooltip("GameObject List That Will Get Enabled While The Button Is On")]
        public GameObject[] EnableList;
        [Tooltip("GameObject List That Will Get Enabled While The Button Is Off")]
        public GameObject[] DisableList;

        public string[] OnEnabledEvents;
        public string[] OnDisabledEvents;
        public UdonBehaviour[] OnEnabledReceiver;
        public UdonBehaviour[] OnDisabledReceiver;



        public Material EnabledMat;
        public Material DisableMat;

        public bool Networked;
        public bool SyncLateJoiners = true;
#if MAGMAMC_PERMISSIONMANAGER
        public string[] AuthorizedPermissions;
#endif
        public bool MasterOnly;
        public bool DefaultValue;

        private bool CurrentValue;


        private ushort Depth = 0;
        private bool LateJoining = false;
        private bool PauseAction = false;
        [RecursiveMethod]
        public void WaitForSync()
        {
            if (!Networked)
                return;

            if (!Networking.IsNetworkSettled)
            {
                Depth++;
                if (Depth > 250)
                {
                    Debug.LogError($"Attempted To Sync EasyButton '{Depth}' Times, Failed On GameObject '{gameObject.name}' ", gameObject);
                    return;
                }
                SendCustomEventDelayedSeconds(nameof(WaitForSync), 0.5f);
                return;
            }
            if (Networking.IsMaster)
                ToggleAction(DefaultValue);
            else
            {
                if (!SyncLateJoiners)
                    return;
                LateJoining = true;
                RequestSync();
            }
        }
#if MAGMAMC_PERMISSIONMANAGER
        public override void OnReady()
#else
        public void Start()
#endif
        {
            SendCustomEventDelayedSeconds(nameof(WaitForSync), 0.5f);
            InteractionText = gameObject.name;
        }

        public override void Interact()
        {
#if MAGMAMC_PERMISSIONMANAGER
        if (!HasPermissions(AuthorizedPermissions) || (Networking.IsMaster && MasterOnly))
            return;

        if (MasterOnly)
            if (!Networking.IsMaster)
                return;
#endif
            string Event = (CurrentValue ? nameof(DisableObjects) : nameof(EnableObjects));

            SendCustomEvent(Event);
            if (Networked)
                SendCustomNetworkEvent(NetworkEventTarget.All, Event);
        }

        public void EnableObjects() => ToggleAction(true);
        public void DisableObjects() => ToggleAction(false);

        public void ToggleAction(bool Action)
        {
            if (PauseAction)
            {
                PauseAction = false;
                return;
            }

            GetComponent<MeshRenderer>().material = Action ? EnabledMat : DisableMat;
            CurrentValue = Action;
            if (ObjectsList)
            {
                foreach (var obj in EnableList)
                    obj.SetActive(Action);
                foreach (var obj in DisableList)
                    obj.SetActive(!Action);
            }
            if (Events)
            {
                if (Action)
                    for(int i = 0; i < OnEnabledEvents.Length; i++)
                        OnEnabledReceiver[i].SendCustomEvent(OnEnabledEvents[i]);
                else
                    for (int i = 0; i < OnEnabledEvents.Length; i++)
                        OnEnabledReceiver[i].SendCustomEvent(OnDisabledEvents[i]);
            }
        }


        public void PauseSynced()
        {
            if (!LateJoining)
                PauseAction = true;
            else
                LateJoining = false;
        }


        public void RequestSync() =>
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(SyncClients));

        public void SyncClients()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PauseSynced));
            if (CurrentValue)
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EnableObjects));
            else
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DisableObjects));
        }

    }
}