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
            string fullPath = EditorUtility.SaveFilePanel("Save Game", defaultPath, DateTime.Now.ToString(), ".json");
            
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
        //If there is some form of exception, return the message so that it is known what the problem was
        catch(Exception ex)
        {
            return ex.Message;
        }
    }
}
