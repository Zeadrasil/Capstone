using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
[DefaultExecutionOrder(-20)]
//Manages saving and loading
public class MusicManager : Singleton<MusicManager>
{
    //Music data
    [SerializeField] private AudioSource menuMusic;
    private float menuRate = 0;
    [SerializeField] private AudioSource betweenMusic;
    private float betweenRate = 0;
    [SerializeField] private AudioSource[] battleMusics = new AudioSource[6];
    private float[] battleRates = { 0, 0, 0, 0, 0, 0 };
    private int selectedBattle = 0;
    public float musicFadeTime = 5;

    //SFX data
    [SerializeField] private AudioSource clickFX;

    //Volume data
    public float masterVolume = 100;
    public float musicVolume = 100;
    public float sfxVolume = 100;

    // Start is called before the first frame update
    private void Start()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 100);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 100);

        MenuManager.Instance.masterVolumeSlider.value = masterVolume;
        MenuManager.Instance.musicVolumeSlider.value = musicVolume;
        MenuManager.Instance.sfxVolumeSlider.value = sfxVolume;

        //Start the menu music since that is where you load in
        PlayMenu();
    }

    //Called every frame
    private void Update()
    {
        //Maximum volume calculation
        float actualMusicVolume = (musicVolume / 100) * (masterVolume / 100);

        //Menu update
        if (menuRate != 0)
        {
            //Start music if volume is 0
            if (menuMusic.volume == 0)
            {
                menuMusic.Play();
            }
            bool aboveMax = menuRate < 0 && menuMusic.volume > actualMusicVolume;

            //Change volume based on whether it is stopping or starting
            menuMusic.volume = Mathf.Clamp(menuMusic.volume + menuRate / musicFadeTime * Time.deltaTime * actualMusicVolume, 0, actualMusicVolume);

            //If decreasing to new max volume is finished
            if (aboveMax && menuMusic.volume < actualMusicVolume)
            {
                menuRate = 0;
                menuMusic.volume = actualMusicVolume;
            }
            //Stop music if volume is zero
            else if (menuMusic.volume == 0)
            {
                menuMusic.Stop();
                menuRate = 0;
            }
            //Stop increasing volume if volume is at max
            else if(menuMusic.volume == actualMusicVolume)
            {
                menuRate = 0;
            }
        }
        //In-game between waves update
        if (betweenRate != 0)
        {
            //Start music if volume is 0
            if (betweenMusic.volume == 0)
            {
                betweenMusic.Play();
            }
            bool aboveMax = menuRate < 0 && menuMusic.volume > actualMusicVolume;

            //Change volume based on whether it is stopping or starting
            betweenMusic.volume = Mathf.Clamp(betweenMusic.volume + betweenRate / musicFadeTime * Time.deltaTime * actualMusicVolume, 0, actualMusicVolume);

            //If decreasing to new max volume is finished
            if (aboveMax && menuMusic.volume < actualMusicVolume)
            {
                betweenRate = 0;
                betweenMusic.volume = actualMusicVolume;
            }
            //Stop music if volume is zero
            else if (betweenMusic.volume == 0)
            {
                betweenMusic.Stop(); 
                betweenRate = 0;
            }
            //Stop increasing volume if volume is at max
            else if (menuMusic.volume == actualMusicVolume)
            {
                betweenRate = 0;
            }
        }
        //In-game battle music updates
        for(int i = 0; i < battleMusics.Length; i++)
        {
            if (battleRates[i] != 0)
            {
                //Start music if volume is 0
                if (battleMusics[i].volume == 0)
                {
                    battleMusics[i].Play();
                }
                bool aboveMax = menuRate < 0 && menuMusic.volume > actualMusicVolume;

                //Change volume based on whether it is stopping or starting
                battleMusics[i].volume = Mathf.Clamp(battleMusics[i].volume + battleRates[i] / musicFadeTime * Time.deltaTime * actualMusicVolume, 0, actualMusicVolume);
                
                //If decreasing to new max volume is finished
                if(aboveMax && menuMusic.volume < actualMusicVolume)
                {
                    battleRates[i] = 0;
                    battleMusics[i].volume = actualMusicVolume;
                }
                //Stop music if volume is zero
                else if (battleMusics[i].volume == 0)
                {
                    battleMusics[i].Stop();
                    battleRates[i] = 0;
                }
                //Stop increasing volume if volume is at max
                else if (menuMusic.volume == actualMusicVolume)
                {
                    battleRates[i] = 0;
                }
            }
        }
    }


    //Start the menu music
    public void PlayMenu()
    {
        //If music fade is enabled do it
        if (musicFadeTime > 0)
        {
            menuRate = 1;
        }
        //If it is not set it to max volume immediately
        else
        {
            menuMusic.volume = (musicVolume / 100) * (masterVolume / 100);
            menuMusic.Play();
        }
    }

    //Stop the menu music
    public void StopMenu()
    {
        //If music fade is enabled do it
        if (musicFadeTime > 0)
        {
            menuRate = -1;
        }
        //If it is not stop it immediately
        else
        {
            menuMusic.volume = 0;
            menuMusic.Stop();
        }
    }

    //Start the music for in-game between waves
    public void PlayBetween()
    {
        //If music fade is enable do it
        if (musicFadeTime > 0)
        {
            betweenRate = 1;
        }
        //If it is not set it to max volume immediately
        else
        {
            betweenMusic.volume = (musicVolume / 100) * (masterVolume / 100);
            betweenMusic.Play();
        }
    }

    //Stop the music for in-game between waves
    public void StopBetween()
    {
        //If music fade is enabled do it
        if (musicFadeTime > 0)
        {
            betweenRate = -1;
        }
        //If it is not stop it immediately
        else
        {
            betweenMusic.volume = 0;
            betweenMusic.Stop();
        }
    }

    //Start battle music
    public void PlayBattle()
    {
        //Determine which music to play based off of which bonuses enemies get (determine by wave number)
        selectedBattle = 0;
        if (GameManager.Instance.wave != 0)
        {
            selectedBattle += GameManager.Instance.wave % 5 == 0 ? 1 : 0;
            selectedBattle += GameManager.Instance.wave % 7 == 0 ? 1 : 0;
            selectedBattle += GameManager.Instance.wave % 9 == 0 ? 1 : 0;
            selectedBattle += GameManager.Instance.wave % 11 == 0 ? 1 : 0;
            selectedBattle += GameManager.Instance.wave % 13 == 0 ? 1 : 0;
        }

        //If music fade is enabled do it
        if (musicFadeTime > 0)
        {
            battleRates[selectedBattle] = 1;
        }
        //If it is not set it to max volume immediately
        else
        {
            battleMusics[selectedBattle].volume = (musicVolume / 100) * (masterVolume / 100);
            battleMusics[selectedBattle].Play();
        }
    }

    //Stops the currently playing battle music
    public void StopBattle()
    {
        //If music fade is enabled do it
        if (musicFadeTime > 0)
        {
            battleRates[selectedBattle] = -1;
        }
        //If it is not stop it immediately
        else
        {
            battleMusics[selectedBattle].volume = 0;
            battleMusics[selectedBattle].Stop();
        }
    }

    //Set the master volume to a new value
    public void UpdateMasterVolume(float newVolume)
    {
        //If music fades fade to new value
        if (musicFadeTime > 0)
        {
            menuRate = masterVolume < newVolume ? 1 : -1;
            masterVolume = newVolume;
        }
        //If music doesn't fade immediately set
        else
        {
            masterVolume = newVolume;
            menuMusic.volume = (musicVolume / 100) * (masterVolume / 100);
        }
        //Save volume
        PlayerPrefs.SetFloat("MasterVolume", newVolume);
        PlayerPrefs.Save();
    }

    //Set the music volume to a new value
    public void UpdateMusicVolume(float newVolume)
    {
        //If music fades fade to new volume
        if (musicFadeTime > 0)
        {
            menuRate = musicVolume < newVolume ? 1 : -1;
            musicVolume = newVolume;
        }
        //If music doesn't fade immediately set
        else
        {
            musicVolume = newVolume;
            menuMusic.volume = (musicVolume / 100) * (masterVolume / 100);
        }
        //Save volume
        PlayerPrefs.SetFloat("MusicVolume", newVolume);
        PlayerPrefs.Save();
    }

    //Play UI click sound effect
    public void PlayClick()
    {
        //Set volume
        clickFX.volume = (sfxVolume / 100) * (masterVolume / 100);

        //Play sound
        clickFX.PlayOneShot(clickFX.clip);
    }

    //Set the volume of the sound effects
    public void UpdateSFXVolume(float newVolume)
    {
        sfxVolume = newVolume;
        clickFX.volume = (newVolume / 100) * (masterVolume / 100);
        PlayerPrefs.SetFloat("SFXVolume", newVolume);
        PlayerPrefs.Save();
    }
}
