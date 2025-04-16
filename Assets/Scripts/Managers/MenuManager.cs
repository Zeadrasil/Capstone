using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-30)]
//MenuManager manages the main menu before you start a run
public class MenuManager : Singleton<MenuManager>
{
    //Major menus
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas optionsMenu;
    [SerializeField] Canvas controlsMenu;
    [SerializeField] Canvas newGameMenu;
    [SerializeField] Canvas creditsMenu;
    [SerializeField] Canvas settingsMenu;
    [SerializeField] Canvas helpMenu;
    [SerializeField] Canvas buildingInfo;
    [SerializeField] Canvas enemyInfo;
    [SerializeField] Canvas tileInfo;

    //Tile help outline type storage
    [SerializeField] Canvas noOutline;
    [SerializeField] Canvas thinWhiteOutline;
    [SerializeField] Canvas thickWhiteOutline;
    [SerializeField] Canvas thinBlackOutline;
    [SerializeField] Canvas thickBlackOutline;

    //In-game data
    public Canvas pauseMenu;
    public Canvas turretMenu;
    public Canvas repairMenu;
    public Canvas wallMenu;
    public Canvas resourceMenu;
    public Canvas buildingMenu;
    public Canvas sellPanel;
    private int openMenu = -1;

    //Hotkey labels
    [SerializeField] TMP_Text cameraForwardText;
    [SerializeField] TMP_Text cameraBackText;
    [SerializeField] TMP_Text cameraLeftText;
    [SerializeField] TMP_Text cameraRightText;
    [SerializeField] TMP_Text tierOneTurretText;
    [SerializeField] TMP_Text tierTwoTurretText;
    [SerializeField] TMP_Text tierThreeTurretText;
    [SerializeField] TMP_Text tierOneRepairStationText;
    [SerializeField] TMP_Text tierTwoRepairStationText;
    [SerializeField] TMP_Text tierOneWallText;
    [SerializeField] TMP_Text tierTwoWallText;
    [SerializeField] TMP_Text tierOneExtractorText;
    [SerializeField] TMP_Text tierTwoExtractorText;
    [SerializeField] TMP_Text tierThreeExtractorText;
    [SerializeField] TMP_Text nextWaveText;
    [SerializeField] TMP_Text cancelText;
    [SerializeField] TMP_Text confirmText;
    [SerializeField] TMP_Text selectionUpText;
    [SerializeField] TMP_Text selectionDownText;
    [SerializeField] TMP_Text selectionLeftText;
    [SerializeField] TMP_Text selectionRightText;
    [SerializeField] TMP_Text sellText;
    private TMP_Text[] hotkeyTexts;

    //Settings controls

    //Graphics controls

    //Fullscreen images
    [SerializeField] Image activeFullscreenImage;
    [SerializeField] Image inactiveFullscreenImage;

    //Other
    [SerializeField] TMP_InputField screenHeightInput;
    [SerializeField] TMP_InputField screenWidthInput;
    [SerializeField] TMP_Dropdown outlineDropdown;

    //Audio Controls
    [SerializeField] TMP_InputField masterVolumeInput;
    public Slider masterVolumeSlider;
    [SerializeField] TMP_InputField musicVolumeInput;
    public Slider musicVolumeSlider;
    [SerializeField] TMP_InputField sfxVolumeInput;
    public Slider sfxVolumeSlider;
    [SerializeField] TMP_InputField musicFadeInput;
    [SerializeField] Slider musicFadeSlider;

    //Settings
    bool fullscreen = true;

    //New game basic settings controls
    [SerializeField] TMP_InputField simplifiedSeedInput;
    [SerializeField] Slider simplifiedSeedSlider;
    [SerializeField] TMP_InputField enemyDifficultyInput;
    [SerializeField] Slider enemyDifficultySlider;
    [SerializeField] TMP_InputField playerPowerInput;
    [SerializeField] Slider playerPowerSlider;
    [SerializeField] TMP_InputField playerEconomyInput;
    [SerializeField] Slider playerEconomySlider;

    //New game advanced settings controls
    [SerializeField] TMP_InputField enemyQuantityInput;
    [SerializeField] Slider enemyQuantitySlider;
    [SerializeField] TMP_InputField enemyStrengthInput;
    [SerializeField] Slider enemyStrengthSlider;
    [SerializeField] TMP_InputField playerStrengthInput;
    [SerializeField] Slider playerStrengthSlider;
    [SerializeField] TMP_InputField playerHealthInput;
    [SerializeField] Slider playerHealthSlider;
    [SerializeField] TMP_InputField playerIncomeInput;
    [SerializeField] Slider playerIncomeSlider;
    [SerializeField] TMP_InputField playerCostsInput;
    [SerializeField] Slider playerCostsSlider;
    [SerializeField] TMP_InputField playerEnergyProductionInput;
    [SerializeField] Slider playerEnergyProductionSlider;
    [SerializeField] TMP_InputField playerEnergyUsageInput;
    [SerializeField] Slider playerEnergyUsageSlider;

    //New game custom settings controls

    //Seed settings
    [SerializeField] TMP_InputField seedAInput;
    [SerializeField] Slider seedASlider;
    [SerializeField] TMP_InputField seedBInput;
    [SerializeField] Slider seedBSlider;
    [SerializeField] TMP_InputField seedCInput;
    [SerializeField] Slider seedCSlider;
    [SerializeField] TMP_InputField seedDInput;
    [SerializeField] Slider seedDSlider;
    [SerializeField] TMP_InputField seedEInput;
    [SerializeField] Slider seedESlider;
    [SerializeField] TMP_InputField seedFInput;
    [SerializeField] Slider seedFSlider;
    [SerializeField] TMP_InputField seedGInput;
    [SerializeField] Slider seedGSlider;
    [SerializeField] TMP_InputField seedHInput;
    [SerializeField] Slider seedHSlider;
    [SerializeField] TMP_InputField seedIInput;
    [SerializeField] Slider seedISlider;

