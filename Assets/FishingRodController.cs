using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject fishingRodPrefab;
    [Header("Settings")]
    public float moveSpeed = 9.0f;
    public float maxAngle = 45.0f;
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
        transform.localEulerAngles = new Vector3(350, 0, 0);
        rodInstance.transform.SetParent(transform);
        rodPosition = 0;
    }

    private void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            float currentAngle = transform.localEulerAngles.x;
            float newAngle = currentAngle + scrollInput * moveSpeed;
            rodPosition = 350 - newAngle;
            rodPosition = Mathf.Clamp(rodPosition, 0, maxAngle);
            newAngle = Mathf.Clamp(newAngle, 350 - maxAngle, 350);
            transform.localEulerAngles = new Vector3(newAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }
}
