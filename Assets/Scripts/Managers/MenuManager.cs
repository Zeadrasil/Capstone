using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//MenuManager manages the main menu before you start a run
public class MenuManager : Singleton<MenuManager>
{
    //Major menus
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas optionsMenu;
    [SerializeField] Canvas newGameMenu;

    //Hotkey labels
    [SerializeField] TMP_Text cameraForwardText;
    [SerializeField] TMP_Text cameraBackText;
    [SerializeField] TMP_Text cameraLeftText;
    [SerializeField] TMP_Text cameraRightText;
    [SerializeField] TMP_Text tierOneTurretText;
    [SerializeField] TMP_Text tierOneRepairStationText;
    [SerializeField] TMP_Text tierOneWallText;
    [SerializeField] TMP_Text tierOneExtractorText;
    [SerializeField] TMP_Text nextWaveText;
    [SerializeField] TMP_Text cancelText;
    [SerializeField] TMP_Text confirmText;
    [SerializeField] TMP_Text selectionUpText;
    [SerializeField] TMP_Text selectionDownText;
    [SerializeField] TMP_Text selectionLeftText;
    [SerializeField] TMP_Text selectionRightText;
    [SerializeField] TMP_Text sellText;
    private TMP_Text[] hotkeyTexts;

    //New game settings controls
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

    //Holder variables to avoid update loops
    bool freshSimplifiedSeedUpdate = true;
    bool freshEnemyDifficultyUpdate = true;
    bool freshPlayerPowerUpdate = true;
    bool freshPlayerIncomeUpdate = true;
    bool freshPlayerCostsUpdate = true;

    //GameManager reference to reduce typing length
    private GameManager gameManager;

    //Controls which hotkey you are changing
    int controlToUpdate = -1;

