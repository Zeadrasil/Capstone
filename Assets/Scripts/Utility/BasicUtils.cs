using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class containing various helper functions
public static class BasicUtils
{
    //Keeps track of stuff for RNG
    public static uint generatedNumbers = 0;
    private static System.Random random = new System.Random();


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
        return min + ((max - min) * random.Next() / (float)int.MaxValue);
    }

    //Wraps around the RNG function in order to have saving and loading not affect RNG
    public static int WrappedRandomRange(int min, int max)
    {
        generatedNumbers++;
        return random.Next(min, max);
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
        random = new System.Random(seed);
        generatedNumbers = 0;
    }

    //Draws line from a given point to another point
    public static void DrawLine(Vector3 start, Vector3 end, Color startColor, Color endColor, float duration = 0.05f, float width = 0.1f, bool fade = false)
    {
        //Create gameobject to store line
        GameObject gameObj = new GameObject();
        LineRenderer lineRenderer = gameObj.AddComponent<LineRenderer>();

        //If you want the line to gradually fade, make it do so
        if (fade)
        {
            FadingRenderer fader = gameObj.AddComponent<FadingRenderer>();
            fader.renderer = lineRenderer;
            fader.fadeDuration = duration;
        }

        //Set colors
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;

        //Solution for the magenta line issue from https://forum.unity.com/threads/cant-set-color-for-linerenderer-always-comes-out-as-magenta-or-black.968447/
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        //End solution

        //Sets rendering positions
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        //Sets width
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        //Queues Destruction
        GameObject.Destroy(gameObj, duration);
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.05f, float width = 0.1f, bool fade = false)
    {
        DrawLine(start, end, color, color, duration, width, fade);
    }

    ////Allows you to queue destruction of gameobjects more effectively than GameObject.Destroy(object, time)
    //public static IEnumerator DestroyLater(GameObject obj, float initialTime, float delay)
    //{
    //    while (initialTime + delay > Time.time)
    //    {
    //        yield return new WaitForSeconds(delay);
    //    }
    //    GameObject.Destroy(obj);
    //    yield return null;
    //}
}
