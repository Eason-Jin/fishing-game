using System.Collections;
using TMPro;
using UnityEngine;

public class GameCountdownController : MonoBehaviour
{

    public TMP_Text messageText;
    public TMP_Text countdownText;
    public float interSetPauseDuration = 120f;

    private Coroutine interSetPauseCoroutine;

    private int setsCompleted = 0;
    public int setsToPlay = 3;

    // Start is called before the first frame update
    void Start()
    {
        if (GlobalVariables.settingsComplete)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        // Time.timeScale = 0f; // Pause the game !!GAME NOW PAUSED IN PlayButton.cs!!
        float countdownTime = 5f; // 5 seconds countdown
        float prevTimeScale = Time.timeScale;
        float unscaledStart = Time.unscaledTime;
        while (countdownTime > 0)
        {
            countdownText.text = $"{countdownTime:F1}";
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
        }
        else
        {
            Debug.LogWarning("MusicButton not found in scene.");
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

    public void StartInterSetPause(PlotController plotController)
    {
        // Ensure this GameObject and its parent are active so coroutine/UI works
        if (transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
            transform.parent.gameObject.SetActive(true);
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        if (interSetPauseCoroutine == null)
            interSetPauseCoroutine = StartCoroutine(InterSetPauseCoroutine(plotController));
    }

    private IEnumerator InterSetPauseCoroutine(PlotController plotController)
    {
        // Pause music
        MusicButton musicButton = FindObjectOfType<MusicButton>();
        if (musicButton != null)
        {
            musicButton.PauseMusic();
        }

        // Set pause flag in plot controller
        if (plotController != null)
            plotController.isInterSetPauseActive = true;

        float pauseTime = interSetPauseDuration;
        float unscaledStart = Time.unscaledTime;

        // Show countdown UI
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        // Do NOT pause Time.timeScale, so dot keeps moving
        while (pauseTime > 0)
        {
            if (countdownText != null)
                countdownText.text = $"{pauseTime:F1}";
            float unscaledNow = Time.unscaledTime;
            pauseTime = interSetPauseDuration - (unscaledNow - unscaledStart);
            yield return null;
        }

        // Resume music
        if (musicButton != null)
        {
            musicButton.ResumeMusic();
        }
        setsCompleted++;

        // Reset plot controller state or finish game
        if (setsCompleted >= setsToPlay)
        {
            if (plotController != null)
            {
                plotController.isPaused = true;
                plotController.isFinished = true;
            }
        }
        else if (plotController != null)
        {
            plotController.OnInterSetPauseEnd();
        }
        gameObject.transform.parent.gameObject.SetActive(false);
        interSetPauseCoroutine = null;
    }

    public void ShowFinishedMessage()
    {
        if (messageText != null && countdownText != null)
        {
            if (transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
                transform.parent.gameObject.SetActive(true);
            if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
            messageText.gameObject.SetActive(true);
            countdownText.gameObject.SetActive(false);
            messageText.text = "Game Finished! Thank you for playing.";
        }
    }
}
