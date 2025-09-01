using UnityEngine;

public class FishController : MonoBehaviour
{
    public PlotController plotController;
    public GameObject fishPrefab;
    public int startingFishDepth = -100;

    private DotStatus dotStatus;
    private int score = 0;
    private int fishDepth;

    private GameObject fishInstance;

    private Vector3 fishSpawnLocation = new Vector3(0, -3, 12);
    private float radius = 3f;           // size of the circle
    private float speed = 1f;            // how fast it moves
    private float squiggleAmplitude = 2f; // how wavy it is
    private float timeCounter = 0f;

    private FishingLine fishingLine;

    private DepthIndicatorController depthIndicator;

    void Start()
    {
        if (plotController != null)
        {
            dotStatus = plotController.dotStatus;
        }
        fishDepth = startingFishDepth;

        depthIndicator = FindObjectOfType<DepthIndicatorController>();

        StartCoroutine(UpdateFishState());

        if (fishPrefab == null)
        {
            Debug.LogError("Fish prefab is not assigned.");
            //return;
        }

        // spawn temporary fish instance and position
        fishInstance = Instantiate(fishPrefab, fishSpawnLocation, Quaternion.identity);
        fishInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // add the fishing line component
        FishingRodController rodController = FindObjectOfType<FishingRodController>();
        if (rodController != null)
        {
            GameObject lineObj = new GameObject("FishingLine");
            fishingLine = lineObj.AddComponent<FishingLine>();
            fishingLine.SetRodTransform(rodController.GetFishingRodInstance().transform);
            fishingLine.SetFishTransform(fishInstance.transform);
        }
    }

    private System.Collections.IEnumerator UpdateFishState()
    {
        while (true)
        {
            Debug.Log("Fish Depth: " + fishDepth + "; Score: " + score);
            dotStatus = plotController.dotStatus;
            if (dotStatus == DotStatus.OnTheLine)
            {
                score += 5;
                fishDepth += 5;
            }
            else if (dotStatus == DotStatus.CloseEnough)
            {
                score += 3;
                fishDepth += 3;
            }
            else if (dotStatus == DotStatus.NotOnTheLine)
            {
                fishDepth -= 1;
            }

            if (fishDepth == 0)
            {
                score += 100;
                fishDepth = startingFishDepth;
                Debug.Log("Fish caught! Score: " + score);
            }

            if (fishDepth < (startingFishDepth - 20))
            {
                Debug.Log("Game Over!");
            }

            if (depthIndicator != null)
            {
                depthIndicator.UpdateProgress(fishDepth);
            } else {
                Debug.LogWarning("DepthIndicatorController is not assigned.");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Update()
    {
        if (fishInstance != null)
        {

            if (fishingLine == null)
            {
                Debug.Log("Finding FishingLine component in FishController...");
                fishingLine = FindObjectOfType<FishingLine>();
                if (fishingLine != null)
                {
                    fishingLine.SetFishTransform(fishInstance.transform);
                    Debug.Log("FishingLine component found and fish transform set.");
                }
            }

            Vector3 previousPosition = fishInstance.transform.position;

            timeCounter += Time.deltaTime * speed;

            // Circular path + squiggle
            float x = fishSpawnLocation.x + Mathf.Cos(timeCounter) * radius;
            float z = fishSpawnLocation.z + Mathf.Sin(timeCounter) * radius;
            float offset = Mathf.Sin(timeCounter * 3f) * squiggleAmplitude; // squiggle in X-Z plane

            Vector3 newPosition = new Vector3(x + offset, fishSpawnLocation.y, z + offset);
            fishInstance.transform.position = newPosition;

            // Rotate the fish to face its direction of movement
            Vector3 direction = (newPosition - previousPosition).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                fishInstance.transform.rotation = Quaternion.Slerp(fishInstance.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
    public GameObject GetFishInstance()
    {
        return fishInstance;
    }
}
