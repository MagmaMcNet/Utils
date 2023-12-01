using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace MagmaMc.Utils
{
#if MAGMAMC_ADMINSYSTEM
public class EasyButton : AdminUtilRef
#else
    public class EasyButton: UdonSharpBehaviour
#endif
    {
        public GameObject[] EnableList;
        public GameObject[] DisableList;

        [UdonSynced] public bool Networked;
#if MAGMAMC_ADMINSYSTEM
    public bool AdminOnly;
#endif
        public bool MasterOnly;
        public bool DefaultValue;

        private bool CurrentValue;

        [HideInInspector, UdonSynced] public bool IntermediateValue;

#if MAGMAMC_ADMINSYSTEM
    public override void InitializedAwake()
#else
        public void Awake()
#endif
        {
            if (Networking.IsMaster)
                IntermediateValue = DefaultValue;
            CurrentValue = IntermediateValue;
        }

        public override void Interact()
        {
#if MAGMAMC_ADMINSYSTEM
        if (MasterOnly)
            AdminOnly = true;
        if (AdminOnly && !MasterOnly)
            if (!IsAdmin())
                return;
#endif
            if (MasterOnly)
#if MAGMAMC_ADMINSYSTEM
            if (!IsAdmin() || Networking.IsMaster)
                return;
#else
                if (Networking.IsMaster)
                    return;
#endif
            string Event = (CurrentValue ? nameof(DisableObjects) : nameof(EnableObjects));

            SendCustomEvent(Event);
            if (Networked)
                SendCustomNetworkEvent(NetworkEventTarget.All, Event);
        }

        public void EnableObjects()
        {
            if (Networked)
                IntermediateValue = true;
            CurrentValue = true;
            foreach (var obj in EnableList)
                obj.SetActive(true);
            foreach (var obj in DisableList)
                obj.SetActive(false);
        }
        public void DisableObjects()
        {
            if (Networked)
                IntermediateValue = false;
            CurrentValue = false;
            foreach (var obj in EnableList)
                obj.SetActive(false);
            foreach (var obj in DisableList)
                obj.SetActive(true);
        }

#if MAGMAMC_ADMINSYSTEM
    public bool IsAdmin() => _AdminUtil.AdminListContains(Networking.LocalPlayer.playerId);
#endif
    }
}