    //Called just before first Update() call
    private void Start()
    {
        hotkeyTexts = new TMP_Text[] {cameraForwardText, cameraBackText, cameraLeftText, cameraRightText, tierOneTurretText, null, null, tierOneRepairStationText,  null, tierOneWallText, null, tierOneExtractorText, null, null, nextWaveText, cancelText, confirmText, selectionUpText, selectionDownText, selectionLeftText, selectionRightText, sellText};
        //Initializes manager reference
        gameManager = GameManager.Instance;

        //Gets a random initial seed so that you don't have to come up with a new one by yourself
        simplifiedSeedSlider.value = UnityEngine.Random.Range((float)int.MinValue, (float)int.MaxValue);

        //Load preferred camera up key
        gameManager.forwardKey = (KeyCode)PlayerPrefs.GetInt("cameraForwardKey");
        if(gameManager.forwardKey == KeyCode.None )
        {
            gameManager.forwardKey = KeyCode.W;
        }
        //Load preferred camera down key
        gameManager.backKey = (KeyCode)PlayerPrefs.GetInt("cameraBackKey");
        if (gameManager.backKey == KeyCode.None)
        {
            gameManager.backKey = KeyCode.S;
        }
        //Load preferred camera left key
        gameManager.leftKey = (KeyCode)PlayerPrefs.GetInt("cameraLeftKey");
        if (gameManager.leftKey == KeyCode.None)
        {
            gameManager.leftKey = KeyCode.A;
        }
        //Load preferred camera right key
        gameManager.rightKey = (KeyCode)PlayerPrefs.GetInt("cameraRightKey");
        if (gameManager.rightKey == KeyCode.None)
        {
            gameManager.rightKey = KeyCode.D;
        }
        //Load preferred tier one turret hotkey
        gameManager.tierOneTurretKey = (KeyCode)PlayerPrefs.GetInt("tierOneTurretKey");
        if (gameManager.tierOneTurretKey == KeyCode.None)
        {
            gameManager.tierOneTurretKey = KeyCode.Alpha1;
        }
        //Load preferred tier one repair station hotkey
        gameManager.tierOneRepairKey = (KeyCode)PlayerPrefs.GetInt("tierOneRepairKey");
        if (gameManager.tierOneRepairKey == KeyCode.None)
        {
            gameManager.tierOneRepairKey = KeyCode.Alpha4;
        }
        //Load preferred tier one wall hotkey
        gameManager.tierOneWallKey = (KeyCode)PlayerPrefs.GetInt("tierOneWallKey");
        if (gameManager.tierOneWallKey == KeyCode.None)
        {
            gameManager.tierOneWallKey = KeyCode.Alpha6;
        }
        //Load preferred tier one extractor hotkey
        gameManager.tierOneExtractorKey = (KeyCode)PlayerPrefs.GetInt("tierOneExtractorKey");
        if (gameManager.tierOneExtractorKey == KeyCode.None)
        {
            gameManager.tierOneExtractorKey = KeyCode.Alpha8;
        }
        //Load preferred next wave hotkey
        gameManager.nextWaveKey = (KeyCode)PlayerPrefs.GetInt("nextWaveKey");
        if (gameManager.nextWaveKey == KeyCode.None)
        {
            gameManager.nextWaveKey = KeyCode.Space;
        }
        //Load preferred cancel key
        gameManager.cancelKey = (KeyCode)PlayerPrefs.GetInt("cancelKey");
        if (gameManager.cancelKey == KeyCode.None)
        {
            gameManager.cancelKey = KeyCode.Escape;
        }
        //Load preferred confirm key
        gameManager.confirmKey = (KeyCode)PlayerPrefs.GetInt("confirmKey");
        if (gameManager.confirmKey == KeyCode.None)
        {
            gameManager.confirmKey = KeyCode.Return;
        }
        //Load preferred selection up key
        gameManager.moveSelectionUpKey = (KeyCode)PlayerPrefs.GetInt("selectionUpKey");
        if (gameManager.moveSelectionUpKey == KeyCode.None)
        {
            gameManager.moveSelectionUpKey = KeyCode.UpArrow;
        }
        //Load preferred selection down key
        gameManager.moveSelectionDownKey = (KeyCode)PlayerPrefs.GetInt("selectionDownKey");
        if (gameManager.moveSelectionDownKey == KeyCode.None)
        {
            gameManager.moveSelectionDownKey = KeyCode.DownArrow;
        }
        //Load preferred selection left key
        gameManager.moveSelectionLeftKey = (KeyCode)PlayerPrefs.GetInt("selectionLeftKey");
        if (gameManager.moveSelectionLeftKey == KeyCode.None)
        {
            gameManager.moveSelectionLeftKey = KeyCode.LeftArrow;
        }
        //Load preferred selection right key
        gameManager.moveSelectionRightKey = (KeyCode)PlayerPrefs.GetInt("selectionRightKey");
        if (gameManager.moveSelectionRightKey == KeyCode.None)
        {
            gameManager.moveSelectionRightKey = KeyCode.RightArrow;
        }
        //Load preferred sell key
        gameManager.sellKey = (KeyCode)PlayerPrefs.GetInt("sellKey");
        if (gameManager.sellKey == KeyCode.None)
        {
            gameManager.sellKey = KeyCode.Backspace;
        }
        //Load saved enemy difficulty
        if (PlayerPrefs.HasKey("previousEnemyDifficulty"))
        {
            enemyDifficultySlider.value = PlayerPrefs.GetFloat("previousEnemyDifficulty");
        }
        //Load saved player power
        if(PlayerPrefs.HasKey("previousPlayerPower"))
        {
            playerPowerSlider.value = PlayerPrefs.GetFloat("previousPlayerPower");
        }
        //Load saved player income
        if(PlayerPrefs.HasKey("previousPlayerIncome"))
        {
            playerIncomeSlider.value = PlayerPrefs.GetFloat("previousPlayerIncome");
        }
        //Load saved player costs
        if(PlayerPrefs.HasKey("previousPlayerCosts"))
        {
            playerCostsSlider.value = PlayerPrefs.GetFloat("previousPlayerCosts");
        }
    }

