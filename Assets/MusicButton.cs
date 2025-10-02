using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    [SerializeField] Button musicButton;
    [SerializeField] bool musicEnabled = false;
    [SerializeField] AudioClip musciClip;
    [SerializeField] AudioSource au;
    [SerializeField] TextMeshProUGUI buttonText;

    // Start is called before the first frame update
    void Start()
    {
        musicButton.onClick.AddListener(OnButtonClick);
        au.volume = float.Parse(SettingsController.Instance.GetVolume()) * 0.01f;
    }

    void OnButtonClick()
    {
        if (!musicEnabled)
        {
            musicEnabled = true;
            au.Play();
            buttonText.text = au.clip.name;

        }
        else
        {
            musicEnabled = false;
            au.Pause();
            buttonText.text = "Music";
        }
    }
}
