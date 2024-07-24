using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //make the SoundManager a singleton
    public static SoundManager Instance;

    //awake
    private void Awake()
    {
        //set the singleton
        Instance = this;
    }

    //play a sound
    public void PlayClip(AudioClip clip, AudioSource audioSource)
    {
        //assign the clip
        audioSource.clip = clip;

        //play the sound
        audioSource.Play();
    }

    //play random clip
    public void PlayRandomClip(AudioClip[] clips, AudioSource audioSource)
    {
        //assign a random clip
        audioSource.clip = clips[Random.Range(0, clips.Length)];

        //play the sound
        audioSource.Play();
    }
    
}
