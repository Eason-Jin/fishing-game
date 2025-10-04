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
        string weight = SettingsController.Instance.GetWeight();

        if (string.IsNullOrEmpty(weight))
        {
            warningPanel.SetActive(true);
        }
        else
        {
            warningPanel.SetActive(false);
        }
    }
}
