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

        int savedWeight = PlayerPrefs.GetInt("PlayerWeight", 5);
        weightSlider.value = savedWeight;
        weightDisplayText.text = savedWeight.ToString();

        weightSlider.onValueChanged.AddListener(delegate { SaveWeight(); });
    }

    void SaveWeight()
    {
        int weight = (int)weightSlider.value;
        PlayerPrefs.SetInt("PlayerWeight", weight);
        weightDisplayText.text = weight.ToString();
    }
}
