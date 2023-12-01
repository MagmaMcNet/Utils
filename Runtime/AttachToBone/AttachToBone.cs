using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
namespace MagmaMc.Utils
{
    public class AttachToBone: UdonSharpBehaviour
    {
        public HumanBodyBones Bone;
        public Vector3 VecOffset;
        public Vector3 RotOffset;
        public float ScaleAdd;
        public float Scale;

        public VRCPlayerApi LocalPlayer;

        public void Start() =>
            LocalPlayer = Networking.LocalPlayer;

        public void Update()
        {
            Vector3 bonepos = LocalPlayer.GetBonePosition(Bone);
            Quaternion bonerot = LocalPlayer.GetBoneRotation(Bone);
            float _S = ScaleAdd + (LocalPlayer.GetAvatarEyeHeightAsMeters() * Scale);
            transform.position = bonepos + (VecOffset * _S);
            transform.rotation = bonerot * Quaternion.Euler(RotOffset + new Vector3(90, 0, 0));
            transform.localScale = new Vector3(_S, _S, _S);
        }
    }
}