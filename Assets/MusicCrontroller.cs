using UnityEngine;
using UnityEngine.UI;

public class MusicDropdownController : MonoBehaviour
{
    public Dropdown musicDropdown;   // Create pulldown menu
    public AudioSource audioSource;  
    public AudioClip[] musicClips;   // Music list

    void Start()
    {
        
        if (musicDropdown == null || audioSource == null || musicClips.Length == 0)
        {
            Debug.LogError("请先在 Inspector 中设置 Dropdown、AudioSource 和音乐列表");
            return;
        }

     
        musicDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    
    void OnDropdownValueChanged(int index)
    {
        if (index < 0 || index >= musicClips.Length) return;

        // Stop current music
        if (audioSource.isPlaying)
            audioSource.Stop();

        // Play the chosen music
        audioSource.clip = musicClips[index];
        audioSource.Play();

        Debug.Log("Playing: " + musicClips[index].name);
    }
}

