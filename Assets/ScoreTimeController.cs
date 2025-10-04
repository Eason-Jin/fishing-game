using TMPro;
using UnityEngine;

public class ScoreTimeController : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text fishCaughtText;

    public void UpdateScore(float score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateFishCaught(int fishCaught)
    {
        if (fishCaughtText != null)
            fishCaughtText.text = "Fish Caught: " + fishCaught.ToString();
    }
}