    //Scaling
    [SerializeField] TMP_InputField mapScalingInput;
    [SerializeField] Slider mapScalingSlider;
    [SerializeField] TMP_InputField resourceScalingInput;
    [SerializeField] Slider resourceScalingSlider;
    [SerializeField] TMP_InputField aestheticScalingInput;
    [SerializeField] Slider aestheticScalingSlider;

    //Cutoffs
    [SerializeField] TMP_InputField mapCutoffInput;
    [SerializeField] Slider mapCutoffSlider;
    [SerializeField] TMP_InputField resourceCutoffInput;
    [SerializeField] Slider resourceCutoffSlider;

    //Aesthetics cutoffs
    [SerializeField] TMP_InputField aestheticACutoffInput;
    [SerializeField] Slider aestheticACutoffSlider;
    [SerializeField] TMP_InputField aestheticBCutoffInput;
    [SerializeField] Slider aestheticBCutoffSlider;
    [SerializeField] TMP_InputField aestheticCCutoffInput;
    [SerializeField] Slider aestheticCCutoffSlider;
    [SerializeField] TMP_InputField aestheticDCutoffInput;
    [SerializeField] Slider aestheticDCutoffSlider;

    //Map data
    [SerializeField] TMP_InputField mapStartSizeInput;
    [SerializeField] Slider mapStartSizeSlider;
    [SerializeField] TMP_InputField mapExpansionRateInput;
    [SerializeField] Slider mapExpansionRateSlider;

    //Arrays for sliders and text fields
    private TMP_InputField[] inputFieldsFloatRange;
    private TMP_InputField[] inputFieldsFullInt;
    private Slider[] slidersFloatRange;
    private Slider[] slidersFullInt;

