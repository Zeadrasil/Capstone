using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BasicUtils
{
    public static string TranslateKey(KeyCode keyCode)
    {
        switch(keyCode)
        {
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
            case KeyCode.Return:
                {
                    return "Enter";
                }
            default:
                {
                    return keyCode.ToString();
                }
        }
    }
}
