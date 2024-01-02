using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Threading;

public class AudioManager : MonoBehaviour
{

    // Credit to Brackeys for the tutorial on this script https://www.youtube.com/watch?v=6OT43pvUyfY


    public bool muted = false;
    public Sound[] sounds;
    public Sound[] BGmusic;

    public static AudioManager i;
    public float VolumeModifier = 1f;

    void Awake()
    {

        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.Clip;
            s.source.volume = s.Vol * VolumeModifier;
            s.source.pitch = s.Pitch;
            s.source.loop = s.Loop;

        }

        foreach (Sound s in BGmusic)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.Clip;
            s.source.volume = s.Vol * VolumeModifier;
            s.source.pitch = s.Pitch;
            s.source.loop = s.Loop;

        }
    }

    void Update()
    {
        AudioListener.volume = VolumeModifier;
    }

    public void StartSongs()
    {

    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || muted)
        {
            return;
        }
        s.source.PlayOneShot(s.Clip);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || muted)
        {
            return;
        }
        s.source.Stop();
    }

    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            try
            {
                s.source.Stop();
            }
            catch {;}
        }
    }

    public void ChangeMasterVolume(float vol)
    {
        VolumeModifier = vol;
        
    }

    public void Mute()
    {
        muted = true;
        StopAll();
    }

    public void Unmute()
    {
        muted = false;
    }

}

[System.Serializable]
public class Sound
{

    public string name;
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Vol = 0.5f;

    [Range(1f, 3f)]
    public float Pitch = 1;

    public bool Loop = false;

    [HideInInspector]
    public AudioSource source;

}