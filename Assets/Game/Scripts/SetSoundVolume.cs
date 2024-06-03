using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSoundVolume : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out audioSource);
        audioSource.volume = PlayerPrefs.GetFloat("soundvolume");
    }
    private void Start()
    {
        TryGetComponent(out audioSource);
        audioSource.volume = PlayerPrefs.GetFloat("soundvolume");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateVolume()
    {
        audioSource.volume = PlayerPrefs.GetFloat("soundvolume");
    }
}
