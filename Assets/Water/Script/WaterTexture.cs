// Assets/Water/Scripts/WaterTexture.cs
using UnityEngine;

[ExecuteAlways]                        // run in Editor so you can see the texture without Play
[RequireComponent(typeof(Renderer))]
public class WaterTexture : MonoBehaviour
{
    [Header("Texture Size")]
    public int width = 1024;
    public int height = 1024;

    [Header("Colors")]
    // Base #6C8FC0, Wave #355A8F by default
    public Color baseColor = new Color(0.424f, 0.561f, 0.753f, 1f);
    public Color waveColor = new Color(0.208f, 0.333f, 0.561f, 1f);

    [Header("Stripe Pattern")]
    [Tooltip("How many dark bands vertically")]
    public float stripeCount = 6f;
    [Range(0.02f, 0.6f)]
    public float stripeThickness = 0.25f;
    [Tooltip("Curvature of bands in pixels")]
    public float bendAmplitudePx = 10f;
    [Tooltip("How many bends across width")]
    public float bendFrequency = 2f;

    [Header("Second subtle layer")]
    public bool addSecondLayer = true;
    public float secondLayerOffset = 0.3f;

    private Texture2D _tex;

    void OnEnable() { Generate(); }   // run when enabled or values change
    void OnValidate() { Generate(); }
    void Start() { Generate(); }   // also refresh when entering Play

    public void Generate()
    {
        if (width < 8 || height < 8) return;

        _tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        _tex.wrapMode = TextureWrapMode.Repeat;
        _tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < height; y++)
        {
            float v = (float)y / height; // 0..1
            for (int x = 0; x < width; x++)
            {
                float u = (float)x / width; // 0..1

                // Horizontal bend so bands curve slightly
                float bend = Mathf.Sin(2f * Mathf.PI * (u * bendFrequency)) * (bendAmplitudePx / height);

                // Primary band
                float s1 = 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * (v * stripeCount + bend));
                float t = SmoothBand(s1, 0.5f, stripeThickness);

                // Optional second band for richness
                if (addSecondLayer)
                {
                    float s2 = 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * (v * (stripeCount * 0.9f) + bend + secondLayerOffset));
                    t = Mathf.Clamp01(t + 0.5f * SmoothBand(s2, 0.5f, stripeThickness * 0.8f));
                }

                _tex.SetPixel(x, y, Color.Lerp(baseColor, waveColor, t));
            }
        }
        _tex.Apply();

        // ✅ Key change: avoid edit-mode material instantiation/leak
        var rend = GetComponent<Renderer>();
        var mat = Application.isPlaying ? rend.material : rend.sharedMaterial;

        // Built-in Unlit/Texture uses _MainTex
        if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", _tex);
        // Future-proof if the project ever switches to URP
        if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", _tex);
    }

    // Soft band function (0..1) around 'center' with 'thickness'
    private float SmoothBand(float value, float center, float thickness)
    {
        float half = thickness * 0.5f;
        float a = Mathf.InverseLerp(center - half, center, value);
        float b = 1f - Mathf.InverseLerp(center, center + half, value);
        return Mathf.Clamp01(Mathf.Min(a, b));
    }
}
