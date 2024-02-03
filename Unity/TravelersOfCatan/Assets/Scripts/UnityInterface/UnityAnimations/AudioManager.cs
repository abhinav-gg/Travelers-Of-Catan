using System;
using UnityEngine;

/// <summary>
/// Controls the audio for the game
/// Credit to Brackeys for the tutorial on this script: <seealso href="https://www.youtube.com/watch?v=6OT43pvUyfY"/>
/// </summary>
public class AudioManager : MonoBehaviour
{
    // Singleton instance of the AudioManager class so that it can be accessed from other scripts
    public static AudioManager i;
    public Sound[] sounds;
    public Sound[] BackgroundMusic;
    [Range(0f, 1f)] public float VolumeModifier = 1f;
    private bool mutedSFX = false;
    private bool mutedBG = false;

    // Awake is called before Start
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

    // Start is called before the first frame update
    private void Start()
    {
        Sound s = BackgroundMusic[0];
        s.source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = VolumeModifier;
    }

    // Plays the sound with the given name
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

    // Stops the sound with the given name
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.source.Stop();
    }

    // Stops all sounds
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

    // Adjusts the game volume
    public void ChangeMasterVolume(float vol)
    {
        VolumeModifier = vol;
        
    }

    // Toggles the mute for SFX or background music depending on the bool parameter
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

    // Returns whether the game is muted or not
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

// Class that holds the information for a sound
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