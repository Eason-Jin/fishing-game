using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class UVScroller : MonoBehaviour
{
    public Vector2 speed = new Vector2(0.02f, 0.01f); // UV units per second
    private Renderer _rend;
    private Vector2 _offset;

    void Awake() { _rend = GetComponent<Renderer>(); }

    void Update()
    {
        if (!Application.isPlaying) return;
        _offset += speed * Time.deltaTime;

        var m = _rend.material; // play-mode instance is fine
        if (m.HasProperty("_MainTex")) m.SetTextureOffset("_MainTex", _offset);
        if (m.HasProperty("_BumpMap")) m.SetTextureOffset("_BumpMap", _offset);
    }
}
