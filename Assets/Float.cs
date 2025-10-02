using UnityEngine;

public class Float : MonoBehaviour
{
    public Transform[] objectsToFloat;  // Objects in this group
    private float moveRange = 0.5f;      // How far they drift
    private float moveSpeed = 0.01f;        // How fast they drift

    private Vector3[] startPositions;
    private float noiseOffset;

    void Start()
    {
        // Store each object's starting position
        startPositions = new Vector3[objectsToFloat.Length];
        for (int i = 0; i < objectsToFloat.Length; i++)
        {
            startPositions[i] = objectsToFloat[i].position;
        }

        // Randomize motion so different controllers don't move identically
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float offsetX = Mathf.PerlinNoise(Time.time * moveSpeed, noiseOffset) - 0.5f;
        float offsetY = Mathf.PerlinNoise(noiseOffset, Time.time * moveSpeed) - 0.5f;

        Vector3 offset = new Vector3(offsetX, offsetY, 0f) * moveRange;

        // Apply same offset to every object in this controller�s group
        for (int i = 0; i < objectsToFloat.Length; i++)
        {
            objectsToFloat[i].position = startPositions[i] + offset;
        }
    }
}
