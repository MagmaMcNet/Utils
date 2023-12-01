using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace MagmaMc.Utils
{
    public class ReflectionProbeUpdater: UdonSharpBehaviour
    {
        public float scalingFactor = 1.0f;
        public float updateInterval = 1.0f / 30.0f;
        public float updateDistanceCutoff = 10.0f;

        private VRCPlayerApi localPlayer;
        private float lastUpdateTime;
        [SerializeField] private ReflectionProbe RP;
        private void Awake()
        {
            RP = GetComponent<ReflectionProbe>();
            if (RP != null)
                return;

            this.enabled = false;
            Debug.LogWarning("No Reflection Probe Found", this.gameObject);
        }

        private void Start() =>
            localPlayer = Networking.LocalPlayer;
        private void Update()
        {
            if (RP == null)
                return;
            float distance = Vector3.Distance(transform.position, localPlayer.GetPosition());

            if (distance > updateDistanceCutoff)
            {
                RP.enabled = false;
                return;
            }
            
            if (Time.time - lastUpdateTime >= updateInterval * scalingFactor * distance)
            {
                RP.enabled = true;
                RP.RenderProbe();
                lastUpdateTime = Time.time;
            }
        }
    }
}