using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;        // Needed for InputField
using TMPro;                // Needed if using TextMesh Pro

public class VolumeInput : MonoBehaviour
{
    public TMP_InputField volumeInputField;

    void Start()
    {
        // preload saved weight into the input field
        volumeInputField.text = SettingsController.Instance.GetVolume();

        // attach SaveWeight to OnEndEdit
        volumeInputField.onEndEdit.AddListener(delegate { SaveVolume(); });
    }

    void SaveVolume()
    {
        SettingsController.Instance.SetVolume(volumeInputField.text);
    }

    public string GetSavedVolume()
    {
        return PlayerPrefs.GetString("Volume", "0");
    }
}
