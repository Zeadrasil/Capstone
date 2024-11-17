using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

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

    //Volume data
    public int masterVolume = 100;
    public int musicVolume = 100;

    // Start is called before the first frame update
    private void Start()
    {
        //Start the menu music since that is where you load in
        PlayMenu();
    }

    //Called every frame
    private void Update()
    {
        //Maximum volume calculation
        float actualVolume = 100f / musicVolume * 100f / masterVolume;

        //Menu update
        if (menuRate != 0)
        {
            //Start music if volume is 0
            if (menuMusic.volume == 0)
            {
                menuMusic.Play();
            }
            //Change volume based on whether it is stopping or starting
            menuMusic.volume = Mathf.Clamp(menuMusic.volume + menuRate * 0.2f * Time.deltaTime * actualVolume, 0, actualVolume);
            
            //Stop music if volume is zero
            if (menuMusic.volume == 0)
            {
                menuMusic.Stop();
                menuRate = 0;
            }
            //Stop increasing volume if volume is at max
            else if(menuMusic.volume == actualVolume)
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
            //Change volume based on whether it is stopping or starting
            betweenMusic.volume = Mathf.Clamp(betweenMusic.volume + betweenRate * 0.2f * Time.deltaTime * actualVolume, 0, actualVolume);
            
            //Stop music if volume is zero
            if (betweenMusic.volume == 0)
            {
                betweenMusic.Stop(); 
                betweenRate = 0;
            }
            //Stop increasing volume if volume is at max
            else if (menuMusic.volume == actualVolume)
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
                //Change volume based on whether it is stopping or starting
                battleMusics[i].volume = Mathf.Clamp(battleMusics[i].volume + battleRates[i] * 0.2f * Time.deltaTime * actualVolume, 0, actualVolume);
                
                //Stop music if volume is zero
                if (battleMusics[i].volume == 0)
                {
                    battleMusics[i].Stop();
                    battleRates[i] = 0;
                }
                //Stop increasing volume if volume is at max
                else if (menuMusic.volume == actualVolume)
                {
                    battleRates[i] = 0;
                }
            }
        }
    }


    //Start the menu music
    public void PlayMenu()
    {
        menuRate = 1;
    }

    //Stop the menu music
    public void StopMenu()
    {
        menuRate = -1;
    }

    //Start the music for in-game between waves
    public void PlayBetween()
    {
        betweenRate = 1;
    }

    //Stop the music for in-game between waves
    public void StopBetween()
    {
        betweenRate = -1;
    }

    //Start battle music
    public void PlayBattle()
    {
        //Determine which music to play based off of which bonuses enemies get (determine by wave number)
        selectedBattle = 0;
        selectedBattle += GameManager.Instance.wave % 5 == 0 ? 1 : 0;
        selectedBattle += GameManager.Instance.wave % 7 == 0 ? 1 : 0;
        selectedBattle += GameManager.Instance.wave % 9 == 0 ? 1 : 0;
        selectedBattle += GameManager.Instance.wave % 11 == 0 ? 1 : 0;
        selectedBattle += GameManager.Instance.wave % 13 == 0 ? 1 : 0;

        //Play the appropriate battle theme
        battleRates[selectedBattle] = 1;
    }

    //Stops the currently playing battle music
    public void StopBattle()
    {
        battleRates[selectedBattle] = -1;
    }
}
