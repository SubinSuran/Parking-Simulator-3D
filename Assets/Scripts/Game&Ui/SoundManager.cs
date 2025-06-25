using UnityEngine;
using System.Collections;

/// <summary>
/// A persistent Singleton that manages all game audio.
/// It has dedicated sources for main BGM, panel BGM, and one-shot sound effects.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    [Tooltip("The AudioSource for looping main background music.")]
    public AudioSource musicSource;
    [Tooltip("The AudioSource for looping panel music (pause, win, lose).")]
    public AudioSource panelMusicSource;
    [Tooltip("The AudioSource for short sound effects.")]
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip panelLoopMusic;
    public AudioClip uiClickSound;
    public AudioClip carCrashSound;
    public AudioClip levelWinSound;

    private void Awake()
    {
        // Singleton Pattern: Ensures only one instance of the SoundManager ever exists.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist this object between scenes
        }
        else
        {
            // If another SoundManager tries to start, destroy it to avoid duplicates.
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Set up and start playing the main background music
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // --- Public functions to be called by other scripts ---

    /// <summary>
    /// Plays the looping music for UI panels and fades out the main BGM.
    /// </summary>
    public void PlayPanelMusic()
    {
        if (panelMusicSource != null && panelLoopMusic != null)
        {
            musicSource.volume = 0.3f; // Lower main music volume
            panelMusicSource.clip = panelLoopMusic;
            panelMusicSource.loop = true;
            panelMusicSource.Play();
        }
    }

    /// <summary>
    /// Stops the looping panel music and restores main BGM volume.
    /// </summary>
    public void StopPanelMusic()
    {
        if (panelMusicSource != null)
        {
            panelMusicSource.Stop();
        }
        musicSource.volume = 1f; // Restore main music volume
    }

    /// <summary>
    /// Plays a given sound effect clip one time.
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // --- Convenience Functions ---

    public void PlayClickSound()
    {
        PlaySound(uiClickSound);
    }

    public void PlayCrashSound()
    {
        PlaySound(carCrashSound);
    }

    public void PlayWinSound()
    {
        PlaySound(levelWinSound);
    }
}
