using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject fishingRodPrefab;
    private GameObject rodInstance;
    private GameObject rodPivot;
    private float rodAngle = 350f; // Start at bottom
    [Header("Settings")]
    public float moveSpeed = 9.0f;
    public float maxAngle = 45.0f;
    private Vector3 spawnLocation = new Vector3(0, 0, 5);

    public float rodPosition;

    public FishingLine fishingLine;

    private void Start()
    {
        if (fishingRodPrefab == null)
        {
            Debug.LogError("Fishing rod prefab is not assigned.");
            return;
        }
        // Create a pivot GameObject at the desired spawn location
        rodPivot = new GameObject("RodPivot");
        rodPivot.transform.position = spawnLocation;
        rodPivot.transform.SetParent(transform); // Keep hierarchy
        // Instantiate the rod as a child of the pivot
        rodInstance = Instantiate(fishingRodPrefab, Vector3.zero, Quaternion.identity, rodPivot.transform);
        rodInstance.transform.localPosition = Vector3.zero;
        rodInstance.transform.localRotation = Quaternion.identity;
        rodAngle = 350f; // Start at bottom
        rodPivot.transform.localEulerAngles = new Vector3(rodAngle, rodPivot.transform.localEulerAngles.y, rodPivot.transform.localEulerAngles.z);
        rodPosition = 0;
    }

    private void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel") * 2;

        if (scrollInput != 0 && rodPivot != null)
        {
            // Update rodAngle based on input
            rodAngle += scrollInput * moveSpeed;
            rodAngle = Mathf.Clamp(rodAngle, 350f - maxAngle, 350f);
            rodPivot.transform.localEulerAngles = new Vector3(rodAngle, rodPivot.transform.localEulerAngles.y, rodPivot.transform.localEulerAngles.z);
            rodPosition = 350f - rodAngle;
        }
    }
    public GameObject GetFishingRodInstance()
    {
        return rodInstance;
    }
}
