using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public Button yourButton;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Time.timeScale = 0f; // Pause the game
        Debug.Log("You have clicked the button!");
        SceneController.Instance.LoadScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
