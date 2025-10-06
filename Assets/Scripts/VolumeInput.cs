using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeInput : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumetDisplayText;

    void Start()
    {
        volumeSlider.wholeNumbers = true;

        string savedVolume = PlayerPrefs.GetString("Volume", "30");
        volumeSlider.value = float.Parse(savedVolume);
        volumetDisplayText.text = savedVolume;

        volumeSlider.onValueChanged.AddListener(delegate { SaveVolume(); });
    }

    void SaveVolume()
    {
        string volume = ((int)volumeSlider.value).ToString();
        PlayerPrefs.SetString("Volume", volume);
        volumetDisplayText.text = volume;
    }
}
