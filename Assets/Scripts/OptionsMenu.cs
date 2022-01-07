using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private Slider musicSlider; 
    private AudioManager am;
    private Sound themeMusic;
    private HoldValues hold;
    private void OnEnable()
    {
        musicSlider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        themeMusic = Array.Find(am.sounds, sound => sound.name == am.musicName);
        hold = GameObject.Find("Holder").GetComponent<HoldValues>();

        musicSlider.value = hold.MusicVolume;
    }

    private void OnDisable()
    {
        hold.MusicVolume = musicSlider.value;
    }

    // Executed on change of music slider 
    public void UpdateMusicVolume()
    {
        // Set value of music volume to music slider value
        am.UpdateVolume(themeMusic, musicSlider.value);
    }
}
