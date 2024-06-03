using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource backgroundMusic;

    public AudioSource effectAus1;
    public AudioSource effectAus2;
    public AudioSource effectAus3;
    public AudioSource effectAus4;
    public AudioSource effectAus5;
    public AudioSource effectAus6;

    public AudioClip[] clips;

    public int lastIndex = -1;

    public Slider audioVolSlide;

    public int currentBSindex = 0;
    public AudioSource[] backgroundSound;
    public AudioSource generalAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("setsound") == 0)
        {
            PlayerPrefs.SetFloat("soundvolume", 0.8f);
            PlayerPrefs.SetInt("setsound", 1);
            audioVolSlide.value = 0.8f;
        }
        else audioVolSlide.value = PlayerPrefs.GetFloat("soundvolume");
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            BackgroundSound();
    }
    public void PlayOneShotSound(int clipIndex, AudioSource audioSource, bool isNoPlaying = false, bool stopPlaying = false)
    {
        if (isNoPlaying && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(clips[clipIndex]);
        }
        else if (stopPlaying)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clips[clipIndex]);
        }
    }
    public void StopSound(AudioSource audioSource)
    {
        audioSource.Stop();
    }
    public void ChangeBackgroundMusic(int clipIndex)
    {
        backgroundMusic.clip = clips[clipIndex];
        backgroundMusic.Play();
    }

    public void SetUpdateSoundVol()
    {
        PlayerPrefs.SetFloat("soundvolume", audioVolSlide.value);

        SetSoundVolume[] setSoundVolume = GameObject.FindObjectsByType<SetSoundVolume>(FindObjectsSortMode.None);
        foreach (SetSoundVolume ssv in setSoundVolume)
        {
            ssv.UpdateVolume();
        }
    }
    public bool CheckBcPlaying()
    {
        for (int i = 0; i < backgroundSound.Length; i++)
        {
            if (backgroundSound[i].isPlaying)
            {
                return true;
            }
        }

        return false;
    }
    public void BackgroundSound()
    {
        if (!CheckBcPlaying())
        {
            int index = -1;
            if (backgroundSound.Length - 1 > currentBSindex)
            {
                currentBSindex++;
                index = currentBSindex - 1;
            }
            else if (backgroundSound.Length - 1 == currentBSindex)
            {
                currentBSindex = 0;
                index = backgroundSound.Length - 1;
            }

            backgroundSound[index].Play();
        }
    }
}
