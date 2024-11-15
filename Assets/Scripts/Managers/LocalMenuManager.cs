using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//MenuManager manages the main menu before you start a run
public class LocalMenuManager : MonoBehaviour
{
    [SerializeField] Canvas pauseMenu;
    [SerializeField] Canvas turretMenu;
    [SerializeField] Canvas repairMenu;
    [SerializeField] Canvas wallMenu;
    [SerializeField] Canvas resourceMenu;
    [SerializeField] Canvas buildingMenu;
    [SerializeField] Canvas sellPanel;

    private void Start()
    {
        MenuManager.Instance.pauseMenu = pauseMenu;
        MenuManager.Instance.turretMenu = turretMenu;
        MenuManager.Instance.repairMenu = repairMenu;
        MenuManager.Instance.wallMenu = wallMenu;
        MenuManager.Instance.resourceMenu = resourceMenu;
        MenuManager.Instance.buildingMenu = buildingMenu;
        MenuManager.Instance.sellPanel = sellPanel;
        UnPause();
    }

    //Relay for buttons in main scene to create a save
    public void Save()
    {
        FileManager.Instance.Save();
    }

    //Relay for buttons in main scene to quit the map
    public void Quit()
    {
        Save();
        MenuManager.Instance.Return();
    }

    //Relay for buttons in main scene to unpause
    public void UnPause()
    {
        MenuManager.Instance.UnPause();
    }

    //Relay for buttons in main scene to load a new map after leaving, not finished yet cause its weird
    public void Load()
    {
        Quit();
        //MenuManager.Instance.LoadData();
    }
}
