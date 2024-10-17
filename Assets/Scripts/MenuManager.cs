using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas optionsMenu;
    [SerializeField] Canvas newGameMenu;
    [SerializeField] TMP_Text forwardText;
    [SerializeField] TMP_Text backText;
    [SerializeField] TMP_Text leftText;
    [SerializeField] TMP_Text rightText;
    [SerializeField] TMP_Text tierOneTurretText;
    [SerializeField] TMP_Text tierOneExtractorText;
    [SerializeField] TMP_Text nextWaveText;
    [SerializeField] TMP_Text cancelConstructionText;

    [SerializeField] TMP_InputField simplifiedSeedInput;
    [SerializeField] Slider simplifiedSeedSlider;
    [SerializeField] TMP_InputField enemyDifficultyInput;
    [SerializeField] Slider enemyDifficultySlider;
    [SerializeField] TMP_InputField playerPowerInput;
    [SerializeField] Slider playerPowerSlider;
    [SerializeField] TMP_InputField playerIncomeInput;
    [SerializeField] Slider playerIncomeSlider;
    [SerializeField] TMP_InputField playerCostsInput;
    [SerializeField] Slider playerCostsSlider;

    bool freshSimplifiedSeedUpdate = true;
    bool freshEnemyDifficultyUpdate = true;
    bool freshPlayerPowerUpdate = true;
    bool freshPlayerIncomeUpdate = true;
    bool freshPlayerCostsUpdate = true;

    private GameManager gameManager;
    int controlToUpdate = -1;

    private void Start()
    {
        gameManager = GameManager.Instance;
        simplifiedSeedSlider.value = UnityEngine.Random.Range((float)int.MinValue, (float)int.MaxValue);
        gameManager.forwardKey = (KeyCode)PlayerPrefs.GetInt("forwardKey");
        if(gameManager.forwardKey == KeyCode.None )
        {
            gameManager.forwardKey = KeyCode.W;
        }
        gameManager.backKey = (KeyCode)PlayerPrefs.GetInt("backKey");
        if (gameManager.backKey == KeyCode.None)
        {
            gameManager.backKey = KeyCode.S;
        }
        gameManager.leftKey = (KeyCode)PlayerPrefs.GetInt("leftKey");
        if (gameManager.leftKey == KeyCode.None)
        {
            gameManager.leftKey = KeyCode.A;
        }
        gameManager.rightKey = (KeyCode)PlayerPrefs.GetInt("rightKey");
        if (gameManager.rightKey == KeyCode.None)
        {
            gameManager.rightKey = KeyCode.D;
        }
        gameManager.tierOneTurretKey = (KeyCode)PlayerPrefs.GetInt("tierOneTurretKey");
        if (gameManager.tierOneTurretKey == KeyCode.None)
        {
            gameManager.tierOneTurretKey = KeyCode.Alpha1;
        }
        gameManager.tierOneExtractorKey = (KeyCode)PlayerPrefs.GetInt("tierOneExtractorKey");
        if (gameManager.tierOneExtractorKey == KeyCode.None)
        {
            gameManager.tierOneExtractorKey = KeyCode.Alpha8;
        }
        gameManager.nextWaveKey = (KeyCode)PlayerPrefs.GetInt("nextWaveKey");
        if (gameManager.nextWaveKey == KeyCode.None)
        {
            gameManager.nextWaveKey = KeyCode.N;
        }
        gameManager.cancelConstructionKey = (KeyCode)PlayerPrefs.GetInt("cancelConstructionKey");
        if (gameManager.cancelConstructionKey == KeyCode.None)
        {
            gameManager.cancelConstructionKey = KeyCode.Escape;
        }
        if(PlayerPrefs.HasKey("previousEnemyDifficulty"))
        {
            enemyDifficultySlider.value = PlayerPrefs.GetFloat("previousEnemyDifficulty");
        }
        if(PlayerPrefs.HasKey("previousPlayerPower"))
        {
            playerPowerSlider.value = PlayerPrefs.GetFloat("previousPlayerPower");
        }
        if(PlayerPrefs.HasKey("previousPlayerIncome"))
        {
            playerIncomeSlider.value = PlayerPrefs.GetFloat("previousPlayerIncome");
        }
        if(PlayerPrefs.HasKey("previousPlayerCosts"))
        {
            playerCostsSlider.value = PlayerPrefs.GetFloat("previousPlayerCosts");
        }

    }

    private void Update()
    {
        if(controlToUpdate != -1)
        {
            foreach(KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if(Input.GetKeyDown(key) && ((int)key < (int)KeyCode.Mouse0 || (int)key > (int)KeyCode.Mouse2))
                {
                    switch(controlToUpdate)
                    {
                        case 0:
                            {
                                gameManager.forwardKey = key;
                                PlayerPrefs.SetInt("forwardKey", (int)key);
                                forwardText.text = key.ToString();
                                break;
                            }
                        case 1:
                            {
                                gameManager.backKey = key;
                                PlayerPrefs.SetInt("backKey", (int)key);
                                backText.text = key.ToString();
                                break;
                            }
                        case 2:
                            {
                                gameManager.leftKey = key;
                                PlayerPrefs.SetInt("leftKey", (int)key);
                                leftText.text = key.ToString();
                                break;
                            }
                        case 3:
                            {
                                gameManager.rightKey = key;
                                PlayerPrefs.SetInt("rightKey", (int)key);
                                rightText.text = key.ToString();
                                break;
                            }
                        case 4:
                            {
                                gameManager.tierOneTurretKey = key;
                                PlayerPrefs.SetInt("tierOneTurretKey", (int)key);
                                tierOneTurretText.text = key.ToString();
                                break;
                            }
                        case 11:
                            {
                                gameManager.tierOneExtractorKey = key;
                                PlayerPrefs.SetInt("tierOneExtractorKey", (int)key);
                                tierOneExtractorText.text = key.ToString();
                                break;
                            }
                        case 14:
                            {
                                gameManager.nextWaveKey = key;
                                PlayerPrefs.SetInt("rightKey", (int)key);
                                nextWaveText.text = key.ToString();
                                break;
                            }
                        case 15:
                            {
                                gameManager.cancelConstructionKey = key;
                                PlayerPrefs.SetInt("cancelConstructionKey", (int)key);
                                cancelConstructionText.text = key.ToString();
                                break;
                            }
                    }
                    controlToUpdate = -1;
                    PlayerPrefs.Save();
                }
            }
        }
    }

    public void Play()
    {
        newGameMenu.enabled = false;
        SceneManager.LoadScene("MainScene");
        GameManager.Instance.queueInitialize = 2;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void EnterOptions()
    {
        optionsMenu.enabled = true;
        mainMenu.enabled = false;
        newGameMenu.enabled = false;
        forwardText.text = gameManager.forwardKey.ToString();
        backText.text = gameManager.backKey.ToString();
        leftText.text = gameManager.leftKey.ToString();
        rightText.text = gameManager.rightKey.ToString();
        tierOneTurretText.text = gameManager.tierOneTurretKey.ToString();
        tierOneExtractorText.text = gameManager.tierOneExtractorKey.ToString();
        nextWaveText.text = gameManager.nextWaveKey.ToString();
        cancelConstructionText.text = gameManager.cancelConstructionKey.ToString();
    }
    public void ExitOptions()
    {
        optionsMenu.enabled = false;
        mainMenu.enabled = true;
        newGameMenu.enabled = false;
        controlToUpdate = -1;
    }
    public void UpdateForwardKey()
    {
        forwardText.text = "Press any key";
        controlToUpdate = 0;
    }
    public void UpdateBackKey()
    {
        backText.text = "Press any key";
        controlToUpdate = 1;
    }
    public void UpdateLeftKey()
    {
        leftText.text = "Press any key";
        controlToUpdate = 2;
    }
    public void UpdateRightKey()
    {
        rightText.text = "Press any key";
        controlToUpdate = 3;
    }
    public void UpdateTierOneTurretKey()
    {
        tierOneTurretText.text = "Press any key";
        controlToUpdate = 4;
    }
    public void UpdateTierOneExtractorKey()
    {
        tierOneExtractorText.text = "Press any key";
        controlToUpdate = 11;
    }
    public void UpdateNextWaveKey()
    {
        nextWaveText.text = "Press any key";
        controlToUpdate = 14;
    }
    public void UpdateCancelConstructionKey()
    {
        cancelConstructionText.text = "Press any key";
        controlToUpdate = 15;
    }
    public void ExitStartMenu()
    {
        optionsMenu.enabled = false;
        mainMenu.enabled = true;
        newGameMenu.enabled = false;
    }
    public void EnterStartMenu()
    {
        optionsMenu.enabled = false;
        mainMenu.enabled = false;
        newGameMenu.enabled = true;
    }
    public void UpdateSimplifiedSeedFromSlider()
    {
        if(freshSimplifiedSeedUpdate)
        {
            freshSimplifiedSeedUpdate = false;
            gameManager.simplifiedSeed = simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value;
            simplifiedSeedInput.text = (simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value).ToString();
            freshSimplifiedSeedUpdate = true;
        }
    }
    public void UpdateSimplifiedSeedFromInput()
    {
        if(freshSimplifiedSeedUpdate && simplifiedSeedInput.text.Length > 0 && simplifiedSeedInput.text != "-" && simplifiedSeedInput.text != ".")
        {
            freshSimplifiedSeedUpdate = false;
            simplifiedSeedSlider.value = float.Parse(simplifiedSeedInput.text);
            gameManager.simplifiedSeed = simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value;
            freshSimplifiedSeedUpdate = true;
        }
    }
    public void FinishUpdatingSimplifiedSeedFromInput()
    {
        freshSimplifiedSeedUpdate = false;
        if(simplifiedSeedInput.text.Length == 0 || simplifiedSeedInput.text == "-" || simplifiedSeedInput.text == "." || float.Parse(simplifiedSeedInput.text) > int.MaxValue || float.Parse(simplifiedSeedInput.text) < int.MinValue)
        {
            simplifiedSeedInput.text = (simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value).ToString();
        }
        freshSimplifiedSeedUpdate = true;
    }
    public void UpdateEnemyDifficultyFromSlider()
    {
        if (freshEnemyDifficultyUpdate)
        {
            freshEnemyDifficultyUpdate = false;
            gameManager.enemyDifficulty = enemyDifficultySlider.value;
            enemyDifficultyInput.text = enemyDifficultySlider.value.ToString();
            freshEnemyDifficultyUpdate = true;
        }
    }
    public void UpdateEnemyDifficultyFromInput()
    {
        if (freshEnemyDifficultyUpdate && enemyDifficultyInput.text.Length > 0 && enemyDifficultyInput.text != "-" && enemyDifficultyInput.text != ".")
        {
            freshEnemyDifficultyUpdate = false;
            enemyDifficultySlider.value = float.Parse(enemyDifficultyInput.text);
            gameManager.enemyDifficulty = enemyDifficultySlider.value;
            freshEnemyDifficultyUpdate = true;
        }
    }
    public void FinishUpdatingEnemyDifficultyFromInput()
    {
        freshEnemyDifficultyUpdate = false;
        if (float.Parse(enemyDifficultyInput.text) < enemyDifficultySlider.minValue || float.Parse(enemyDifficultyInput.text) > enemyDifficultySlider.maxValue || enemyDifficultyInput.text.Length == 0 || enemyDifficultyInput.text == "-" || enemyDifficultyInput.text == ".")
        {
            enemyDifficultyInput.text = enemyDifficultySlider.value.ToString();
        }
        freshEnemyDifficultyUpdate = true;
    }
    public void UpdatePlayerPowerFromSlider()
    {
        if (freshPlayerPowerUpdate)
        {
            freshPlayerPowerUpdate = false;
            gameManager.playerPower = playerPowerSlider.value;
            playerPowerInput.text = playerPowerSlider.value.ToString();
            freshPlayerPowerUpdate = true;
        }
    }
    public void UpdatePlayerPowerFromInput()
    {
        if (freshPlayerPowerUpdate && playerPowerInput.text.Length > 0 && playerPowerInput.text != "-" && playerPowerInput.text != ".")
        {
            freshPlayerPowerUpdate = false;
            playerPowerSlider.value = float.Parse(playerPowerInput.text);
            gameManager.playerPower = playerPowerSlider.value;
            freshPlayerPowerUpdate = true;
        }
    }
    public void FinishUpdatingPlayerPowerFromInput()
    {
        freshPlayerPowerUpdate = false;
        if (float.Parse(playerPowerInput.text) < playerPowerSlider.minValue || float.Parse(playerPowerInput.text) > playerPowerSlider.maxValue || playerPowerInput.text.Length == 0 || playerPowerInput.text == "-" || playerPowerInput.text == ".")
        {
            playerPowerInput.text = playerPowerSlider.value.ToString();
        }
        freshPlayerPowerUpdate = true;
    }
    public void UpdatePlayerIncomeFromSlider()
    {
        if (freshPlayerIncomeUpdate)
        {
            freshPlayerIncomeUpdate = false;
            gameManager.playerIncome = playerIncomeSlider.value;
            playerIncomeInput.text = playerIncomeSlider.value.ToString();
            freshPlayerIncomeUpdate = true;
        }
    }
    public void UpdatePlayerIncomeFromInput()
    {
        if (freshPlayerIncomeUpdate && playerIncomeInput.text.Length > 0 && playerIncomeInput.text != "-" && playerIncomeInput.text != ".")
        {
            freshPlayerIncomeUpdate = false;
            playerIncomeSlider.value = float.Parse(playerIncomeInput.text);
            gameManager.playerIncome = playerIncomeSlider.value;
            freshPlayerIncomeUpdate = true;
        }
    }
    public void FinishUpdatingPlayerIncomeFromInput()
    {
        freshPlayerIncomeUpdate = false;
        if (float.Parse(playerIncomeInput.text) < playerIncomeSlider.minValue || float.Parse(playerIncomeInput.text) > playerIncomeSlider.maxValue || playerIncomeInput.text.Length == 0 || playerIncomeInput.text == "-" || playerIncomeInput.text == ".")
        {
            playerIncomeInput.text = playerIncomeSlider.value.ToString();
        }
        freshPlayerIncomeUpdate = true;
    }
    public void UpdatePlayerCostsFromSlider()
    {
        if (freshPlayerCostsUpdate)
        {
            freshPlayerCostsUpdate = false;
            gameManager.playerCosts = playerCostsSlider.value;
            playerCostsInput.text = playerCostsSlider.value.ToString();
            freshPlayerCostsUpdate = true;
        }
    }
    public void UpdatePlayerCostsFromInput()
    {
        if (freshPlayerCostsUpdate && playerCostsInput.text.Length > 0 && playerCostsInput.text != "-" && playerCostsInput.text != ".")
        {
            freshPlayerCostsUpdate = false;
            playerCostsSlider.value = float.Parse(playerCostsInput.text);
            gameManager.playerCosts = playerCostsSlider.value;
            freshPlayerCostsUpdate = true;
        }
    }
    public void FinishUpdatingPlayerCostsFromInput()
    {
        freshPlayerCostsUpdate = false;
        if (float.Parse(playerCostsInput.text) < playerCostsSlider.minValue || float.Parse(playerCostsInput.text) > playerCostsSlider.maxValue || playerCostsInput.text.Length == 0 || playerCostsInput.text == "-" || playerCostsInput.text == ".")
        {
            playerCostsInput.text = playerCostsSlider.value.ToString();
        }
        freshPlayerCostsUpdate = true;
    }
}
