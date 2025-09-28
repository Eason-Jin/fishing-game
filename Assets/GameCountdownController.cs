using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCountdownController : MonoBehaviour
{

    public TMP_Text countdownText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        Time.timeScale = 0f; // Pause the game
        float countdownTime = 5f; // 5 seconds countdown
        float prevTimeScale = Time.timeScale;
        float unscaledStart = Time.unscaledTime;
        while (countdownTime > 0)
        {
            countdownText.text = $"Time Left: {countdownTime:F1}";
            float unscaledNow = Time.unscaledTime;
            countdownTime = 5f - (unscaledNow - unscaledStart);
            yield return null;
        }

        // Resume the game
        Time.timeScale = prevTimeScale;

        // Play music after countdown
        MusicButton musicButton = FindObjectOfType<MusicButton>();
        if (musicButton != null)
        {
            musicButton.PlayMusic();
        } else
        {
            Debug.LogWarning("MusicButton not found in scene.");
        }

        // Start the gameplay timer after countdown
        ScoreTimeController scoreTimeController = FindObjectOfType<ScoreTimeController>();
        if (scoreTimeController != null)
        {
            scoreTimeController.StartTimer();
        } else {
            Debug.LogWarning("ScoreTimeController not found in scene.");
        }

        gameObject.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Safety: If countdown UI is disabled early, always resume game
        if (!gameObject.activeInHierarchy && Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        
    }

    private void OnDisable()
    {
        // Always resume game if this object is disabled (scene change, etc)
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;
    }
}
