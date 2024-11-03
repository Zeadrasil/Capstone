using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class containing various helper functions
public static class BasicUtils
{
    //Keeps track of stuff for RNG
    public static uint generatedNumbers = 0;

    //Translates a KeyCode into a more readable string than the raw implementation of its ToString()
    public static string TranslateKey(KeyCode keyCode)
    {
        switch(keyCode)
        {
            //Number keys use their number
            case KeyCode.Alpha0:
                {
                    return "0";
                }
            case KeyCode.Alpha1:
                {
                    return "1";
                }
            case KeyCode.Alpha2:
                {
                    return "2";
                }
            case KeyCode.Alpha3:
                {
                    return "3";
                }
            case KeyCode.Alpha4:
                {
                    return "4";
                }
            case KeyCode.Alpha5:
                {
                    return "5";
                }
            case KeyCode.Alpha6:
                {
                    return "6";
                }
            case KeyCode.Alpha7:
                {
                    return "7";
                }
            case KeyCode.Alpha8:
                {
                    return "8";
                }
            case KeyCode.Alpha9:
                {
                    return "9";
                }
            //Cut off the arrow part of arrow keys
            case KeyCode.DownArrow:
                {
                    return "Down";
                }
            case KeyCode.UpArrow:
                {
                    return "Up";
                }
            case KeyCode.RightArrow:
                {
                    return "Right";
                }
            case KeyCode.LeftArrow:
                {
                    return "Left";
                }
            //Nobody calls it return, they all call it enter
            case KeyCode.Return:
                {
                    return "Enter";
                }
            //Most are fine, so itll just defualt to the ToString()
            default:
                {
                    return keyCode.ToString();
                }
        }
    }

    //Wraps around the RNG function in order to have saving and loading not affect RNG
    public static float WrappedRandomRange(float min, float max)
    {
        generatedNumbers++;
        return UnityEngine.Random.Range(min, max);
    }

    //Wraps around the RNG function in order to have saving and loading not affect RNG
    public static int WrappedRandomRange(int min, int max)
    {
        generatedNumbers++;
        return UnityEngine.Random.Range(min, max);
    }

    //Spams random numbers until you reachthe same position in the RNG sequence as you the given position
    public static void SpamRNGUntil(uint goal)
    {
        while(generatedNumbers < goal)
        {
            WrappedRandomRange(0, goal);
        }
    }

    //Wraps around the RNG function in order to ensure that the generated number tracking is accurate
    public static void WrappedInitState(int seed)
    {
        UnityEngine.Random.InitState(seed);
        generatedNumbers = 0;
    }
}