    //Called every frame
    private void Update()
    {
        //Skip if not updating a hotkey
        if(controlToUpdate != -1)
        {
            //Goes through every single key you can press
            foreach(KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                //If the key is pressed, change the appropriate action to have it as its hotkey
                if(Input.GetKeyDown(key) && ((int)key < (int)KeyCode.Mouse0 || (int)key > (int)KeyCode.Mouse2))
                {
                    //Identifies which control to change
                    switch(controlToUpdate)
                    {
                        //Camera up
                        case 0:
                            {
                                //Sets the key in the GameManager
                                gameManager.forwardKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("cameraForwardKey", (int)key);

                                //Updates the text to display the new key
                                cameraForwardText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Camera down
                        case 1:
                            {
                                //Sets the key in the GameManager
                                gameManager.backKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("cameraBackKey", (int)key);

                                //Updates the text to display the new key
                                cameraBackText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Camera left
                        case 2:
                            {
                                //Sets the key in the GameManager
                                gameManager.leftKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("cameraLeftKey", (int)key);

                                //Updates the text to display the new key
                                cameraLeftText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Camera right
                        case 3:
                            {
                                //Sets the key in the GameManager
                                gameManager.rightKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("cameraRightKey", (int)key);

                                //Updates the text to display the new key
                                cameraRightText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Tier one turret
                        case 4:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierOneTurretKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierOneTurretKey", (int)key);

                                //Updates the text to display the new key
                                tierOneTurretText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Tier one repair station
                        case 7:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierOneRepairKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierOneRepairKey", (int)key);

                                //Updates the text to display the new key
                                tierOneRepairStationText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Tier one wall
                        case 9:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierOneWallKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierOneWallKey", (int)key);

                                //Updates the text to display the new key
                                tierOneWallText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Tier one extractor
                        case 11:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierOneExtractorKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierOneExtractorKey", (int)key);

                                //Updates the text to display the new key
                                tierOneExtractorText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Next wave
                        case 14:
                            {
                                //Sets the key in the GameManager
                                gameManager.nextWaveKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("newWaveKey", (int)key);

                                //Updates the text to display the new key
                                nextWaveText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Cancel
                        case 15:
                            {
                                //Sets the key in the GameManager
                                gameManager.cancelKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("cancelKey", (int)key);

                                //Updates the text to display the new key
                                cancelText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Confirm
                        case 16:
                            {
                                //Sets the key in the GameManager
                                gameManager.confirmKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("confirmKey", (int)key);

                                //Updates the text to display the new key
                                confirmText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Selection up
                        case 17:
                            {
                                //Sets the key in the GameManager
                                gameManager.moveSelectionUpKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("selectionUpKey", (int)key);

                                //Updates the text to display the new key
                                selectionUpText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Selection down
                        case 18:
                            {
                                //Sets the key in the GameManager
                                gameManager.moveSelectionDownKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("selectionDownKey", (int)key);

                                //Updates the text to display the new key
                                selectionDownText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Selection left
                        case 19:
                            {
                                //Sets the key in the GameManager
                                gameManager.moveSelectionLeftKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("selectionLeftKey", (int)key);

                                //Updates the text to display the new key
                                selectionLeftText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Selection right
                        case 20:
                            {
                                //Sets the key in the GameManager
                                gameManager.moveSelectionRightKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("selectionRightKey", (int)key);

                                //Updates the text to display the new key
                                selectionRightText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Sell building
                        case 21:
                            {
                                //Sets the key in the GameManager
                                gameManager.sellKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("sellKey", (int)key);

                                //Updates the text to display the new key
                                sellText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                    }
                    //Marks there are being no control to change now that one has been changed
                    controlToUpdate = -1;

                    //Finishes saving new key between launches
                    PlayerPrefs.Save();
                }
            }
        }
        //Allow users to change hotkeys in the settings men by pressing the current hotkey for it
        else if(optionsMenu.enabled)
        {
            //Camera forward
            if (Input.GetKeyDown(gameManager.forwardKey))
            {
                UpdateKey(0);
            }
            //Camera back
            else if (Input.GetKeyDown(gameManager.backKey))
            {
                UpdateKey(1);
            }
            //Camera left
            else if (Input.GetKeyDown(gameManager.leftKey))
            {
                UpdateKey(2);
            }
            //Camera right
            else if (Input.GetKeyDown(gameManager.rightKey))
            {
                UpdateKey(3);
            }
            //Tier one turret
            else if (Input.GetKeyDown(gameManager.tierOneTurretKey))
            {
                UpdateKey(4);
            }
            //Space for tier two and three turrets left in the keyIds
            //Tier one repair station
            else if (Input.GetKeyDown(gameManager.tierOneRepairKey))
            {
                UpdateKey(7);
            }
            //Space for tier two repair stations left in the keyIds
            //Tier one wall
            else if (Input.GetKeyDown(gameManager.tierOneWallKey))
            {
                UpdateKey(9);
            }
            //Space for tier two walls left in the keyIds
            //Tier one extractor
            else if (Input.GetKeyDown(gameManager.tierOneExtractorKey))
            {
                UpdateKey(11);
            }
            //Space for tier two and three extractors left in the keyIds
            //Next wave
            else if (Input.GetKeyDown(gameManager.nextWaveKey))
            {
                UpdateKey(14);
            }
            //Cancel
            else if (Input.GetKeyDown(gameManager.cancelKey))
            {
                UpdateKey(15);
            }
            //Confirm
            else if (Input.GetKeyDown(gameManager.confirmKey))
            {
                UpdateKey(16);
            }
            //Selection up
            else if (Input.GetKeyDown(gameManager.moveSelectionUpKey))
            {
                UpdateKey(17);
            }
            //Selection down
            else if (Input.GetKeyDown(gameManager.moveSelectionDownKey))
            {
                UpdateKey(18);
            }
            //Selection left
            else if (Input.GetKeyDown(gameManager.moveSelectionLeftKey))
            {
                UpdateKey(19);
            }
            //Selection right
            else if (Input.GetKeyDown(gameManager.moveSelectionRightKey))
            {
                UpdateKey(20);
            }
            //Sell selected building
            else if(Input.GetKeyDown(gameManager.sellKey))
            {
                UpdateKey(21);
            }
        }
    }

    //Finish deciding settings for new game so start creating it
    public void Play()
    {
        //Close the menu
        newGameMenu.enabled = false;

        //Go to the scene with all of in game data
        SceneManager.LoadScene("MainScene");

        //Queue initialization of the GameManager in order to ensure there is time to populate data
        GameManager.Instance.queueInitialize = 2;
    }

    //Exit the game
    public void Exit()
    {
        Application.Quit();
    }

    //Go to the menu where you change your hotkeys
    public void EnterOptions()
    {
        //Ensure that only the relevant menu is visible
        optionsMenu.enabled = true;
        mainMenu.enabled = false;
        newGameMenu.enabled = false;

        //Ensure that the listed hotkeys are accurate
        cameraForwardText.text = BasicUtils.TranslateKey(gameManager.forwardKey);
        cameraBackText.text = BasicUtils.TranslateKey(gameManager.backKey);
        cameraLeftText.text = BasicUtils.TranslateKey(gameManager.leftKey);
        cameraRightText.text = BasicUtils.TranslateKey(gameManager.rightKey);
        tierOneTurretText.text = BasicUtils.TranslateKey(gameManager.tierOneTurretKey);
        tierOneRepairStationText.text = BasicUtils.TranslateKey(gameManager.tierOneRepairKey);
        tierOneWallText.text = BasicUtils.TranslateKey(gameManager.tierOneWallKey);
        tierOneExtractorText.text = BasicUtils.TranslateKey(gameManager.tierOneExtractorKey);
        nextWaveText.text = BasicUtils.TranslateKey(gameManager.nextWaveKey);
        cancelText.text = BasicUtils.TranslateKey(gameManager.cancelKey);
        confirmText.text = BasicUtils.TranslateKey(gameManager.confirmKey);
        selectionUpText.text = BasicUtils.TranslateKey(gameManager.moveSelectionUpKey);
        selectionDownText.text = BasicUtils.TranslateKey(gameManager.moveSelectionDownKey);
        selectionLeftText.text = BasicUtils.TranslateKey(gameManager.moveSelectionLeftKey);
        selectionRightText.text = BasicUtils.TranslateKey(gameManager.moveSelectionRightKey);
        sellText.text = BasicUtils.TranslateKey(gameManager.sellKey);
    }

    //Return from the menu where you change your hotkeys
    public void ExitOptions()
    {
        //Ensure that only the relevant menu is visible
        optionsMenu.enabled = false;
        mainMenu.enabled = true;
        newGameMenu.enabled = false;

        //Ensure that you do not accidently change data outside of the options menu
        controlToUpdate = -1;
    }

    //Mark a hotkey as needing to be changed
    public void UpdateKey(int keyId)
    {
        controlToUpdate = keyId;
        hotkeyTexts[keyId].text = "Press any key";
    }

    //Return to the main menu from the new game menu
    public void ExitStartMenu()
    {
        //Ensure only the relevant menu is visible
        optionsMenu.enabled = false;
        mainMenu.enabled = true;
        newGameMenu.enabled = false;
    }

    //Go to the new game menu from the main menu
    public void EnterStartMenu()
    {
        //Ensure that only the relevant menu is visible
        optionsMenu.enabled = false;
        mainMenu.enabled = false;
        newGameMenu.enabled = true;
    }

    //Changes the seed from the slider
    public void UpdateSimplifiedSeedFromSlider()
    {
        //If this was prompted by the text, skip everything
        if(freshSimplifiedSeedUpdate)
        {
            //Mark as prompted by slider
            freshSimplifiedSeedUpdate = false;

            //Update game manager
            gameManager.simplifiedSeed = simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value;
            
            //Update text input to match slider
            simplifiedSeedInput.text = (simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value).ToString();
            
            //Further updates were not prompted by this
            freshSimplifiedSeedUpdate = true;
        }
    }

    //Changes the seed from the text
    public void UpdateSimplifiedSeedFromInput()
    {
        //If this was prompted by the slider or it is not a valid seed, skip everything
        if(freshSimplifiedSeedUpdate && simplifiedSeedInput.text.Length > 0 && simplifiedSeedInput.text != "-" && simplifiedSeedInput.text != ".")
        {
            //Mark as prompted by text
            freshSimplifiedSeedUpdate = false;

            //Update slider to match text
            simplifiedSeedSlider.value = float.Parse(simplifiedSeedInput.text);

            //Update game manager
            gameManager.simplifiedSeed = simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value;
            
            //Further updates were not prompted by this
            freshSimplifiedSeedUpdate = true;
        }
    }

    //Ensures that seed values outside of the acceptable range that are entered through the text are clamped
    public void FinishUpdatingSimplifiedSeedFromInput()
    {
        //Mark as stale
        freshSimplifiedSeedUpdate = false;
        
        //If it is not a valid seed value, set it to the slider's value
        if(simplifiedSeedInput.text.Length == 0 || simplifiedSeedInput.text == "-" || simplifiedSeedInput.text == "." || float.Parse(simplifiedSeedInput.text) > int.MaxValue || float.Parse(simplifiedSeedInput.text) < int.MinValue)
        {
            simplifiedSeedInput.text = (simplifiedSeedSlider.value == (float)int.MinValue ? int.MinValue : simplifiedSeedSlider.value == (float)int.MaxValue ? int.MaxValue : (int)simplifiedSeedSlider.value).ToString();
        }
        //Further updates are no longer stale
        freshSimplifiedSeedUpdate = true;
    }

    //Changes enemy difficulty from slider
    public void UpdateEnemyDifficultyFromSlider()
    {
        //If it is prompted by text, skip everything
        if (freshEnemyDifficultyUpdate)
        {
            //Mark as prompted by slider
            freshEnemyDifficultyUpdate = false;

            //Update game manager
            gameManager.enemyDifficulty = enemyDifficultySlider.value;

            //Update text to match slider
            enemyDifficultyInput.text = enemyDifficultySlider.value.ToString();

            //Further updates are not prompted by this
            freshEnemyDifficultyUpdate = true;
        }
    }

    //Changes enemy difficulty from text
    public void UpdateEnemyDifficultyFromInput()
    {
        //If this was prompted by slider or it is not a valid difficulty, skip everything
        if (freshEnemyDifficultyUpdate && enemyDifficultyInput.text.Length > 0 && enemyDifficultyInput.text != "-" && enemyDifficultyInput.text != ".")
        {
            //Mark as prompted by text
            freshEnemyDifficultyUpdate = false;

            //Update slider to match text
            enemyDifficultySlider.value = float.Parse(enemyDifficultyInput.text);

            //Update game manager
            gameManager.enemyDifficulty = enemyDifficultySlider.value;

            //Further updates are not prompted by this
            freshEnemyDifficultyUpdate = true;
        }
    }

    //Ensures that difficulty values outside of the acceptable range that are entered through text are clamps
    public void FinishUpdatingEnemyDifficultyFromInput()
    {
        //Mark as stale
        freshEnemyDifficultyUpdate = false;

        //If it is not a valid difficulty, set it to the slider value
        if (float.Parse(enemyDifficultyInput.text) < enemyDifficultySlider.minValue || float.Parse(enemyDifficultyInput.text) > enemyDifficultySlider.maxValue || enemyDifficultyInput.text.Length == 0 || enemyDifficultyInput.text == "-" || enemyDifficultyInput.text == ".")
        {
            enemyDifficultyInput.text = enemyDifficultySlider.value.ToString();
        }
        //Further updates are no longer stale
        freshEnemyDifficultyUpdate = true;
    }

    //Changes player power from slider
    public void UpdatePlayerPowerFromSlider()
    {
        //If it is prompted by text, skip everything
        if (freshPlayerPowerUpdate)
        {
            //Mark as prompted by slider
            freshPlayerPowerUpdate = false;

            //Update game manager
            gameManager.playerPower = playerPowerSlider.value;

            //Update text to match slider
            playerPowerInput.text = playerPowerSlider.value.ToString();

            //Further updates are not prompted by this
            freshPlayerPowerUpdate = true;
        }
    }

    //Changes enemy difficulty from text
    public void UpdatePlayerPowerFromInput()
    {
        //If this was prompted by slider or it is not a valid difficulty, skip everything
        if (freshPlayerPowerUpdate && playerPowerInput.text.Length > 0 && playerPowerInput.text != "-" && playerPowerInput.text != ".")
        {
            //Mark as prompted by text
            freshPlayerPowerUpdate = false;

            //Update slider to match text
            playerPowerSlider.value = float.Parse(playerPowerInput.text);

            //Update game manager
            gameManager.playerPower = playerPowerSlider.value;

            //Further updates are not prompted by this
            freshPlayerPowerUpdate = true;
        }
    }
    public void FinishUpdatingPlayerPowerFromInput()
    {
        //Mark as stale
        freshPlayerPowerUpdate = false;

        //If it is not a valid difficulty, set it to the slider value
        if (float.Parse(playerPowerInput.text) < playerPowerSlider.minValue || float.Parse(playerPowerInput.text) > playerPowerSlider.maxValue || playerPowerInput.text.Length == 0 || playerPowerInput.text == "-" || playerPowerInput.text == ".")
        {
            playerPowerInput.text = playerPowerSlider.value.ToString();
        }

        //Further updates are no longer stale
        freshPlayerPowerUpdate = true;
    }

    //Changes player income from slider
    public void UpdatePlayerIncomeFromSlider()
    {
        //If it is prompted by text, skip everything
        if (freshPlayerIncomeUpdate)
        {
            //Mark as prompted by slider
            freshPlayerIncomeUpdate = false;

            //Update game manager
            gameManager.playerIncome = playerIncomeSlider.value;

            //Update text to match slider
            playerIncomeInput.text = playerIncomeSlider.value.ToString();

            //Further updates are not prompted by this
            freshPlayerIncomeUpdate = true;
        }
    }

    //Changes enemy difficulty from text
    public void UpdatePlayerIncomeFromInput()
    {
        //If this was prompted by slider or it is not a valid difficulty, skip everything
        if (freshPlayerIncomeUpdate && playerIncomeInput.text.Length > 0 && playerIncomeInput.text != "-" && playerIncomeInput.text != ".")
        {
            //Mark as prompted by text
            freshPlayerIncomeUpdate = false;

            //Update slider to match text
            playerIncomeSlider.value = float.Parse(playerIncomeInput.text);

            //Update game manager
            gameManager.playerIncome = playerIncomeSlider.value;

            //Further updates are not prompted by this
            freshPlayerIncomeUpdate = true;
        }
    }
    public void FinishUpdatingPlayerIncomeFromInput()
    {
        //Mark as stale
        freshPlayerIncomeUpdate = false;

        //If it is not a valid difficulty, set it to the slider value
        if (float.Parse(playerIncomeInput.text) < playerIncomeSlider.minValue || float.Parse(playerIncomeInput.text) > playerIncomeSlider.maxValue || playerIncomeInput.text.Length == 0 || playerIncomeInput.text == "-" || playerIncomeInput.text == ".")
        {
            playerIncomeInput.text = playerIncomeSlider.value.ToString();
        }

        //Further updates are no longer stale
        freshPlayerIncomeUpdate = true;
    }

    //Changes player costs from slider
    public void UpdatePlayerCostsFromSlider()
    {
        //If it is prompted by text, skip everything
        if (freshPlayerCostsUpdate)
        {
            //Mark as prompted by slider
            freshPlayerCostsUpdate = false;

            //Update game manager
            gameManager.playerCosts = playerCostsSlider.value;

            //Update text to match slider
            playerCostsInput.text = playerCostsSlider.value.ToString();

            //Further updates are not prompted by this
            freshPlayerCostsUpdate = true;
        }
    }

    //Changes enemy difficulty from text
    public void UpdatePlayerCostsFromInput()
    {
        //If this was prompted by slider or it is not a valid difficulty, skip everything
        if (freshPlayerCostsUpdate && playerCostsInput.text.Length > 0 && playerCostsInput.text != "-" && playerCostsInput.text != ".")
        {
            //Mark as prompted by text
            freshPlayerCostsUpdate = false;

            //Update slider to match text
            playerCostsSlider.value = float.Parse(playerCostsInput.text);

            //Update game manager
            gameManager.playerCosts = playerCostsSlider.value;

            //Further updates are not prompted by this
            freshPlayerCostsUpdate = true;
        }
    }
    public void FinishUpdatingPlayerCostsFromInput()
    {
        //Mark as stale
        freshPlayerCostsUpdate = false;

        //If it is not a valid difficulty, set it to the slider value
        if (float.Parse(playerCostsInput.text) < playerCostsSlider.minValue || float.Parse(playerCostsInput.text) > playerCostsSlider.maxValue || playerCostsInput.text.Length == 0 || playerCostsInput.text == "-" || playerCostsInput.text == ".")
        {
            playerCostsInput.text = playerCostsSlider.value.ToString();
        }

        //Further updates are no longer stale
        freshPlayerCostsUpdate = true;
    }
}
