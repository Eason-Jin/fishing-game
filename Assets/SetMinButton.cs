using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetMinButton : MonoBehaviour
{
    public TMP_Text heightText;
    public Transform leftController;
    public Transform rightController;
    public Button button;

    private float timer;
    private int state; // 0: idle, 1: countdown, 2: calculate height

    void Start()
    {
        Button btn = button.GetComponent<Button>();
        btn.onClick.AddListener(StartCountdown);
        state = 0;
    }

    void Update()
    {
        if (state == 1) // Countdown phase
        {
            timer -= Time.deltaTime;
            if (timer > 0)
            {
                heightText.text = timer.ToString("F1");
            }
            else
            {
                state = 2;
                CalculateHeight();
                state = 0;
            }
        }
    }

    void StartCountdown()
    {
        if (state == 0) // Only start if idle
        {
            Debug.Log("Starting state 1.");
            state = 1;
            timer = 3f;
        }
    }

    void CalculateHeight()
    {
        int iters = 5;
        float[] heights = new float[iters];
        for (int i = 0; i < iters; i++)
        {
            heights[i] = (leftController.position.y + rightController.position.y) / 2;
            heightText.text = heights[i].ToString("F2");
        }

        float sum = 0;
        for (int i = 0; i < iters; i++)
        {
            sum += heights[i];
        }
        float avgHeight = sum / iters;
        heightText.text = avgHeight.ToString("F2");
        PlayerPrefs.SetFloat("MinHeight", avgHeight);
    }
}
