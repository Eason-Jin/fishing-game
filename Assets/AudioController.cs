using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private GameAudioUi gameAudioUi;

    [Header("Master Audio")]
    [Tooltip("Master volume level (0 - 100)")]
    public int volume = 30; // volume default set to 30% to not kill ears (0 - 100)
    // NOTE: volume in public and shown in inspector for dev purposes. Later, we should make this private and adjustable in game with the SetVolume() method

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource SFXSource;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip SFX1;
    public AudioClip SFX2;
    public AudioClip SFX3;

    private void Start() {
        backgroundMusicSource.clip = background;
        SetVolume(volume);
        backgroundMusicSource.loop = true;
        backgroundMusicSource.Play();

        gameAudioUi = FindObjectOfType<GameAudioUi>();
        gameAudioUi.UpdateAudioText(backgroundMusicSource.clip.name);
    }

    public void SetVolume(int volume)
    {
        float normalized = Mathf.Clamp(volume, 0, 100) / 100f;
        backgroundMusicSource.volume = normalized;
        SFXSource.volume = normalized;
    }
}
