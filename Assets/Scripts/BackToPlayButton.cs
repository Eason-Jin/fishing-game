using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackToPlayButton : MonoBehaviour
{
    public Button yourButton;

    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the home button!");
        SceneController.Instance.LoadScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
