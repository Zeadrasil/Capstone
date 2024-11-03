using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

//Manages saving and loading
public class FileManager : Singleton<FileManager>
{
    //Attempts to save a game
    public string Save()
    {
        try
        {
            //Gets the default save path for the application
            string defaultPath = Path.Combine(Application.dataPath, "Saves");

            //Ensures that the default save path actually exists
            Directory.CreateDirectory(defaultPath);

            //Gets the path that the user wishes to save their game at
            string fullPath = StandaloneFileBrowser.SaveFilePanel("Save Game", defaultPath, $"{DateTime.Now.Year.ToString("0000")}-{DateTime.Now.Month.ToString("00")}-{DateTime.Now.Day.ToString("00")}-{DateTime.Now.Hour.ToString("00")}-{DateTime.Now.Minute.ToString("00")}-{DateTime.Now.Second.ToString("00")}", ".json");

            //If not canceled, save
            if (fullPath.Length > 0)
            {
                //Transforms the save data into JSON format
                string data = JsonUtility.ToJson(GameManager.Instance.GetSaveData(), true);

                //Creates a writer to write the data
                StreamWriter sw = new StreamWriter(new FileStream(fullPath, FileMode.Create));

                //Writes data
                sw.Write(data);

                //Ensure that file saves properly
                sw.Close();

                //Returns a string indicating no errors
                return "Save Successful";
            }
            //If canceled return a string indicating so
            return "Save Canceled";
        }
        //If there is some form of exception, return the message so that it is known what the problem was
        catch(Exception ex)
        {
            return $"Save Error:\n{ex.Message}";
        }
    }

    public GameData Load()
    {
        try
        {
            //Gets the default save path for the application
            string defaultPath = Path.Combine(Application.dataPath, "Saves");

            //Ensures that the default save path actually exists
            Directory.CreateDirectory(defaultPath);

            //Get the actual path to the save that you are loading
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Load Save", defaultPath, ".json", false);

            //If not canceled, load
            if (paths.Length > 0)
            {
                //Creates a reader to read the data
                StreamReader sr = new StreamReader(new FileStream(paths[0], FileMode.Open));

                //Reads the data
                string data = sr.ReadToEnd();

                //Parses the data from JSON format and returns it
                return JsonUtility.FromJson<GameData>(data);
            }
            //If canceled return null
            return null;
        }
        //If error happens return null
        catch(Exception ex)
        {
            return null;
        }
    }
}
