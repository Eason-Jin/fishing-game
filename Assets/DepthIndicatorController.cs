using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthIndicatorController : MonoBehaviour
{
    int progres = 0;
    public Slider slider;

    public void OnSliderChanged(float value)
    {
        progres = (int)value;
        Debug.Log("Slider changed to: " + progres);
    }

    public void UpdateProgress(int progres) {
        slider.value = progres;
        Debug.Log("Slider progress updated to: " + progres);
    }
}

