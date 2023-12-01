using UdonSharp;
using UnityEngine;
namespace MagmaMc.Utils
{
    public class RotateEmissionHue: UdonSharpBehaviour
    {
        public float rotationSpeed = 1.0f;

        private Renderer _renderer;
        private Material _sharedMaterial;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _sharedMaterial = _renderer.sharedMaterial;
        }

        private void Update()
        {
            Color Emission = _sharedMaterial.GetColor("_EmissionColor");
            Color.RGBToHSV(Emission, out float hue, out float saturation, out float value);

            hue = (hue + (rotationSpeed / 10) * Time.deltaTime) % 1.0f;

            _sharedMaterial.SetColor("_EmissionColor", Color.HSVToRGB(hue, saturation, value));
            _sharedMaterial.EnableKeyword("_EMISSION");
        }
    }
}