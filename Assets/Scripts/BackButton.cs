using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public Button yourButton;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        // Check if all settings are set
        string weight = PlayerPrefs.GetString("PlayerWeight", "");
        float minHeight = PlayerPrefs.GetFloat("MinHeight", -1.0f);
        float maxHeight = PlayerPrefs.GetFloat("MaxHeight", -1.0f);
        if (string.IsNullOrEmpty(weight) || minHeight < 0 || maxHeight < 0)
        {
            GlobalVariables.settingsComplete = false;
            Debug.Log("Please set all settings before going back.");
            return;
        }
        else
        {
            GlobalVariables.settingsComplete = true;
            SceneController.Instance.LoadPreviousScene();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
