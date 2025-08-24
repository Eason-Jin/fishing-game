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

    private Vector3 fishSpawnLocation = new Vector3(0, -3, 10);
    private float radius = 3f;           // size of the circle
    private float speed = 1f;            // how fast it moves
    private float squiggleAmplitude = 2f; // how wavy it is
    private float timeCounter = 0f;

    private FishingLine fishingLine;

    void Start()
    {
        if (plotController != null)
        {
            dotStatus = plotController.dotStatus;
        }
        fishDepth = startingFishDepth;

        StartCoroutine(UpdateFishState());

        if (fishPrefab == null)
        {
            Debug.LogError("Fish prefab is not assigned.");
            //return;
        }

        // spawn temporary fish instance (primitive) and position
        fishInstance = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        fishInstance.transform.position = fishSpawnLocation;
        fishInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        fishInstance.transform.rotation = Quaternion.identity;

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

            timeCounter += Time.deltaTime * speed;

            // Circular path + squiggle
            float x = fishSpawnLocation.x + Mathf.Cos(timeCounter) * radius;
            float z = fishSpawnLocation.z + Mathf.Sin(timeCounter) * radius;
            float offset = Mathf.Sin(timeCounter * 3f) * squiggleAmplitude; // squiggle in X-Z plane

            fishInstance.transform.position = new Vector3(x + offset, fishSpawnLocation.y, z + offset);
        }
    }
    public GameObject GetFishInstance()
    {
        return fishInstance;
    }
}
