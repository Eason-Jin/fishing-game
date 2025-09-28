using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTimeController : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateScore(float score) {
        scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateTime(float time) {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.RoundToInt(time % 60f);
        timeText.text = $"Time: {minutes}:{seconds:00}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        // Reset time display when exiting the scene
        if (timeText != null)
            timeText.text = "Time: 0:00";
    }
}
