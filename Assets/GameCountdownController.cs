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
        float countdownTime = 5f; // 5 seconds countdown
        float prevTimeScale = Time.timeScale;
        Time.timeScale = 0f; // Pause the game
        float unscaledStart = Time.unscaledTime;
        while (countdownTime > 0)
        {
            countdownText.text = $"Time Left: {countdownTime:F1}";
            float unscaledNow = Time.unscaledTime;
            countdownTime = 5f - (unscaledNow - unscaledStart);
            yield return null;
        }
        Time.timeScale = prevTimeScale; // Resume the game
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
