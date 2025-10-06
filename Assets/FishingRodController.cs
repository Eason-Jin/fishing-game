using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("Feature Flags")]
    public bool useXRInput = false; // If false, use scroll wheel input
    [Header("GameObject")]
    public GameObject fishingRodPrefab;
    private GameObject rodInstance;
    private GameObject rodPivot;
    private float rodAngle = 350f; // Start at bottom
    [Header("Settings")]
    public float moveSpeed = 9.0f;
    public float maxAngle = 45.0f;
    private Vector3 spawnLocation = new Vector3(0, 0, 3);

    public float rodPosition;

    public FishingLine fishingLine;

    public Transform leftController;
    public Transform rightController;

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
        float low = 350f - maxAngle;
        float high = 350f;

        if (useXRInput && leftController != null && rightController != null)
        {
            float heightInput = ((leftController.position.y + rightController.position.y) / 2f);
            float lowRange = PlayerPrefs.GetFloat("MinHeight");
            float highRange = PlayerPrefs.GetFloat("MaxHeight");
            rodAngle = ScaleRange(heightInput, highRange, lowRange, low, high);
            if (!float.IsNaN(rodAngle) && !float.IsInfinity(rodAngle) && rodPivot != null)
            {
                rodPivot.transform.localEulerAngles = new Vector3(rodAngle, rodPivot.transform.localEulerAngles.y, rodPivot.transform.localEulerAngles.z);
                rodPosition = 350f - rodAngle;
            }
        }
        else
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel") * 2;
            if (scrollInput != 0 && rodPivot != null)
            {
                rodAngle += scrollInput * moveSpeed;
                rodAngle = Mathf.Clamp(rodAngle, low, high);
                if (!float.IsNaN(rodAngle) && !float.IsInfinity(rodAngle))
                {
                    rodPivot.transform.localEulerAngles = new Vector3(rodAngle, rodPivot.transform.localEulerAngles.y, rodPivot.transform.localEulerAngles.z);
                    rodPosition = 350f - rodAngle;
                }
            }
        }
    }
    public GameObject GetFishingRodInstance()
    {
        return rodInstance;
    }

    private float ScaleRange(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }
}
