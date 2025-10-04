using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameAudioUi : MonoBehaviour
{
    public TMP_Text audioText;

    public void UpdateAudioText(string text)
    {
        audioText.text = "Audio: " + text;
    }
}
