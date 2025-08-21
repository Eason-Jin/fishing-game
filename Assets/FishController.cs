using UnityEngine;

public class FishController : MonoBehaviour
{
    public PlotController plotController;
    public GameObject fishPrefab;
    public int startingFishDepth = -100;

    private DotStatus dotStatus;
    private int score = 0;
    private int fishDepth;

    void Start()
    {
        if (plotController != null)
        {
            dotStatus = plotController.dotStatus;
        }
        fishDepth = startingFishDepth;

        StartCoroutine(UpdateFishState());
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
}
