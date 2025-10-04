using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeightInput : MonoBehaviour
{
    public Slider weightSlider;
    public TMP_Text weightDisplayText;

    void Start()
    {
        weightSlider.wholeNumbers = true;

        string savedWeight = PlayerPrefs.GetString("PlayerWeight", "");
        weightSlider.value = float.Parse(savedWeight);
        weightDisplayText.text = savedWeight;

        weightSlider.onValueChanged.AddListener(delegate { SaveWeight(); });
    }

    void SaveWeight()
    {
        string weight = ((int)weightSlider.value).ToString();
        PlayerPrefs.SetString("PlayerWeight", weight);
        weightDisplayText.text = weight;
    }
}
