using UnityEngine;

public class WeightChecker : MonoBehaviour
{
    public GameObject warningPanel;   // Group of UI components

    void Start()
    {
        CheckWeight();
    }

    void Update()
    {
        //CheckWeight(); // optional, remove if not needed every frame
    }

    void CheckWeight()
    {
        if (!GlobalVariables.settingsComplete)
        {
            warningPanel.SetActive(true);
        }
        else
        {
            warningPanel.SetActive(false);
        }
    }
}
