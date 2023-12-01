using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRCStation = VRC.SDK3.Components.VRCStation;

public class LockableSeat : UdonSharpBehaviour
{
    public bool Locked = false;
    [HideInInspector] public VRCStation station;

    public override void Interact() =>
        Networking.LocalPlayer.UseAttachedStation();


    public bool Toggle() =>
        Locked = !Locked;

    /// <summary>
    /// Invoke To Unlock The Seat
    /// </summary>
    /// <returns></returns>
    public bool Unlock() =>
        Locked = false;

    /// <summary>
    /// Invoke To lock The Seat
    /// </summary>
    /// <returns></returns>
    public bool Lock() =>
        Locked = true;


    public void Awake() =>
        station = GetComponent<VRCStation>();

    public void FixedUpdate()
    {
        station.disableStationExit = Locked;

    }

}
