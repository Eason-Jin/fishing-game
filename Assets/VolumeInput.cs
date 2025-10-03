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

        int savedVolume = PlayerPrefs.GetInt("Volume", 30);
        volumeSlider.value = savedVolume;
        volumetDisplayText.text = savedVolume.ToString();

        volumeSlider.onValueChanged.AddListener(delegate { SaveVolume(); });
    }

    void SaveVolume()
    {
        int weight = (int)volumeSlider.value;
        PlayerPrefs.SetInt("Volume", weight);
        volumetDisplayText.text = weight.ToString();
    }
}
