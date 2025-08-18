using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    // Unity UI has higher priority for these fields
    public GameObject fishingRodPrefab;
    public float moveSpeed = 3f;
    public float maxRange = 3f;
    public Vector3 spawnLocation = new Vector3(0, 0, 0);

    public float rodPosition;

    private void Start()
    {
        if (fishingRodPrefab == null)
        {
            Debug.LogError("Fishing rod prefab is not assigned.");
            return;
        }
        GameObject rodInstance = Instantiate(fishingRodPrefab, spawnLocation, Quaternion.identity);
        rodInstance.transform.SetParent(transform);
        rodPosition = rodInstance.transform.position.z;
    }

    private void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            Vector3 newPosition = transform.position + transform.forward * scrollInput * moveSpeed;
            newPosition.z = Mathf.Clamp(newPosition.z, 0, spawnLocation.z + maxRange);
            rodPosition = newPosition.z;
            transform.position = newPosition;
        }
    }
}
