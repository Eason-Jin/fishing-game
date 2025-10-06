using UnityEngine;

public class FishingLine : MonoBehaviour
{
    private Transform rodTransform;
    private Transform fishTransform;

    private LineRenderer lineRenderer;
    private Vector3 tipLocalOffset = new Vector3(0, 0, 4.5f);

    // Offset for where the line attaches to the fish (in local space)
    public Vector3 fishLocalOffset = Vector3.zero;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
    if (rodTransform == null || fishTransform == null) return;

    Vector3 tipPos = rodTransform.position
             + rodTransform.forward * tipLocalOffset.z
             + rodTransform.up * tipLocalOffset.y
             + rodTransform.right * tipLocalOffset.x;

    // Offset the fish end of the line
    Vector3 fishPos = fishTransform.TransformPoint(fishLocalOffset);

    lineRenderer.SetPosition(0, tipPos);
    lineRenderer.SetPosition(1, fishPos);
    }

    public void SetRodTransform(Transform rod)
    {
        rodTransform = rod;
    }

    public void SetFishTransform(Transform fish)
    {
        fishTransform = fish;
    }
}
