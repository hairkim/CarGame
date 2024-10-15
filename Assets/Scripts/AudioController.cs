using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    // Audio clips for different scenes or game states
    public AudioClip mainMenuMusic;
    public AudioClip inGameMusic;
    public AudioClip pauseMenuMusic;
    public AudioClip gameOverMusic;
    public AudioClip buttonClickSound;
    public AudioClip skrtAudio;
    public AudioClip ambiance;
    public AudioClip cameraAudio;
    public AudioClip alertSound;

    public AudioSource ambianceSource;
    public AudioSource audioSource;
    public AudioSource sfxAudioSource;

    private float inGameMusicPosition;
    private float musicAudioLevel;
    private float sfxAudioLevel;

    void Awake()
    {
        // Singleton pattern to ensure only one instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps AudioManager persistent across scenes
            inGameMusicPosition = 0;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        ambianceSource.clip = ambiance;
        ambianceSource.Play();
    }

    // Method to play different types of music
    public void PlayMusic(string musicType)
    {
        switch (musicType)
        {
            case "MainMenu":
                audioSource.clip = mainMenuMusic;
                break;
            case "InGame":
                audioSource.clip = inGameMusic;
                break;
            case "PauseMenu":
                audioSource.clip = pauseMenuMusic;
                break;
            case "GameOver":
                audioSource.clip = gameOverMusic;
                break;
        }

        //want to call resume music if it is ingamemusic and it has not already been played yet
        if (audioSource.clip == inGameMusic && inGameMusicPosition > 0)
        {
            ResumeMusic();
        }

        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PauseMusicAndPlayPauseMenu()
    {
        if(audioSource.clip == inGameMusic && audioSource.isPlaying)
        {
            inGameMusicPosition = audioSource.time;
            Debug.Log("changing inGameMusicPosition to " + audioSource.time);
            PlayMusic("PauseMenu");
        }
    }

    public void ResumeMusic()
    {
        Debug.Log("Resuming at " + inGameMusicPosition);
        audioSource.time = inGameMusicPosition;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the in-game music position when a new scene is loaded
        Debug.Log("Scene loaded: " + scene.name + ". Resetting inGameMusicPosition.");
        inGameMusicPosition = 0;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void toggleMusicAudio(bool isMusicOff)
    {
        if(isMusicOff)
        {
            musicAudioLevel = audioSource.volume;
            audioSource.volume = 0;
        } else
        {
            audioSource.volume = musicAudioLevel;
        }
    }

    public void toggleSfxAudio(bool isSfxOff)
    {
        if (isSfxOff)
        {

            sfxAudioLevel = sfxAudioSource.volume;
            sfxAudioSource.volume = 0;
        }
        else
        {
            sfxAudioSource.volume = sfxAudioLevel;
        }
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            sfxAudioSource.PlayOneShot(buttonClickSound); // Plays click sound effect without interrupting music
        }
    }

    public void PlaySkrtSound()
    {
        sfxAudioSource.PlayOneShot(skrtAudio, 0.3f);
    }

    public void PlayCameraSound()
    {
        sfxAudioSource.PlayOneShot(cameraAudio, 0.3f);
    }

    public void PlayAlertSound()
    {
        sfxAudioSource.PlayOneShot(alertSound);
    }

    //
    //press esc (initiate PauseMusic) this is going to save last in game music time and play the pause menu song
    //
    //press resume (initiate ResumeMusic)
    //
}
