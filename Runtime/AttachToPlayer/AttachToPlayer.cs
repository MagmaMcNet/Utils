using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering;
using VRC.SDKBase;
using VRC.Udon;


namespace MagmaMc.Utils
{
    public class AttachToPlayer: UdonSharpBehaviour
    {
        public HumanBodyBones Bone;
        public Vector3 VecOffset;
        public Vector3 RotOffset;
        public float ScaleAdd;
        public float Scale;

        public VRCPlayerApi LocalPlayer;

        public string DisplayName;
        public void Start()
        {

            VRCPlayerApi[] Players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(Players);

            foreach (VRCPlayerApi Player in Players)
            {
                if (Player.displayName.ToLower() == DisplayName.ToLower())
                    LocalPlayer = Player;
            }
            if (LocalPlayer == null)
            {
                enabled = false;
                transform.position = Vector3.zero;
            }
        }

        public void Update()
        {
            if (LocalPlayer == null) return;
            Vector3 bonepos = LocalPlayer.GetBonePosition(Bone);
            Quaternion bonerot = LocalPlayer.GetBoneRotation(Bone);
            float _S = ScaleAdd + (LocalPlayer.GetAvatarEyeHeightAsMeters() * Scale);
            transform.position = bonepos + (VecOffset * _S);
            transform.rotation = bonerot * Quaternion.Euler(RotOffset + new Vector3(90, 0, 0));
            transform.localScale = new Vector3(_S, _S, _S);
        }

        public override void OnPlayerJoined(VRCPlayerApi Player)
        {
            if (Player.displayName == DisplayName)
            {
                LocalPlayer = Player;
                enabled = true;
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi Player)
        {
            if (Player.displayName == DisplayName)
            {
                LocalPlayer = null;
                enabled = false;
                transform.position = Vector3.zero;
            }
        }
    }
}