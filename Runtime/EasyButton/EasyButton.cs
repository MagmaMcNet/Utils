#define _MAGMAMC_ADMINSYSTEM
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
#if MAGMAMC_ADMINSYSTEM
using MagmaMc.AdminUtil;
#endif

namespace MagmaMc.Utils
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
#if MAGMAMC_ADMINSYSTEM
public class EasyButton : AdminUtilRef
#else
    public class EasyButton: UdonSharpBehaviour
#endif
    {
        [Tooltip("GameObject List That Will Get Enabled While The Button Is On")]
        public GameObject[] EnableList;
        [Tooltip("GameObject List That Will Get Enabled While The Button Is Off")]
        public GameObject[] DisableList;

        public Material EnabledMat;
        public Material DisableMat;

        public bool Networked;
#if MAGMAMC_ADMINSYSTEM
        public bool AdminOnly;
#endif
        public bool MasterOnly;
        public bool DefaultValue;

        private bool CurrentValue;


        private ushort Depth = 0;

        [RecursiveMethod]
        public void WaitForSync()
        {
            if (Networking.IsMaster)
            {
                ToggleAction(DefaultValue);
                return;
            }

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
            RequestSync();
        }

        public void Start() =>
            SendCustomEventDelayedSeconds(nameof(WaitForSync), 0.5f);

        public override void Interact()
        {
#if MAGMAMC_ADMINSYSTEM
        if (MasterOnly)
            AdminOnly = true;
        if (AdminOnly && !MasterOnly)
            if (!_AdminUtil.IsAdmin(Networking.LocalPlayer))
                return;
#endif
            if (MasterOnly)
#if MAGMAMC_ADMINSYSTEM
            if (!_AdminUtil.IsAdmin(Networking.LocalPlayer) && !Networking.IsMaster)
                return;
#else
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
            GetComponent<MeshRenderer>().material = Action ? EnabledMat : DisableMat;
            CurrentValue = Action;
            foreach (var obj in EnableList)
                obj.SetActive(Action);
            foreach (var obj in DisableList)
                obj.SetActive(!Action);
        }


        public void RequestSync() =>
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(SyncClients));

        public void SyncClients()
        {
            if (CurrentValue)
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EnableObjects));
            else
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DisableObjects));
        }

    }
}