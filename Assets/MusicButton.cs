using TMPro;
using UnityEngine;

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

            // Play the audio with the calculated delay
            au.PlayDelayed(GlobalVariables.delay);

            if (buttonText != null)
                buttonText.text = au.clip != null ? au.clip.name : "Playing";
        }
    }

    public void PauseMusic()
    {
        if (au != null && au.isPlaying)
            au.Pause();
    }

    public void ResumeMusic()
    {
        if (au != null)
            au.UnPause();
    }
}
