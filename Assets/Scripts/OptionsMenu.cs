using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider musicSlider; 
    public Slider sfxSlider;
    public AudioManager am;
    private Sound themeMusic;
    void Start()
    {
        themeMusic = Array.Find(am.sounds, sound => sound.name == am.musicName);
        themeMusic.volume = musicSlider.value;
    }

    // Executed on change of music slider 
    public void UpdateMusicVolume()
    {
        // Set value of music volume to music slider value
        am.UpdateVolume(themeMusic, musicSlider.value);
    }

    // Executed on change of SFX volume
    public void UpdateSFXVolume()
    {
        // All other sounds other than 0th item are sfx, so update volume of all of them to sfx slider value
        for (int i = 1; i < am.sounds.Length; i++)
        {
            am.UpdateVolume(am.sounds[i], sfxSlider.value);
        }
    }
}
