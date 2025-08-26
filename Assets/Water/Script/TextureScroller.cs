using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureScroller : MonoBehaviour
{
    [Tooltip("X = sideways scroll, Y = vertical scroll")]
    public Vector2 speed = new Vector2(0.015f, 0f);

    private Renderer _rend;
    private Vector2 _offset;

    void Awake() { _rend = GetComponent<Renderer>(); }

    void Update()
    {
        // Only animate during Play to avoid edit-mode warnings
        if (!Application.isPlaying) return;

        _offset += speed * Time.deltaTime;

        var m = _rend.material; // Play mode: instance is fine
        if (m.HasProperty("_MainTex")) m.SetTextureOffset("_MainTex", _offset);
        if (m.HasProperty("_BaseMap")) m.SetTextureOffset("_BaseMap", _offset);
    }
}
