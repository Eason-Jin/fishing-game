using UnityEngine;

public class FishController : MonoBehaviour
{
    private bool isFishCaught = false;
    private float flyOutDuration = 0.8f; // seconds
    private int flyOutDirection = 0; // -1 for left, 1 for right

    public PlotController plotController;
    public GameObject fishPrefab;
    public int startingFishDepth = -100;

    private DotStatus dotStatus;
    private int fishDepth;

    private GameObject fishInstance;

    private Vector3 fishSpawnLocation = new Vector3(0, -3, 20);
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

    public float score = 0;
    public int fishCaughtCount = 0;

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

        // spawn fish instance and position
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

        weight = float.Parse(SettingsController.Instance.GetWeight());

        // fish models in fishSet
        Transform smallFish = fishInstance.transform.Find("Fish3");
        Transform medFish = fishInstance.transform.Find("Fish1");
        Transform largeFish = fishInstance.transform.Find("Shark");

        // set fish models 
        smallFish.gameObject.SetActive(false);
        medFish.gameObject.SetActive(false);
        largeFish.gameObject.SetActive(false);

        Vector3 fishLineOffset = Vector3.zero;
        if (weight > 2 && weight <= 8)
        {
            medFish.gameObject.SetActive(true);
            // Set offset for medium fish (adjust as needed)
            fishLineOffset = new Vector3(0, 0, 1.2f);
        }
        else if (weight > 8)
        {
            largeFish.gameObject.SetActive(true);
            // Set offset for large fish (adjust as needed)
            fishLineOffset = new Vector3(0, 0, 5.0f);
        }
        else
        {
            smallFish.gameObject.SetActive(true);
            // Set offset for small fish (adjust as needed)
            fishLineOffset = new Vector3(0, 0, 0.5f);
        }

        // Set the fishing line to attach to the offset
        if (fishingLine != null)
        {
            fishingLine.SetFishTransform(fishInstance.transform);
            fishingLine.fishLocalOffset = fishLineOffset;
        }
    }

    private System.Collections.IEnumerator UpdateFishState()
    {
        while (!plotController.isPaused && !plotController.isFinished)
        {
            dotStatus = plotController.dotStatus;

            float rodPosition = plotController.fishingRodController.rodPosition;
            float rodMax = plotController.fishingRodController.maxAngle;

            if (dotStatus == DotStatus.OnTheLine)
            {
                score += 5 * weight;
                fishDepth += 10;
            }
            else if (dotStatus == DotStatus.CloseEnough)
            {
                score += 3 * weight;
                fishDepth += 5;
            }
            else if (dotStatus == DotStatus.NotOnTheLine)
            {
                fishDepth -= 5;
            }

            if (fishDepth >= 0 && dotStatus == DotStatus.OnTheLine && rodPosition >= 38f)
            {
                score += 100 * weight;
                isFishCaught = true;
                fishCaughtCount++;
            }

            if (fishDepth < (startingFishDepth - 20))
            {
                plotController.isFinished = true;
                Debug.Log("Game Over!");
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
                scoreTimeController.UpdateFishCaught(fishCaughtCount);
            }
            else
            {
                Debug.LogWarning("ScoreTimeController is not assigned.");
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void Update()
    {
        if (fishInstance == null)
            return;

        if (fishingLine == null)
        {
            fishingLine = FindObjectOfType<FishingLine>();
            if (fishingLine != null)
            {
                fishingLine.SetFishTransform(fishInstance.transform);
            }
        }

        if (isPaused)
        {
            UpdateCaughtAnimation();
            return;
        }

        if (isFishCaught)
        {
            StartCaughtAnimation();
            return;
        }

        UpdateFishMovement();

    }

    private void UpdateCaughtAnimation()
    {
        pauseTimer += Time.deltaTime;
        if (pauseTimer < flyOutDuration)
        {
            // Fish flying out of water animation
            Vector3 prevPos = fishInstance.transform.position;
            Vector3 flyOutPos = prevPos;
            flyOutPos.y += 14f * Time.deltaTime; // Move up quickly
            flyOutPos.x += flyOutDirection * 5.0f * Time.deltaTime; // Move sideways (right or left)
            flyOutPos.z -= 30f * Time.deltaTime; // Move towards player (-z direction)
            fishInstance.transform.position = flyOutPos;

            // Face direction of movement
            Vector3 velocity = flyOutPos - prevPos;
            if (velocity.sqrMagnitude > 0.0001f)
            {
                Quaternion moveRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
                // Oscillate only the fish's nose (yaw)
                float oscillation = Mathf.Sin(Time.time * 17f) * 35f; // frequency=17, amplitude=35 degrees
                fishInstance.transform.rotation = moveRotation * Quaternion.Euler(0, oscillation, 0);
            }
        }
        else
        {
            // Reset only fish position and depth, NOT score
            isPaused = false;
            pauseTimer = 0f;
            fishDepth = startingFishDepth;
            fishInstance.transform.position = fishSpawnLocation;
            fishInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            isFishCaught = false;
        }
    }

    private void StartCaughtAnimation()
    {
        isPaused = true;
        pauseTimer = 0f;
        // Randomly choose fly out direction: -1 (left) or 1 (right)
        flyOutDirection = (Random.value < 0.5f) ? -1 : 1;
    }

    private void UpdateFishMovement()
    {
        Vector3 previousPosition = fishInstance.transform.position;
        timeCounter += Time.deltaTime * speed;
        float x = fishSpawnLocation.x + Mathf.Cos(timeCounter) * radius;
        float z = fishSpawnLocation.z + Mathf.Sin(timeCounter) * radius;
        float offset = Mathf.Sin(timeCounter * 3f) * squiggleAmplitude;
        float y = (fishDepth * 0.1f) - 10.0f;
        Vector3 newPosition = new Vector3(x + offset, y, z + offset);
        fishInstance.transform.position = newPosition;

        Vector3 direction = (newPosition - previousPosition).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            fishInstance.transform.rotation = Quaternion.Slerp(fishInstance.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public GameObject GetFishInstance()
    {
        return fishInstance;
    }
}
