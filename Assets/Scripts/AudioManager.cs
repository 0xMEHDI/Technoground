using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    public static AudioManager instance;
    
    private void Awake()
    {
        instance = this;
        GameObject newMusicSource = new GameObject();
        audioSource = newMusicSource.AddComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip musicClip, float volume = 1f)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = musicClip;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
