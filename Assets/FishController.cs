using UnityEngine;

public class FishController : MonoBehaviour
{
    private bool isFishCaught = false;
    private float flyOutTimer = 0f;
    private float flyOutDuration = 1f; // seconds
    private int flyOutDirection = 0; // -1 for left, 1 for right

    public PlotController plotController;
    public GameObject fishPrefab;
    public int startingFishDepth = -100;

    private DotStatus dotStatus;
    private float score = 0;
    private int fishDepth;

    private GameObject fishInstance;

    private Vector3 fishSpawnLocation = new Vector3(0, -3, 12);
    private float radius = 3f;           // size of the circle
    private float speed = 1f;            // how fast it moves
    private float squiggleAmplitude = 2f; // how wavy it is
    private float timeCounter = 0f;

    private FishingLine fishingLine;

    private DepthIndicatorController depthIndicator;
    private ScoreTimeController scoreTimeController;
    private float weight;

    private bool isPaused = false;
    private float pauseTimer = 0f;

    void Start()
    {
        if (plotController != null)
        {
            dotStatus = plotController.dotStatus;
        }
        fishDepth = startingFishDepth;

        depthIndicator = FindObjectOfType<DepthIndicatorController>();
        scoreTimeController = FindObjectOfType<ScoreTimeController>();

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
        weight = float.Parse(WeightController.Instance.GetWeight());
        while (true)
        {
            Debug.Log("Fish Depth: " + fishDepth + "; Score: " + score);
            dotStatus = plotController.dotStatus;
            if (!isPaused)
            {
                if (dotStatus == DotStatus.OnTheLine)
                {
                    score += 5 * weight;
                    fishDepth += 5;
                }
                else if (dotStatus == DotStatus.CloseEnough)
                {
                    score += 3 * weight;
                    fishDepth += 3;
                }
                else if (dotStatus == DotStatus.NotOnTheLine)
                {
                    fishDepth -= 1;
                }

                if (fishDepth >= 0)
                {
                    score += 100 * weight;
                    isFishCaught = true;
                    flyOutTimer = 0f;
                    Debug.Log("Fish caught! Score: " + score);
                }

                if (fishDepth < (startingFishDepth - 20))
                {
                    Debug.Log("Game Over!");
                }
            }

            if (depthIndicator != null)
            {
                depthIndicator.UpdateProgress(fishDepth);
            }
            else
            {
                Debug.LogWarning("DepthIndicatorController is not assigned.");
            }

            if (scoreTimeController != null)
            {
                scoreTimeController.UpdateScore(score);
                scoreTimeController.UpdateTime(Mathf.RoundToInt(Time.time));
            }
            else
            {
                Debug.LogWarning("ScoreTimeController is not assigned.");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Update()
    {
        if (isPaused)
        {
            // Pause the graph
            if (plotController != null)
                plotController.isPaused = true;

            pauseTimer += Time.deltaTime;
            if (pauseTimer < flyOutDuration)
            {
                // Fish flying out of water animation
                Vector3 flyOutPos = fishInstance.transform.position;
                flyOutPos.y += 14f * Time.deltaTime; // Move up quickly
                flyOutPos.x += flyOutDirection * 10f * Time.deltaTime; // Move sideways (right or left)
                flyOutPos.z -= 15f * Time.deltaTime; // Move towards player (-z direction)
                fishInstance.transform.position = flyOutPos;
            }
            else
            {
                // Reset only fish position and depth, NOT score
                isPaused = false;
                pauseTimer = 0f;
                fishDepth = startingFishDepth;
                fishInstance.transform.position = fishSpawnLocation;
                fishInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                // Resume the graph
                if (plotController != null)
                    plotController.isPaused = false;
            }
            return; // Skip normal update while paused
        }

        if (fishInstance != null)
        {
            if (fishingLine == null)
            {
                fishingLine = FindObjectOfType<FishingLine>();
                if (fishingLine != null)
                {
                    fishingLine.SetFishTransform(fishInstance.transform);
                }
            }

            if (!isFishCaught)
            {
                Vector3 previousPosition = fishInstance.transform.position;
                timeCounter += Time.deltaTime * speed;
                float x = fishSpawnLocation.x + Mathf.Cos(timeCounter) * radius;
                float z = fishSpawnLocation.z + Mathf.Sin(timeCounter) * radius;
                float offset = Mathf.Sin(timeCounter * 3f) * squiggleAmplitude;
                float y = (fishDepth * 0.1f) - 2.5f;
                Vector3 newPosition = new Vector3(x + offset, y, z + offset);
                fishInstance.transform.position = newPosition;

                Vector3 direction = (newPosition - previousPosition).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                    fishInstance.transform.rotation = Quaternion.Slerp(fishInstance.transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
            else
            {
                // Start pause and animation
                isPaused = true;
                pauseTimer = 0f;
                isFishCaught = false;
                // Randomly choose fly out direction: -1 (left) or 1 (right)
                flyOutDirection = (Random.value < 0.5f) ? -1 : 1;
            }
        }
    }
    public GameObject GetFishInstance()
    {
        return fishInstance;
    }
}
