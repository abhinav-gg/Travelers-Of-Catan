using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Threading;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{

    public static AudioManager i;

    // Credit to Brackeys for the tutorial on this script https://www.youtube.com/watch?v=6OT43pvUyfY

    public Sound[] sounds;
    public Sound[] BackgroundMusic;

    // add inspector range to this from 0 to 1
    [Range(0f, 1f)] public float VolumeModifier = 1f;
    private bool mutedSFX = false;
    private bool mutedBG = false;

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

        foreach (Sound s in BackgroundMusic)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.Clip;
            s.source.volume = s.Vol * VolumeModifier;
            s.source.pitch = s.Pitch;
            s.source.loop = s.Loop;

        }
    }

    private void Start()
    {
        Sound s = BackgroundMusic[0];
        s.source.Play();
    }

    void Update()
    {
        AudioListener.volume = VolumeModifier;
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log(name + "not found");
            return;
        }
        else if (mutedSFX)
        {
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
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

    public void ToggleMute(bool Background)
    {
        if (Background)
        {
            mutedBG = !mutedBG;
            Sound s = BackgroundMusic[0];
            if (mutedBG)
            {
                s.source.Stop();
            }
            else
            {
                s.source.Play();
            }
        }
        else
        {
            mutedSFX = !mutedSFX;
            if (mutedSFX)
            {
                StopAll();
            }
        }
    }

    public bool isMuted(bool Background)
    {
        if (Background)
        {
            return mutedBG;
        }
        else
        {
            return mutedSFX;
        }
    }


}

[System.Serializable]
public class Sound
{

    public string name;
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Vol = 1f;

    [Range(1f, 3f)]
    public float Pitch = 1f;

    public bool Loop = false;

    [HideInInspector]
    public AudioSource source;

}