    //Holder variables to avoid update loops
    private bool[] freshesFloatRange = new bool[] { true, true, true, true, true, true, true, true, true, true, true, 
        true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
    private bool[] freshesFullInt = new bool[] { true, true, true, true, true, true, true, true, true, true };

    //Holder variables to store creation settings before they are passed into the GameManager
    int simplifiedSeed = 0;
    int seedA = 0;
    int seedB = 0;
    int seedC = 0;
    int seedD = 0;
    int seedE = 0;
    int seedF = 0;
    int seedG = 0;
    int seedH = 0;
    int seedI = 0;

    //Basic
    float enemyDifficulty = 1;
    float playerPower = 3;
    float playerEconomy = 1;

    //Advanced
    float enemyQuantity = 1;
    float enemyStrength = 1;
    float playerStrength = 1;
    float playerHealth = 1;
    float playerIncome = 1;
    float playerCosts = 1;
    float energyProduction = 1;
    float energyConsumption = 1;

    //Scaling
    float mapScaling = 300000000;
    float resourceScaling = 2.5f;
    float aestheticScaling = 3.0f;

    //Cutoffs
    float resourceCutoff = 0.725f;
    float traversableCutoff = 0.45f;
    float aestheticACutoff = 0.2f;
    float aestheticBCutoff = 0.4f;
    float aestheticCCutoff = 0.6f;
    float aestheticDCutoff = 0.8f;

    //Map data
    int startSize = 15;
    int expansionRate = 2; 

    //Allows GameManager initialization to be queued up in order to allow time for data to be passed into it
    int queueInitialize = -1;
    int initializationType = -1;

    //GameManager reference to reduce typing length
    private GameManager gameManager;

    //Controls which hotkey you are changing
    int controlToUpdate = -1;

    //Storage for new game menu UIs
    [SerializeField] Canvas basicSettings;
    [SerializeField] Canvas advancedSettings;
    [SerializeField] Canvas customSettings;

    //Loaded data storage
    GameData loadedData;

    //Background helpers
    [SerializeField] LocalTileManager localTileManager;
    bool loadBackgroundLater = true;

    //Menu background seeds
    private uint menuTraversableSeedA;
    private uint menuTraversableSeedB;
    private uint menuTraversableSeedC;
    private uint menuResourceSeedA;
    private uint menuResourceSeedB;
    private uint menuResourceSeedC;
    private uint menuAestheticSeedA;
    private uint menuAestheticSeedB;
    private uint menuAestheticSeedC;

    //Called just before first Update() call
    private void Start()
    {
        //Create storage for hotkey labels
        hotkeyTexts = new TMP_Text[] {cameraForwardText, cameraBackText, cameraLeftText, cameraRightText, tierOneTurretText, tierTwoTurretText, tierThreeTurretText, tierOneRepairStationText,  tierTwoRepairStationText, tierOneWallText, tierTwoWallText, tierOneExtractorText, tierTwoExtractorText, tierThreeExtractorText, nextWaveText, cancelText, confirmText, selectionUpText, selectionDownText, selectionLeftText, selectionRightText, sellText};

        //Create storages for settings inputs
        inputFieldsFloatRange = new TMP_InputField[] { enemyDifficultyInput, playerPowerInput, playerEconomyInput, 
            enemyQuantityInput, enemyStrengthInput, playerStrengthInput, playerHealthInput, playerIncomeInput, 
            playerCostsInput, playerEnergyProductionInput, playerEnergyUsageInput, mapScalingInput, 
            resourceScalingInput, aestheticScalingInput, mapCutoffInput, resourceCutoffInput, aestheticACutoffInput, 
            aestheticBCutoffInput, aestheticCCutoffInput, aestheticDCutoffInput, mapStartSizeInput, 
            mapExpansionRateInput, masterVolumeInput, musicVolumeInput, sfxVolumeInput, musicFadeInput };
        inputFieldsFullInt = new TMP_InputField[] { simplifiedSeedInput, seedAInput, seedBInput, seedCInput, seedDInput,
            seedEInput, seedFInput, seedGInput, seedHInput, seedIInput};
        
        //Create storages for settings sliders
        slidersFloatRange = new Slider[] { enemyDifficultySlider, playerPowerSlider, playerEconomySlider, 
            enemyQuantitySlider, enemyStrengthSlider, playerStrengthSlider, playerHealthSlider, playerIncomeSlider, 
            playerCostsSlider, playerEnergyProductionSlider, playerEnergyUsageSlider, mapScalingSlider, 
            resourceScalingSlider, aestheticScalingSlider, mapCutoffSlider, resourceCutoffSlider, aestheticACutoffSlider,
            aestheticBCutoffSlider, aestheticCCutoffSlider, aestheticDCutoffSlider, mapStartSizeSlider,
            mapExpansionRateSlider, masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider, musicFadeSlider };
        slidersFullInt = new Slider[] { simplifiedSeedSlider, seedASlider, seedBSlider, seedCSlider, seedDSlider,
            seedESlider, seedFSlider, seedGSlider, seedHSlider, seedISlider };

        //Initializes manager reference
        gameManager = GameManager.Instance;

        //Gets a random initial seed so that you don't have to come up with a new one by yourself
        simplifiedSeedSlider.value = UnityEngine.Random.Range((float)int.MinValue, (float)int.MaxValue);

        //Initializes background seeds
        System.Random rand = new System.Random();
        menuTraversableSeedA = (uint)rand.Next();
        menuTraversableSeedB = (uint)rand.Next();
        menuTraversableSeedC = (uint)rand.Next();
        menuResourceSeedA = (uint)rand.Next();
        menuResourceSeedB = (uint)rand.Next();
        menuResourceSeedC = (uint)rand.Next();
        menuAestheticSeedA = (uint)rand.Next();
        menuAestheticSeedB = (uint)rand.Next();
        menuAestheticSeedC = (uint)rand.Next();

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
        //Load preferred tier two turret hotkey
        gameManager.tierThreeTurretKey = (KeyCode)PlayerPrefs.GetInt("tierTwoTurretKey");
        if (gameManager.tierTwoTurretKey == KeyCode.None)
        {
            gameManager.tierTwoTurretKey = KeyCode.Alpha2;
        }
        //Load preferred tier three turret hotkey
        gameManager.tierThreeTurretKey = (KeyCode)PlayerPrefs.GetInt("tierThreeTurretKey");
        if (gameManager.tierThreeTurretKey == KeyCode.None)
        {
            gameManager.tierThreeTurretKey = KeyCode.Alpha3;
        }
        //Load preferred tier one repair station hotkey
        gameManager.tierOneRepairKey = (KeyCode)PlayerPrefs.GetInt("tierOneRepairKey");
        if (gameManager.tierOneRepairKey == KeyCode.None)
        {
            gameManager.tierOneRepairKey = KeyCode.Alpha4;
        }
        //Load preferred tier two repair station hotkey
        gameManager.tierTwoRepairKey = (KeyCode)PlayerPrefs.GetInt("tierTwoRepairKey");
        if (gameManager.tierTwoRepairKey == KeyCode.None)
        {
            gameManager.tierTwoRepairKey = KeyCode.Alpha5;
        }
        //Load preferred tier one wall hotkey
        gameManager.tierOneWallKey = (KeyCode)PlayerPrefs.GetInt("tierOneWallKey");
        if (gameManager.tierOneWallKey == KeyCode.None)
        {
            gameManager.tierOneWallKey = KeyCode.Alpha6;
        }
        //Load preferred tier two wall hotkey
        gameManager.tierTwoWallKey = (KeyCode)PlayerPrefs.GetInt("tierTwoWallKey");
        if (gameManager.tierTwoWallKey == KeyCode.None)
        {
            gameManager.tierTwoWallKey = KeyCode.Alpha7;
        }
        //Load preferred tier one extractor hotkey
        gameManager.tierOneExtractorKey = (KeyCode)PlayerPrefs.GetInt("tierOneExtractorKey");
        if (gameManager.tierOneExtractorKey == KeyCode.None)
        {
            gameManager.tierOneExtractorKey = KeyCode.Alpha8;
        }
        //Load preferred tier two extractor hotkey
        gameManager.tierTwoExtractorKey = (KeyCode)PlayerPrefs.GetInt("tierTwoExtractorKey");
        if (gameManager.tierTwoExtractorKey == KeyCode.None)
        {
            gameManager.tierTwoExtractorKey = KeyCode.Alpha9;
        }
        //Load preferred tier three extractor hotkey
        gameManager.tierThreeExtractorKey = (KeyCode)PlayerPrefs.GetInt("tierThreeExtractorKey");
        if (gameManager.tierThreeExtractorKey == KeyCode.None)
        {
            gameManager.tierThreeExtractorKey = KeyCode.Alpha0;
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

        //Might end up dropping this

        ////Load saved enemy difficulty
        //if (PlayerPrefs.HasKey("previousEnemyDifficulty"))
        //{
        //    enemyDifficultySlider.value = PlayerPrefs.GetFloat("previousEnemyDifficulty");
        //}
        ////Load saved player power
        //if(PlayerPrefs.HasKey("previousPlayerPower"))
        //{
        //    playerPowerSlider.value = PlayerPrefs.GetFloat("previousPlayerPower");
        //}
        ////Load saved player income
        //if(PlayerPrefs.HasKey("previousPlayerIncome"))
        //{
        //    playerIncomeSlider.value = PlayerPrefs.GetFloat("previousPlayerIncome");
        //}
        ////Load saved player costs
        //if(PlayerPrefs.HasKey("previousPlayerCosts"))
        //{
        //    playerCostsSlider.value = PlayerPrefs.GetFloat("previousPlayerCosts");
        //}

        newGameMenu.enabled = false;
        settingsMenu.enabled = false;
        basicSettings.enabled = false;
        advancedSettings.enabled = false;
        customSettings.enabled = false;
        creditsMenu.enabled = false;
        optionsMenu.enabled = false;
        helpMenu.enabled = false;
        controlsMenu.enabled = false;
        buildingInfo.enabled = false;
        enemyInfo.enabled = false;
        tileInfo.enabled = false;
        noOutline.enabled = false;
        thickBlackOutline.enabled = false;
        thickWhiteOutline.enabled = false;
        thinBlackOutline.enabled = false;
        thinWhiteOutline.enabled = false;

        outlineDropdown.value = PlayerPrefs.GetInt("OutlineType", 0);
        screenHeightInput.text = PlayerPrefs.GetInt("ScreenHeight", 1920).ToString();
        screenWidthInput.text = PlayerPrefs.GetInt("ScreenWidth", 1080).ToString();
        fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 0;
        ToggleFullscreen();
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
                        //Tier two turret
                        case 5:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierTwoTurretKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierTwoTurretKey", (int)key);

                                //Updates the text to display the new key
                                tierTwoTurretText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Tier three turret
                        case 6:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierThreeTurretKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierThreeTurretKey", (int)key);

                                //Updates the text to display the new key
                                tierThreeTurretText.text = BasicUtils.TranslateKey(key);
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
                        //Tier two repair station
                        case 8:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierTwoRepairKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierTwoRepairKey", (int)key);

                                //Updates the text to display the new key
                                tierTwoRepairStationText.text = BasicUtils.TranslateKey(key);
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
                        //Tier two wall
                        case 10:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierTwoWallKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierTwoWallKey", (int)key);

                                //Updates the text to display the new key
                                tierTwoWallText.text = BasicUtils.TranslateKey(key);
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
                        //Tier two extractor
                        case 12:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierTwoExtractorKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierTwoExtractorKey", (int)key);

                                //Updates the text to display the new key
                                tierTwoExtractorText.text = BasicUtils.TranslateKey(key);
                                break;
                            }
                        //Tier three extractor
                        case 13:
                            {
                                //Sets the key in the GameManager
                                gameManager.tierThreeExtractorKey = key;

                                //Saves the key between game launches
                                PlayerPrefs.SetInt("tierThreeExtractorKey", (int)key);

                                //Updates the text to display the new key
                                tierThreeExtractorText.text = BasicUtils.TranslateKey(key);
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
        else if(controlsMenu.enabled)
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
            //Tier two turret
            else if (Input.GetKeyDown(gameManager.tierTwoTurretKey))
            {
                UpdateKey(5);
            }
            //Tier three turret
            else if (Input.GetKeyDown(gameManager.tierThreeTurretKey))
            {
                UpdateKey(6);
            }
            //Tier one repair station
            else if (Input.GetKeyDown(gameManager.tierOneRepairKey))
            {
                UpdateKey(7);
            }
            //Tier two repair station
            else if (Input.GetKeyDown(gameManager.tierTwoRepairKey))
            {
                UpdateKey(8);
            }
            //Tier one wall
            else if (Input.GetKeyDown(gameManager.tierOneWallKey))
            {
                UpdateKey(9);
            }
            //Tier two wall
            else if (Input.GetKeyDown(gameManager.tierOneWallKey))
            {
                UpdateKey(10);
            }
            //Tier one extractor
            else if (Input.GetKeyDown(gameManager.tierOneExtractorKey))
            {
                UpdateKey(11);
            }
            //Tier two extractor
            else if (Input.GetKeyDown(gameManager.tierTwoExtractorKey))
            {
                UpdateKey(12);
            }
            //Tier three extractor
            else if (Input.GetKeyDown(gameManager.tierThreeExtractorKey))
            {
                UpdateKey(13);
            }
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
        else if(queueInitialize > -1)
        {
            if (queueInitialize == 0)
            {
                //Initialize GameManager depending on chosen settings
                switch (initializationType)
                {
                    //Basic settings
                    case 0:
                        {
                            GameManager.Instance.Initialize(simplifiedSeed, enemyDifficulty, playerPower, playerEconomy);
                            break;
                        }
                    //Advanced settings
                    case 1:
                        {
                            GameManager.Instance.Initialize(simplifiedSeed, enemyQuantity, enemyStrength, playerStrength, playerHealth, playerCosts, playerIncome, energyProduction, energyConsumption);
                            break;
                        }
                    //Custom settings
                    case 2:
                        {
                            GameManager.Instance.Initialize(simplifiedSeed, enemyQuantity, enemyStrength, playerStrength, playerHealth, playerCosts, playerIncome, energyProduction, energyConsumption, (uint)seedA, (uint)seedB, (uint)seedC, (uint)seedD, (uint)seedE, (uint)seedF, (uint)seedG, (uint)seedH, (uint)seedI, (int)mapScaling, traversableCutoff, resourceScaling, resourceCutoff, aestheticScaling, aestheticACutoff, aestheticACutoff, aestheticCCutoff, aestheticDCutoff, startSize, expansionRate);
                            break;
                        }
                    //Loaded Save
                    case 3:
                        {
                            GameManager.Instance.InitializeFromLoad(loadedData);
                            break;
                        }
                }
                queueInitialize = -1;
            }
            else
            {
                queueInitialize--;
            }
        }
        if(loadBackgroundLater)
        {
            loadBackground();
            loadBackgroundLater = false;
        }
    }

    //Finished deciding settings for new game so start creating it
    public void Play()
    {
        //Close the menu
        newGameMenu.enabled = false;

        //Ensures that all settings inputs are disabled in an attempt to get it to stop having them randomly selected during the map
        foreach(Slider slider in slidersFloatRange)
        {
            slider.enabled = false;
        }
        foreach(Slider slider in slidersFullInt)
        {
            slider.enabled = false;
        }
        foreach(TMP_InputField field in inputFieldsFloatRange)
        {
            field.enabled = false;
        }
        foreach(TMP_InputField field in inputFieldsFullInt)
        {
            field.enabled = false;
        }

        //Stop menu theme
        MusicManager.Instance.StopMenu();

        //Disable irrelevant menus
        basicSettings.enabled = false;
        advancedSettings.enabled = false;
        optionsMenu.enabled = false;

        TileManager.Instance.BlockerTilemap.ClearAllTiles();
        TileManager.Instance.TraversableTilemap.ClearAllTiles();

        //Deactivate audio listener in order to ensure proper one is active
        MusicManager.Instance.gameObject.GetComponent<AudioListener>().enabled = false;

        //Go to the scene with all of the in game data
        SceneManager.LoadScene("MainScene");

        //Queue up initialization of the game, allowing time for values to be passed into the GameManager
        queueInitialize = 1;
    }

    //Exit the game
    public void Exit()
    {
        Application.Quit();
    }

    //Go to the menu where you change your hotkeys
    public void EnterControls()
    {
        //Ensure that only the relevant menu is visible
        controlsMenu.enabled = true;
        settingsMenu.enabled = false;

        //Ensure that the listed hotkeys are accurate
        cameraForwardText.text = BasicUtils.TranslateKey(gameManager.forwardKey);
        cameraBackText.text = BasicUtils.TranslateKey(gameManager.backKey);
        cameraLeftText.text = BasicUtils.TranslateKey(gameManager.leftKey);
        cameraRightText.text = BasicUtils.TranslateKey(gameManager.rightKey);
        tierOneTurretText.text = BasicUtils.TranslateKey(gameManager.tierOneTurretKey);
        tierTwoTurretText.text = BasicUtils.TranslateKey(gameManager.tierTwoTurretKey);
        tierThreeTurretText.text = BasicUtils.TranslateKey(gameManager.tierThreeTurretKey);
        tierOneRepairStationText.text = BasicUtils.TranslateKey(gameManager.tierOneRepairKey);
        tierTwoRepairStationText.text = BasicUtils.TranslateKey(gameManager.tierTwoRepairKey);
        tierOneWallText.text = BasicUtils.TranslateKey(gameManager.tierOneWallKey);
        tierTwoWallText.text = BasicUtils.TranslateKey(gameManager.tierTwoWallKey);
        tierOneExtractorText.text = BasicUtils.TranslateKey(gameManager.tierOneExtractorKey);
        tierTwoExtractorText.text = BasicUtils.TranslateKey(gameManager.tierTwoExtractorKey);
        tierThreeExtractorText.text = BasicUtils.TranslateKey(gameManager.tierThreeExtractorKey);
        nextWaveText.text = BasicUtils.TranslateKey(gameManager.nextWaveKey);
        cancelText.text = BasicUtils.TranslateKey(gameManager.cancelKey);
        confirmText.text = BasicUtils.TranslateKey(gameManager.confirmKey);
        selectionUpText.text = BasicUtils.TranslateKey(gameManager.moveSelectionUpKey);
        selectionDownText.text = BasicUtils.TranslateKey(gameManager.moveSelectionDownKey);
        selectionLeftText.text = BasicUtils.TranslateKey(gameManager.moveSelectionLeftKey);
        selectionRightText.text = BasicUtils.TranslateKey(gameManager.moveSelectionRightKey);
        sellText.text = BasicUtils.TranslateKey(gameManager.sellKey);
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
        controlsMenu.enabled = false;
        basicSettings.enabled = false;
        mainMenu.enabled = true;
        newGameMenu.enabled = false;
    }

    //Go to the new game menu from the main menu
    public void EnterStartMenu()
    {
        //Ensure that only the relevant menu is visible
        controlsMenu.enabled = false;
        mainMenu.enabled = false;
        newGameMenu.enabled = true;
        BasicSettings();
    }

    //Full int generic update
    private void fullIntUpdate(int setting)
    {
        //Switch depending on what you want to update
        switch (setting)
        {
            //Simplified seed
            case 0:
                {
                    simplifiedSeed = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Map seed A
            case 1:
                {
                    seedA = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Map seed B
            case 2:
                {
                    seedB = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Map seed C
            case 3:
                {
                    seedC = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Resource seed A
            case 4:
                {
                    seedD = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Resource seed B
            case 5:
                {
                    seedE = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Resource seed C
            case 6:
                {
                    seedF = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Aesthetic seed A
            case 7:
                {
                    seedG = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Aesthetic seed B
            case 8:
                {
                    seedH = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
            //Aesthetic seed C
            case 9:
                {
                    seedI = slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value;
                    break;
                }
        }
    }

    //Update any setting using full int range using slider
    public void UpdateFullIntFromSlider(int setting)
    {
        //If this was prompted by the text, skip everything
        if (freshesFullInt[setting])
        {
            //Mark as prompted by slider
            freshesFullInt[setting] = false;

            //Update relevant setting with new setting
            fullIntUpdate(setting);

            //Update text to match slider
            inputFieldsFullInt[setting].text = (slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value).ToString();

            //Further updates were not prompted by this
            freshesFullInt[setting] = true;
        }
    }

    //Update any setting using full in range using slider
    public void UpdateFullIntFromText(int setting)
    {
        //If this was prompted by the slider or it is not a valid seed, skip everything
        if (freshesFullInt[setting] && inputFieldsFullInt[setting].text.Length > 0 && inputFieldsFullInt[setting].text != "-" && inputFieldsFullInt[setting].text != ".")
        {
            //Mark as prompted by text
            freshesFullInt[setting] = false;

            //Update slider to match text
            slidersFullInt[setting].value = float.Parse(inputFieldsFullInt[setting].text);

            //Update relevant setting with new setting
            fullIntUpdate(setting);

            //Further updates were not prompted by this
            freshesFullInt[setting] = true;
        }
    }

    //Ensures that full int range values outside of the acceptable range that are entered through the text are clamped
    public void FinishUpdatingFullIntFromInput(int setting)
    {
        //If it is not a valid seed value, set it to the slider's value
        if (inputFieldsFullInt[setting].text.Length == 0 || inputFieldsFullInt[setting].text == "-" || inputFieldsFullInt[setting].text == "." || float.Parse(inputFieldsFullInt[setting].text) > int.MaxValue || float.Parse(inputFieldsFullInt[setting].text) < int.MinValue)
        {
            inputFieldsFullInt[setting].text = (slidersFullInt[setting].value == (float)int.MinValue ? int.MinValue : slidersFullInt[setting].value == (float)int.MaxValue ? int.MaxValue : (int)slidersFullInt[setting].value).ToString();
        }
    }

    //Float range generic update
    private void floatRangeUpdate(int setting)
    {
        //Switch depending on what you want to update
        switch (setting)
        {
            //Enemy difficulty
            case 0:
                {
                    enemyDifficulty = slidersFloatRange[setting].value;
                    break;
                }
            //Player power
            case 1:
                {
                    playerPower = slidersFloatRange[setting].value;
                    break;
                }
            //Player economy
            case 2:
                {
                    playerEconomy = slidersFloatRange[setting].value;
                    break;
                }
            //Enemy quantity
            case 3:
                {
                    enemyQuantity = slidersFloatRange[setting].value;
                    break;
                }
            //Enemy strength
            case 4:
                {
                    enemyStrength = slidersFloatRange[setting].value;
                    break;
                }
            //Player strength
            case 5:
                {
                    playerStrength = slidersFloatRange[setting].value;
                    break;
                }
            //Player health
            case 6:
                {
                    playerHealth = slidersFloatRange[setting].value;
                    break;
                }
            //Player income
            case 7:
                {
                    playerIncome = slidersFloatRange[setting].value;
                    break;
                }
            //Player costs
            case 8:
                {
                    playerCosts = slidersFloatRange[setting].value;
                    break;
                }
            //Energy production
            case 9:
                {
                    energyProduction = slidersFloatRange[setting].value;
                    break;
                }
            //Energy consumption
            case 10:
                {
                    energyConsumption = slidersFloatRange[setting].value;
                    break;
                }
            //Map scaling
            case 11:
                {
                    mapScaling = slidersFloatRange[setting].value;
                    break;
                }
            case 12:
                {
                    resourceScaling = slidersFloatRange[setting].value;
                    break;
                }
            //Aesthetic scaling
            case 13:
                {
                    aestheticScaling = slidersFloatRange[setting].value;
                    break;
                }
            //Map cutoff
            case 14:
                {
                    traversableCutoff = slidersFloatRange[setting].value;
                    break;
                }
            //Resource cutoff
            case 15:
                {
                    resourceCutoff = slidersFloatRange[setting].value;
                    break;
                }
            //Aesthetic A cutoff
            case 16:
                {
                    aestheticACutoff = slidersFloatRange[setting].value;
                    break;
                }
            //Aesthetic B cutoff
            case 17:
                {
                    aestheticBCutoff = slidersFloatRange[setting].value;
                    break;
                }
            //Aesthetic C cutoff
            case 18:
                {
                    aestheticCCutoff = slidersFloatRange[setting].value;
                    break;
                }
            //Aesthetic D cutoff
            case 19:
                {
                    aestheticDCutoff = slidersFloatRange[setting].value;
                    break;
                }
            //Map start size
            case 20:
                {
                    startSize = (int)slidersFloatRange[setting].value;
                    break;
                }
            //Expansion rate
            case 21:
                {
                    expansionRate = (int)slidersFloatRange[setting].value;
                    break;
                }
            //Master Volume
            case 22:
                {
                    MusicManager.Instance.UpdateMasterVolume(slidersFloatRange[setting].value);
                    break;
                }
            //Music Volume
            case 23:
                {
                    MusicManager.Instance.UpdateMusicVolume(slidersFloatRange[setting].value);
                    break;
                }
            //SFX Volume
            case 24:
                {
                    MusicManager.Instance.UpdateSFXVolume(slidersFloatRange[setting].value);
                    break;
                }
            //Music Fade:
            case 25:
                {
                    MusicManager.Instance.musicFadeTime = slidersFloatRange[setting].value;
                    break;
                }
        }
    }

    //Update any setting using full int range using slider
    public void UpdateFloatRangeFromSlider(int setting)
    {
        //If this was prompted by the text, skip everything
        if (freshesFloatRange[setting])
        {
            //Mark as prompted by slider
            freshesFloatRange[setting] = false;

            //Update relevant setting with new setting
            floatRangeUpdate(setting);

            //Update text to match slider
            inputFieldsFloatRange[setting].text = slidersFloatRange[setting].value.ToString();

            //Further updates were not prompted by this
            freshesFloatRange[setting] = true;
        }
    }

    //Update any setting using full in range using slider
    public void UpdateFloatRangeFromText(int setting)
    {
        //If this was prompted by the slider or it is not a valid seed, skip everything
        if (freshesFloatRange[setting] && inputFieldsFloatRange[setting].text.Length > 0 && inputFieldsFloatRange[setting].text != "-" && inputFieldsFloatRange[setting].text != ".")
        {
            //Mark as prompted by text
            freshesFloatRange[setting] = false;

            //Update slider to match text
            slidersFloatRange[setting].value = float.Parse(inputFieldsFloatRange[setting].text);

            //Update relevant setting with new setting
            floatRangeUpdate(setting);

            //Further updates were not prompted by this
            freshesFloatRange[setting] = true;
        }
    }

    //Ensures that float range values outside of the acceptable range that are entered through text are clamped
    public void FinishUpdatingFloatRangeFromInput(int setting)
    {
        //If it is not a valid difficulty, set it to the slider value
        if (float.Parse(inputFieldsFloatRange[setting].text) < slidersFloatRange[setting].minValue || float.Parse(inputFieldsFloatRange[setting].text) > slidersFloatRange[setting].maxValue || inputFieldsFloatRange[setting].text.Length == 0 || inputFieldsFloatRange[setting].text == "-" || inputFieldsFloatRange[setting].text == ".")
        {
            inputFieldsFloatRange[setting].text = slidersFloatRange[setting].value.ToString();
        }
    }
    
    //Go to advanced new game settings
    public void AdvancedSettings()
    {
        advancedSettings.enabled = true;
        basicSettings.enabled = false;
        customSettings.enabled = false;
        initializationType = 1;
    }

    //Go to basic new game settings
    public void BasicSettings()
    {
        basicSettings.enabled = true;
        advancedSettings.enabled = false;
        customSettings.enabled= false;
        initializationType = 0;
    }

    //Go to custom new game settings
    public void CustomSettings()
    {
        basicSettings.enabled = false;
        advancedSettings.enabled = false;
        customSettings.enabled = true;
        initializationType = 2;
    }

    //Load data from save
    public void LoadData()
    {
        //Load from file
        loadedData = FileManager.Instance.Load();

        //Skip if no file was loaded
        if (loadedData != null)
        {
            //Specify to intialize as loaded game
            initializationType = 3;

            //Close the main menu
            mainMenu.enabled = false;

            //Begin initialization
            Play();
        }
    }

    //Open the pause menu
    public void Pause()
    {
        //Enable pause menu
        pauseMenu.enabled = true;

        //Disable those which always need to be
        sellPanel.enabled = false;
        buildingMenu.enabled = false;

        //Identifies if any upgrade menu is open
        if(turretMenu.enabled)
        {
            turretMenu.enabled = false;
            openMenu = 0;
        }
        else if(repairMenu.enabled)
        {
            repairMenu.enabled = false;
            openMenu = 1;
        }
        else if(wallMenu.enabled)
        {
            wallMenu.enabled = false;
            openMenu = 2;
        }
        else if(resourceMenu.enabled)
        {
            resourceMenu.enabled = false;
            openMenu = 3;
        }
        else
        {
            openMenu = -1;
        }

        //Tells the game manager to stop working so hard
        gameManager.paused = true;
    }

    //Close the pause menu
    public void UnPause()
    {
        //Disables pause menu
        pauseMenu.enabled = false;

        //Enables building menu
        buildingMenu.enabled = true;
        if(openMenu != -1)
        {
            //You always have the sell option if you have any upgrade window open
            sellPanel.enabled = true;

            //Use the variable stoed earlier to determine which upgrade menu needs to be enabled
            switch(openMenu)
            {
                case 0:
                    {
                        turretMenu.enabled = true;
                        break;
                    }
                case 1:
                    {
                        repairMenu.enabled = true;
                        break;
                    }
                case 2:
                    {
                        wallMenu.enabled = true;
                        break;
                    }
                case 3:
                    {
                        resourceMenu.enabled = true;
                        break;
                    }
            }
        }
        //Tell the GameManager to stop slacking off
        gameManager.paused = false;
    }

    //Go back to main menu
    public void Return()
    {
        //Update music
        MusicManager.Instance.PlayMenu();

        //Deactivate GameManager while in menu
        GameManager.Instance.Deactivate();

        //Load the menu again
        SceneManager.LoadScene("MenuScene");

        //Reactivate main menu listener
        MusicManager.Instance.gameObject.GetComponent<AudioListener>().enabled = true;

        mainMenu.enabled = true;
        basicSettings.enabled = false;
        advancedSettings.enabled = false;
        controlsMenu.enabled = false;
        newGameMenu.enabled = false;

        //Ensures that all settings inputs are reenabled after leaving the map
        foreach (Slider slider in slidersFloatRange)
        {
            slider.enabled = true;
        }
        foreach (Slider slider in slidersFullInt)
        {
            slider.enabled = true;
        }
        foreach (TMP_InputField field in inputFieldsFloatRange)
        {
            field.enabled = true;
        }
        foreach (TMP_InputField field in inputFieldsFullInt)
        {
            field.enabled = true;
        }
        loadBackgroundLater = true;
    }

    //Go to credits
    public void EnterCredits()
    {
        mainMenu.enabled = false;
        creditsMenu.enabled = true;
    }

    //Return from credits
    public void ExitCredits()
    {
        mainMenu.enabled = true;
        creditsMenu.enabled = false;
    }

    //Go to settings
    public void EnterSettings()
    {
        controlsMenu.enabled = false;
        settingsMenu.enabled = true;
        controlToUpdate = -1;
    }

    //Turn fullscreen mode on and off
    public void ToggleFullscreen()
    {
        //Store data for use
        fullscreen = !fullscreen;

        //Update UI
        activeFullscreenImage.enabled = fullscreen;
        inactiveFullscreenImage.enabled = !fullscreen;

        //Change screen to match
        UpdateResolution();
    }

    //Update screen to match new resolution settings
    public void UpdateResolution()
    {
        Screen.SetResolution(int.Parse(screenWidthInput.text), int.Parse(screenHeightInput.text), fullscreen);

        //Update saved settings
        PlayerPrefs.SetInt("ScreenWidth", int.Parse(screenWidthInput.text));
        PlayerPrefs.SetInt("ScreenHeight", int.Parse(screenHeightInput.text));
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    //Update outline type
    public void UpdateOutline()
    {
        GameManager.Instance.outlineType = outlineDropdown.value;

        //Update saved settings
        PlayerPrefs.SetInt("OutlineType", outlineDropdown.value);
        PlayerPrefs.Save();

        //Update background
        loadBackgroundLater = true;
    }

    //Load background tiles
    private void loadBackground()
    {
        //Set the tiles based on the outline settings
        localTileManager.Start();
        
        //Sets how much is generated, 15 is just large enough
        TileManager.Instance.size = 15;

        //Sets seeds for the traversal data of the background
        TileManager.Instance.seedA = menuTraversableSeedA;
        TileManager.Instance.seedB = menuTraversableSeedB;
        TileManager.Instance.seedC = menuTraversableSeedC;

        //Sets seeds for the resource data of the background
        TileManager.Instance.seedD = menuResourceSeedA;
        TileManager.Instance.seedE = menuResourceSeedB;
        TileManager.Instance.seedF = menuResourceSeedC;

        //Sets seeds for the resource data of the background
        TileManager.Instance.seedG = menuAestheticSeedA;
        TileManager.Instance.seedH = menuAestheticSeedB;
        TileManager.Instance.seedI = menuAestheticSeedC;

        //Scaling for map
        TileManager.Instance.resourceScaling = 2.5f;
        TileManager.Instance.aestheticScaling = 3.0f;
        TileManager.Instance.mapScaling = 300000000;

        //Cutoffs 
        TileManager.Instance.traversableCutoff = 0.45f;
        TileManager.Instance.resourceCutoff = 0.75f;
        TileManager.Instance.aestheticACutoff = 0.2f;
        TileManager.Instance.aestheticBCutoff = 0.4f;
        TileManager.Instance.aestheticCCutoff = 0.6f;
        TileManager.Instance.aestheticDCutoff = 0.8f;

        //Makes the background
        TileManager.Instance.Initialize();
    }

    //Enter the combined options menus
    public void EnterOptions()
    {
        optionsMenu.enabled = true;
        mainMenu.enabled = false;
        EnterControls();
    }

    //Exit the combined options menu
    public void ExitOptions()
    {
        optionsMenu.enabled = false;
        settingsMenu.enabled = false;
        controlsMenu.enabled = false;
        mainMenu.enabled = true;
        controlToUpdate = -1;
    }

    //Enter help menu
    public void EnterHelp()
    {
        helpMenu.enabled = true;
        mainMenu.enabled = false;
        EnterBuildingInfo();
    }

    //Enter the section of the help menu with the info about buildings
    public void EnterBuildingInfo()
    {
        buildingInfo.enabled = true;
        enemyInfo.enabled = false;
        tileInfo.enabled = false;
        noOutline.enabled = false;
        thickBlackOutline.enabled = false;
        thickWhiteOutline.enabled = false;
        thinBlackOutline.enabled = false;
        thinWhiteOutline.enabled = false;
    }

    //Enter the section of the help menu with the info about buildings
    public void EnterEnemyInfo()
    {
        buildingInfo.enabled = false;
        enemyInfo.enabled = true;
        tileInfo.enabled = false;
        noOutline.enabled = false;
        thickBlackOutline.enabled = false;
        thickWhiteOutline.enabled = false;
        thinBlackOutline.enabled = false;
        thinWhiteOutline.enabled = false;
    }

    //Enter the section of the help menu with the info about buildings
    public void EnterTileInfo()
    {
        buildingInfo.enabled = false;
        enemyInfo.enabled = false;
        tileInfo.enabled = true;
        noOutline.enabled = GameManager.Instance.outlineType == 0;
        thinBlackOutline.enabled = GameManager.Instance.outlineType == 1;
        thinWhiteOutline.enabled = GameManager.Instance.outlineType == 2;
        thickBlackOutline.enabled = GameManager.Instance.outlineType == 3;
        thickWhiteOutline.enabled = GameManager.Instance.outlineType == 4;
    }

    //Exit help menu
    public void ExitHelp()
    {
        helpMenu.enabled = false;
        mainMenu.enabled = true;
        buildingInfo.enabled = false;
        enemyInfo.enabled = false;
        tileInfo.enabled = false;
        noOutline.enabled = false;
        thickBlackOutline.enabled = false;
        thickWhiteOutline.enabled = false;
        thinBlackOutline.enabled = false;
        thinWhiteOutline.enabled = false;
    }
}
