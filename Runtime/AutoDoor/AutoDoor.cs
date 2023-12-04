using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace MagmaMc.Utils
{
    public class AutoDoor: UdonSharpBehaviour
    {
        public float activationDistance = 2.0f;
        public float openAngle = 90.0f;
        public AnimationCurve rotationCurve; // Set this in the Inspector
        public float animationDuration = 1.0f; // Set the duration of the animation (in seconds)

        private Quaternion initialRotation;
        private bool isOpen = false;
        private float animationStartTime;
        public Vector3 RotationAngle = Vector3.up;
        public Transform LocationPos;
        private int Frame = 0;
        private int WaitFrames = 5;

        private void Start()
        {
            initialRotation = transform.rotation;
            if (LocationPos == null)
                LocationPos = transform;
        }

        private void Update()
        {
            if (Frame >= WaitFrames)
            {
                Frame = 0;
                VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
                VRCPlayerApi.GetPlayers(players);
                float closestDistance = float.MaxValue;

                foreach (VRCPlayerApi player in players)
                {
                    float distance = Vector3.Distance(player.GetPosition(), LocationPos.position);

                    if (distance < closestDistance)
                        closestDistance = distance;
                }
                if (closestDistance < activationDistance)
                {

                    if (!isOpen)
                        OpenDoor(true);
                }
                else if (isOpen)
                    OpenDoor(false);
            } 
            else
                Frame++;
        }

        private void OpenDoor(bool open)
        {
            if (open)
            {
                isOpen = true;
                animationStartTime = Time.time;
            }
            else
            {
                isOpen = false;
                animationStartTime = Time.time;
            }
        }

        private void FixedUpdate()
        {
            if (isOpen || !isOpen) // This is just to force the doorTransform rotation update
            {
                float t = Mathf.Clamp01((Time.time - animationStartTime) / animationDuration);
                float curveValue = rotationCurve.Evaluate(t);

                if (isOpen)
                    transform.rotation = Quaternion.Slerp(initialRotation, initialRotation * Quaternion.Euler(RotationAngle * openAngle), curveValue);
                else
                    transform.rotation = Quaternion.Slerp(initialRotation * Quaternion.Euler(RotationAngle * openAngle), initialRotation, curveValue);
            }
        }
    }
}