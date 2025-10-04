using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;        // Needed for InputField
using TMPro;                // Needed if using TextMesh Pro

public class WeightInput : MonoBehaviour
{
    public TMP_InputField weightInputField;

    void Start()
    {
        // preload saved weight into the input field
        weightInputField.text = SettingsController.Instance.GetWeight();

        // attach SaveWeight to OnEndEdit
        weightInputField.onEndEdit.AddListener(delegate { SaveWeight(); });
    }

    void SaveWeight()
    {
        SettingsController.Instance.SetWeight(weightInputField.text);
    }

    public string GetSavedWeight()
    {
        return PlayerPrefs.GetString("PlayerWeight", "0");
    }
}
