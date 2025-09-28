using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    [SerializeField] bool musicEnabled = false;
    [SerializeField] AudioClip musciClip;
    [SerializeField] AudioSource au;
    [SerializeField] TextMeshProUGUI buttonText;

    // Start is called before the first frame update
    void Start()
    {
    au.volume = float.Parse(SettingsController.Instance.GetVolume()) * 0.01f;
    }

    void OnButtonClick()
    {
        // no longer used
    }

    public void PlayMusic()
    {
        if (!musicEnabled)
        {
            musicEnabled = true;
            au.Play();
            if (buttonText != null)
                buttonText.text = au.clip != null ? au.clip.name : "Playing";
        }
    }
}
