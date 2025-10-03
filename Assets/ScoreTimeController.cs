using TMPro;
using UnityEngine;

public class ScoreTimeController : MonoBehaviour
{
    public TMP_Text scoreText;
    //public TMP_Text timeText;
    public TMP_Text fishCaughtText;
    //private float gameplayTime = 0f;
    //private bool timerRunning = false;

    //private void OnEnable()
    //{
    //    gameplayTime = 0f;
    //    timerRunning = false;
    //    if (timeText != null)
    //        timeText.text = "Time: 0:00";
    //}

    public void StartTimer()
    {
        //timerRunning = true;
    }

    public void StopTimer()
    {
        //timerRunning = false;
        //gameplayTime = 0f;
        //if (timeText != null)
        //    timeText.text = "Time: 0:00";
    }

    //public float GetTime()
    //{
    //    return gameplayTime;
    //}

    private void Update()
    {
        //if (timerRunning)
        //{
        //    gameplayTime += Time.deltaTime;
        //    if (timeText != null)
        //    {
        //        int minutes = Mathf.FloorToInt(gameplayTime / 60f);
        //        int seconds = Mathf.RoundToInt(gameplayTime % 60f);
        //        timeText.text = $"Time: {minutes}:{seconds:00}";
        //    }
        //}
    }

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

    //private void OnDisable()
    //{
    //    if (timeText != null)
    //        timeText.text = "Time: 0:00";
    //}
}
