
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PlayerMod : UdonSharpBehaviour
{
    [SerializeField, Tooltip("Player Walking Speed")] float WalkSpeed = 4;
    [SerializeField, Tooltip("Player Running Speed")] float RunSpeed = 6;
    [SerializeField, Tooltip("Player Strafe Speed")] float StrafeSpeed = 3;
    [SerializeField, Tooltip("Player Jumping Strength")] float JumpStrength = 6;
    [SerializeField, Tooltip("Player Gravity Strength")] float GravityStrength = 2;
    /// <summary>
    /// Set LocalPlayer Movement Settings To The World Default Settings
    /// </summary>
    public void Start()
    {
        if (Utilities.IsValid(Networking.LocalPlayer))
        {
            Networking.LocalPlayer.SetWalkSpeed(WalkSpeed);
            Networking.LocalPlayer.SetRunSpeed(RunSpeed);
            Networking.LocalPlayer.SetStrafeSpeed(StrafeSpeed);
            Networking.LocalPlayer.SetJumpImpulse(JumpStrength);
            Networking.LocalPlayer.SetGravityStrength(GravityStrength);
        }
        Destroy(this);
    }
}
