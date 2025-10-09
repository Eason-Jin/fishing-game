using TMPro;
using UnityEngine;

public class MusicButton : MonoBehaviour
{
    [SerializeField] bool musicEnabled = false;
    [SerializeField] AudioClip musciClip;
    [SerializeField] AudioSource au;
    [SerializeField] TextMeshProUGUI buttonText;

    public PlotController plotController;
    private float bpm = 120f; // Default BPM
    private float beatOffset = 0f; // Default beat offset in seconds

    // Start is called before the first frame update
    void Start()
    {
        au.volume = float.Parse(SettingsController.Instance.GetVolume()) * 0.01f;
        if (plotController != null)
        {
            bpm = plotController.bpm;
            beatOffset = plotController.beatOffset;
        }
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

            // Calculate delay time based on beatOffset and bpm
            float delayTime = (beatOffset / bpm);

            // Play the audio with the calculated delay
            au.PlayDelayed(delayTime);

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
