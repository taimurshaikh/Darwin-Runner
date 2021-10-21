using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public string musicName;
    public Sound[] sounds;
    public static AudioManager instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play(musicName);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.source.Play();
    }

    public void UpdateVolume(Sound s, float volume)
    {
        // Change Audio Manager volume value for this sound
        s.volume = volume;
        
        // Update actual sound volume from the sound source in the game.
        s.source.volume = volume;
    }
}
