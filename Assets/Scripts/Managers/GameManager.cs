using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

//GameManager manages everything inside the game that is not directly related to the main menu or tiles
public class GameManager : Singleton<GameManager>
{

    public GameObject EnemyCheckpointPrefab;
    public Camera Camera;
    public bool paused;


    //Map data
    int expansionRate = 2;

    //Stores which wave you are on
    public int wave;

    //Stores enemies so that it is known when you have killed them all
    public List<Enemy> currentEnemies = new List<Enemy>();

    //Stores checkpoints so that they can be culled for the new wave
    public List<GameObject> checkpoints = new List<GameObject>();
    public bool betweenWaves = true;

    //Stores max enemies so that progress towards completion can be stored
    int maxEnemiesThisWave;

    //Camera movement keys
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    //Building hotkeys
    public KeyCode tierOneTurretKey = KeyCode.Alpha1;
    public KeyCode tierTwoTurretKey = KeyCode.Alpha2;
    public KeyCode tierThreeTurretKey = KeyCode.Alpha3;
    public KeyCode tierOneRepairKey = KeyCode.Alpha4;
    public KeyCode tierTwoRepairKey = KeyCode.Alpha5;
    public KeyCode tierOneWallKey = KeyCode.Alpha6;
    public KeyCode tierTwoWallKey = KeyCode.Alpha7;
    public KeyCode tierOneExtractorKey = KeyCode.Alpha8;
    public KeyCode tierTwoExtractorKey = KeyCode.Alpha9;
    public KeyCode tierThreeExtractorKey = KeyCode.Alpha0;

    //Basic hotkeys
    public KeyCode nextWaveKey = KeyCode.N;
    public KeyCode cancelKey = KeyCode.Escape;
    public KeyCode moveSelectionDownKey = KeyCode.DownArrow;
    public KeyCode moveSelectionUpKey = KeyCode.UpArrow;
    public KeyCode moveSelectionRightKey = KeyCode.RightArrow;
    public KeyCode moveSelectionLeftKey = KeyCode.LeftArrow;
    public KeyCode confirmKey = KeyCode.Return;

    public TMP_Text nextWaveLabel;

    //Enemy prefabs
    public GameObject baseEnemy;
    public GameObject fastEnemy;
    public GameObject swarmEnemy;
    public GameObject fastSwarmEnemy;
    public GameObject tankEnemy;
    public GameObject fastTankEnemy;
    public GameObject swarmTankEnemy;
    public GameObject fastSwarmTankEnemy;
    public GameObject deadlyEnemy;
    public GameObject fastDeadlyEnemy;
    public GameObject swarmDeadlyEnemy;
    public GameObject fastSwarmDeadlyEnemy;
    public GameObject tankDeadlyEnemy;
    public GameObject fastTankDeadlyEnemy;
    public GameObject swarmTankDeadlyEnemy;
    public GameObject fastSwarmTankDeadlyEnemy;
    public GameObject spammyEnemy;
    public GameObject fastSpammyEnemy;
    public GameObject swarmSpammyEnemy;
    public GameObject fastSwarmSpammyEnemy;
    public GameObject tankSpammyEnemy;
    public GameObject fastTankSpammyEnemy;
    public GameObject swarmTankSpammyEnemy;
    public GameObject fastSwarmTankSpammyEnemy;
    public GameObject deadlySpammyEnemy;
    public GameObject fastDeadlySpammyEnemy;
    public GameObject swarmDeadlySpammyEnemy;
    public GameObject fastSwarmDeadlySpammyEnemy;
    public GameObject tankDeadlySpammyEnemy;
    public GameObject fastTankDeadlySpammyEnemy;
    public GameObject swarmTankDeadlySpammyEnemy;
    public GameObject fastSwarmTankDeadlySpammyEnemy;
    public GameObject rangedEnemy;
    public GameObject fastRangedEnemy;
    public GameObject swarmRangedEnemy;
    public GameObject fastSwarmRangedEnemy;
    public GameObject tankRangedEnemy;
    public GameObject fastTankRangedEnemy;
    public GameObject swarmTankRangedEnemy;
    public GameObject fastSwarmTankRangedEnemy;
    public GameObject deadlyRangedEnemy;
    public GameObject fastDeadlyRangedEnemy;
    public GameObject swarmDeadlyRangedEnemy;
    public GameObject fastSwarmDeadlyRangedEnemy;
    public GameObject tankDeadlyRangedEnemy;
    public GameObject fastTankDeadlyRangedEnemy;
    public GameObject swarmTankDeadlyRangedEnemy;
    public GameObject fastSwarmTankDeadlyRangedEnemy;
    public GameObject spammyRangedEnemy;
    public GameObject fastSpammyRangedEnemy;
    public GameObject swarmSpammyRangedEnemy;
    public GameObject fastSwarmSpammyRangedEnemy;
    public GameObject tankSpammyRangedEnemy;
    public GameObject fastTankSpammyRangedEnemy;
    public GameObject swarmTankSpammyRangedEnemy;
    public GameObject fastSwarmTankSpammyRangedEnemy;
    public GameObject deadlySpammyRangedEnemy;
    public GameObject fastDeadlySpammyRangedEnemy;
    public GameObject swarmDeadlySpammyRangedEnemy;
    public GameObject fastSwarmDeadlySpammyRangedEnemy;
    public GameObject tankDeadlySpammyRangedEnemy;
    public GameObject fastTankDeadlySpammyRangedEnemy;
    public GameObject swarmTankDeadlySpammyRangedEnemy;
    public GameObject fastSwarmTankDeadlySpammyRangedEnemy;
    public GameObject auraEnemy;

    //Building UI storage
    public Image nextWaveBackground;
    public Image nextWaveFrame;

    public Image tierOneTurretBackground;
    public Image tierOneTurretFrame;
    public Image tierTwoTurretBackground;
    public Image tierTwoTurretFrame;
    public Image tierThreeTurretBackground;
    public Image tierThreeTurretFrame;

    public Image tierOneRepairBackground;
    public Image tierOneRepairFrame;
    public Image tierTwoRepairBackground;
    public Image tierTwoRepairFrame;

    public Image tierOneWallBackground;
    public Image tierOneWallFrame;
    public Image tierTwoWallBackground;
    public Image tierTwoWallFrame;

    public Image tierOneExtractorBackground;
    public Image tierOneExtractorFrame;
    public Image tierTwoExtractorBackground;
    public Image tierTwoExtractorFrame;
    public Image tierThreeExtractorBackground;
    public Image tierThreeExtractorFrame;

    //Economy text
    public TMP_Text budgetText;
    public TMP_Text incomeText;
    public TMP_Text energyText;

    //Menu colors
    public Color unselectedColor;
    public Color selectedColor;
    public Vector3 unavailableColor;
    public Vector3 availableColor;

    //Hotkey labels for buildings
    public TMP_Text tierOneTurretLabel;
    public TMP_Text tierTwoTurretLabel;
    public TMP_Text tierThreeTurretLabel;
    public TMP_Text tierOneRepairLabel;
    public TMP_Text tierTwoRepairLabel;
    public TMP_Text tierOneWallLabel;
    public TMP_Text tierTwoWallLabel;
    public TMP_Text tierOneExtractorLabel;
    public TMP_Text tierTwoExtractorLabel;
    public TMP_Text tierThreeExtractorLabel;

    //Settings data
    public int simplifiedSeed;
    public int outlineType = 0;

    public float enemyQuantity = 1;
    public float enemyStrength = 1;
    public float playerStrength = 1;
    public float playerHealth = 1;

    //Prevents exceptions due to data for in-game updates not being passed in yet
    private bool active = false;

    //Selling data storage
    public Canvas SellWindow;
    public TMP_Text sellText;
    public KeyCode sellKey = KeyCode.Backspace;

    //UI storage for the turret upgrade window
    public Canvas TurretUpgradeWindow;

    public TMP_Text turretSplashUpgradeText;
    public Image turretSplashFrame;
    public Image turretSplashBackground;

    public TMP_Text turretRangeUpgradeText;
    public Image turretRangeFrame;
    public Image turretRangeBackground;

    public TMP_Text turretDamageUpgradeText;
    public Image turretDamageFrame;
    public Image turretDamageBackground;

    public TMP_Text turretFirerateUpgradeText;
    public Image turretFirerateFrame;
    public Image turretFirerateBackground;

    public TMP_Text turretHealthUpgradeText;
    public Image turretHealthFrame;
    public Image turretHealthBackground;

    public TMP_Text turretDescriptionText;

    //UI storage for repair station upgrade window
    public Canvas RepairUpgradeWindow;

    public TMP_Text repairRangeUpgradeText;
    public Image repairRangeFrame;
    public Image repairRangeBackground;

    public TMP_Text repairHealingUpgradeText;
    public Image repairHealingFrame;
    public Image repairHealingBackground;

    public TMP_Text repairHealthUpgradeText;
    public Image repairHealthFrame;
    public Image repairHealthBackground;

    public TMP_Text repairDescriptionText;

    //UI storage for wall upgrade window
    public Canvas WallUpgradeWindow;

    public TMP_Text wallHealthUpgradeText;
    public Image wallHealthFrame;
    public Image wallHealthBackground;

    public TMP_Text wallHealingUpgradeText;
    public Image wallHealingFrame;
    public Image wallHealingBackground;

    public TMP_Text wallDescriptionText;

    //UI storage for repair station upgrade window
    public Canvas ExtractorUpgradeWindow;

    public TMP_Text extractorExtractionUpgradeText;
    public Image extractorExtractionFrame;
    public Image extractorExtractionBackground;

    public TMP_Text extractorEnergyUpgradeText;
    public Image extractorEnergyFrame;
    public Image extractorEnergyBackground;

    public TMP_Text extractorProtectionUpgradeText;
    public Image extractorProtectionFrame;
    public Image extractorProtectionBackground;

    public TMP_Text extractorHealthUpgradeText;
    public Image extractorHealthFrame;
    public Image extractorHealthBackground;

    public TMP_Text extractorDescriptionText;

    //Holds enemy types for easier access
    //GameObject[] allEnemies;
    GameObject[] tierOneEnemies;
    GameObject[] tierTwoEnemies;
    GameObject[] tierThreeEnemies;
    GameObject[] tierFourEnemies;
    GameObject[] tierFiveEnemies;

    public void Initialize(int simplifiedSeed, float enemyDifficulty, float playerPower, float playerEconomy)
    {
        //Sets the passed in data
        this.simplifiedSeed = simplifiedSeed;
        enemyQuantity = enemyDifficulty;
        enemyStrength = enemyDifficulty;
        playerStrength = playerPower;
        playerHealth = playerPower;

        EconomyManager.Instance.Initialize(playerEconomy);

        //Further events are identical between advanced and basic, so pass to another function
        midInit();
    }

    //Common initialization events
    private void midInit(bool generateSeeds = true)
    {
        //Sets the RNG seed so that you can generate the same map every time if you use the same seed
        BasicUtils.WrappedInitState(simplifiedSeed);

        //Sets defaults
        wave = 0;
        maxEnemiesThisWave = 1;
        paused = false;


        //Generates the map seeds if applicable
        if (generateSeeds)
        {
            TileManager.Instance.seedA = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedB = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedC = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedD = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedE = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedF = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedG = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedH = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
            TileManager.Instance.seedI = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
        }

        //Modifies starting values by the difficulty modifiers
        EconomyManager.Instance.MidInit();
        BuildingManager.Instance.MidInit();

        //Calls common initialization events
        Initialize();
    }

    public void Initialize(int simplifiedSeed, float enemyQuantity, float enemyStrength, float playerStrength, 
        float playerHealth, float playerCosts, float playerIncome, float energyProduction, float energyConsumption, bool generateSeeds = true)
    {
        //Sets the passed in data
        this.simplifiedSeed = simplifiedSeed;
        this.enemyQuantity = enemyQuantity;
        this.enemyStrength = enemyStrength;
        this.playerStrength = playerStrength;
        this.playerHealth = playerHealth;
        EconomyManager.Instance.Initialize(playerIncome, playerCosts, energyConsumption, energyProduction);

        //Further events are identical between advanced and basic, so pass to another function
        midInit(generateSeeds);
    }

    //Initialize with custom game settings
    public void Initialize(int simplifiedSeed, float enemyQuantity, float enemyStrength, float playerStrength, 
        float playerHealth, float playerCosts, float playerIncome, float energyProduction, float energyConsumption, 
        uint seedA, uint seedB, uint seedC, uint seedD, uint seedE, uint seedF, uint seedG, uint seedH, uint seedI, 
        int mapScaling, float traversalCutoff, float resourceScaling, float resourceCutoff, float aestheticScaling, 
        float aestheticACutoff, float aestheticBCutoff, float aestheticCCutoff, float aestheticDCutoff, 
        int mapStartSize, int mapExpansionRate)
    {
        //Seed data
        TileManager.Instance.seedA = seedA;
        TileManager.Instance.seedB = seedB;
        TileManager.Instance.seedC = seedC;
        TileManager.Instance.seedD = seedD;
        TileManager.Instance.seedE = seedE;
        TileManager.Instance.seedF = seedF;
        TileManager.Instance.seedG = seedG;
        TileManager.Instance.seedH = seedH;
        TileManager.Instance.seedI = seedI;

        //Aesthetic cutoffs
        TileManager.Instance.aestheticACutoff = aestheticACutoff;
        TileManager.Instance.aestheticBCutoff = aestheticBCutoff;
        TileManager.Instance.aestheticCCutoff = aestheticCCutoff;
        TileManager.Instance.aestheticDCutoff = aestheticDCutoff;

        //Scaling data
        TileManager.Instance.mapScaling = mapScaling;
        TileManager.Instance.resourceScaling = resourceScaling;
        TileManager.Instance.aestheticScaling = aestheticScaling;

        //Cutoff data
        TileManager.Instance.traversableCutoff = traversalCutoff;
        TileManager.Instance.resourceCutoff = resourceCutoff;

        //Other map data
        TileManager.Instance.size = mapStartSize;
        expansionRate = mapExpansionRate;

        Initialize(simplifiedSeed, enemyQuantity, enemyStrength, playerStrength, playerHealth, playerCosts, playerIncome, energyProduction, energyConsumption, false);
    }

    //Initialize from loaded file
    public void InitializeFromLoad(GameData data)
    {
        //Load seed data
        simplifiedSeed = data.simplifiedSeed;
        TileManager.Instance.seedA = data.seedA;
        TileManager.Instance.seedB = data.seedB;
        TileManager.Instance.seedC = data.seedC;
        TileManager.Instance.seedD = data.seedD;
        TileManager.Instance.seedE = data.seedE;
        TileManager.Instance.seedF = data.seedF;
        TileManager.Instance.seedG = data.seedG;
        TileManager.Instance.seedH = data.seedH;
        TileManager.Instance.seedI = data.seedI;
        TileManager.Instance.size = data.startSize;
        expansionRate = data.expansionRate;

        //Load difficulty settings
        enemyQuantity = data.enemyQuantity;
        enemyStrength = data.enemyStrength;
        playerStrength = data.playerStrength;
        playerHealth = data.playerHealth;

        //Default values
        paused = false;
        maxEnemiesThisWave = 1;

        //Load economic data
        EconomyManager.Instance.InitializeLoad(data);

        //Initiate RNG and move it to the appropriate RNG stream position
        BasicUtils.WrappedInitState(simplifiedSeed);
        BasicUtils.SpamRNGUntil(data.generatedNumbers);

        //Main initilization phase
        Initialize();
        betweenWaves = true;

        //Load other data
        wave = data.wave;
        TileManager.Instance.subbedTiles = new List<Vector2Int>(data.swappedTiles);
        
        //Ensure that all tiles have been generated properly
        for(int i = 0; i < wave * expansionRate; i++)
        {
            TileManager.Instance.Next();
        }

        //Ensure that all subbed tiles have been properly placed
        foreach(Vector2Int tile in TileManager.Instance.subbedTiles)
        {
            TileManager.Instance.Generate(tile);
        }

        BuildingManager.Instance.InitializeLoad(data);
        //updateEnergy();
    }

    //Initialize the manager since you cannot pass in most of the data until you open the main scene
    public void Initialize()
    {
        //Starts music
        MusicManager.Instance.PlayBetween();

        //Assigns enemies to proper tier storages
        tierOneEnemies = new GameObject[] { fastEnemy, swarmEnemy, tankEnemy, deadlyEnemy, spammyEnemy, rangedEnemy };
        tierTwoEnemies = new GameObject[] { fastSwarmEnemy, fastTankEnemy, fastDeadlyEnemy, fastSpammyEnemy,
            swarmTankEnemy, swarmDeadlyEnemy, swarmSpammyEnemy, tankDeadlyEnemy, tankSpammyEnemy, deadlySpammyEnemy,
            fastRangedEnemy, swarmRangedEnemy, tankRangedEnemy, deadlyRangedEnemy, spammyRangedEnemy};
        tierThreeEnemies = new GameObject[] { fastSwarmTankEnemy, fastSwarmDeadlyEnemy, fastSwarmSpammyEnemy,
            fastTankDeadlyEnemy, fastTankSpammyEnemy, fastDeadlySpammyEnemy, swarmTankDeadlyEnemy, swarmTankSpammyEnemy,
            swarmDeadlySpammyEnemy, tankDeadlySpammyEnemy, fastSwarmRangedEnemy, fastTankRangedEnemy, 
            fastDeadlyRangedEnemy, fastSpammyRangedEnemy, swarmTankRangedEnemy, swarmDeadlyRangedEnemy, 
            swarmSpammyRangedEnemy, tankDeadlyRangedEnemy, tankSpammyRangedEnemy, deadlySpammyRangedEnemy };
        tierFourEnemies = new GameObject[] { fastSwarmTankDeadlyEnemy, fastSwarmTankSpammyEnemy,
            fastSwarmDeadlySpammyEnemy, fastTankDeadlySpammyEnemy, swarmTankDeadlySpammyEnemy, fastSwarmTankRangedEnemy,
            fastSwarmDeadlyRangedEnemy, fastSwarmSpammyRangedEnemy, fastTankDeadlyRangedEnemy, fastTankSpammyRangedEnemy,
            fastDeadlySpammyRangedEnemy, swarmTankDeadlyRangedEnemy, swarmTankSpammyRangedEnemy, 
            swarmDeadlySpammyRangedEnemy, tankDeadlySpammyRangedEnemy };
        tierFiveEnemies = new GameObject[] { fastSwarmTankDeadlySpammyEnemy, fastSwarmTankDeadlyRangedEnemy,
            fastSwarmTankSpammyRangedEnemy, fastSwarmDeadlySpammyRangedEnemy, fastTankDeadlySpammyRangedEnemy,
            swarmTankDeadlySpammyRangedEnemy};

        //Combine tier storages to form overall storage
        //allEnemies = new GameObject[tierOneEnemies.Length + tierTwoEnemies.Length + tierThreeEnemies.Length + tierFourEnemies.Length + tierFiveEnemies.Length + 2];
        //allEnemies[0] = baseEnemy;
        //Array.Copy(tierOneEnemies, 0, allEnemies, 1, tierOneEnemies.Length);
        //Array.Copy(tierTwoEnemies, 0, allEnemies, tierOneEnemies.Length + 1, tierTwoEnemies.Length);
        //Array.Copy(tierThreeEnemies, 0, allEnemies, tierOneEnemies.Length + tierTwoEnemies.Length + 1, tierThreeEnemies.Length);
        //Array.Copy(tierFourEnemies, 0, allEnemies, tierOneEnemies.Length + tierTwoEnemies.Length + tierThreeEnemies.Length + 1, tierFourEnemies.Length);
        //Array.Copy(tierFiveEnemies, 0, allEnemies, tierOneEnemies.Length + tierTwoEnemies.Length + tierThreeEnemies.Length + tierFourEnemies.Length + 1, tierFiveEnemies.Length);
        //allEnemies[allEnemies.Length - 1] = fastSwarmTankDeadlySpammyEnemy;

        //Enable self
        active = true;

        //Hide non-starting windows
        TurretUpgradeWindow.enabled = false;
        RepairUpgradeWindow.enabled = false;
        WallUpgradeWindow.enabled = false;
        ExtractorUpgradeWindow.enabled = false;
        SellWindow.enabled = false;

        //Set hotkey text
        tierOneTurretLabel.text = BasicUtils.TranslateKey(tierOneTurretKey);
        tierTwoTurretLabel.text = BasicUtils.TranslateKey(tierTwoTurretKey);
        tierThreeTurretLabel.text = BasicUtils.TranslateKey(tierThreeTurretKey);
        tierOneRepairLabel.text = BasicUtils.TranslateKey(tierOneRepairKey);
        tierTwoRepairLabel.text = BasicUtils.TranslateKey(tierTwoRepairKey);
        tierOneWallLabel.text = BasicUtils.TranslateKey(tierOneWallKey);
        tierTwoWallLabel.text = BasicUtils.TranslateKey(tierTwoWallKey);
        tierOneExtractorLabel.text = BasicUtils.TranslateKey(tierOneExtractorKey);
        tierTwoExtractorLabel.text = BasicUtils.TranslateKey(tierTwoExtractorKey);
        tierThreeExtractorLabel.text = BasicUtils.TranslateKey(tierThreeExtractorKey);
        nextWaveLabel.text = BasicUtils.TranslateKey(nextWaveKey);
        sellText.text = $"Sell ({BasicUtils.TranslateKey(sellKey)})";

        //Ensure that wave background indicates that you can start a new wave
        nextWaveBackground.color = new Color(availableColor.x, availableColor.y, availableColor.z);

        betweenWaves = true;

        //Initialize the tilemanager for the same reason that this needs to be initialized
        TileManager.Instance.Initialize();


        BuildingManager.Instance.Initialize();
    }

    //Start a new wave
    public void NextWave()
    {
        //You cannot start a wave if you have not finished your current wave, might change this later
        if(betweenWaves)
        {
            FileManager.Instance.AutoSave(simplifiedSeed);

            MusicManager.Instance.PlayClick();

            //Change music
            MusicManager.Instance.StopBetween();
            MusicManager.Instance.PlayBattle();

            //Changes color to indicate that you cannot yet start a new wave
            nextWaveBackground.color = new Color(unavailableColor.x, unavailableColor.y, unavailableColor.z);

            //Clear out construction and upgrade data as well as selects new wave in the bottom menu
            Build(10);

            //Mark new wave
            wave++;

            //Mark during wave
            betweenWaves = false;

            //Culls checkpoints
            EnemyCheckpoint.positions.Clear();
            foreach (GameObject checkpoint in checkpoints)
            {
                Destroy(checkpoint);
            }
            checkpoints.Clear();

            //List where new enemies will be kept, holds them while they finish asynchronously generating
            List<AsyncInstantiateOperation<GameObject>> enemySpawns = new List<AsyncInstantiateOperation<GameObject>>();

            //Creates the correct amount of tier zero enemies for the wave
            int tierZeroEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave, 1 + (0.15f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave, 1 + (0.2f * enemyQuantity)) - 3.73719281885f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierZeroEnemyCount; i++)
            {
                //Get location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Start instantiation
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(baseEnemy, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.1f, 0.1f), BasicUtils.WrappedRandomRange(-0.1f, 0.1f)), Quaternion.identity);
                
                //Store reference to the handler
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier one enemies for the wave
            int tierOneEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 3, 1 + (0.125f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 3, 1 + (0.175f * enemyQuantity)) - 8.20970816332f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierOneEnemyCount; i++)
            {
                //Get location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Start instantiation
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierOneEnemies[BasicUtils.WrappedRandomRange(0, tierOneEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.1f, 0.1f), BasicUtils.WrappedRandomRange(-0.1f, 0.1f)), Quaternion.identity);
                
                //Store reference to the handler
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier two enemies for the wave
            int tierTwoEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 9, 1 + (0.1f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 9, 1 + (0.15f * enemyQuantity)) - 17.4204124379f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierTwoEnemyCount; i++)
            {
                //Get location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Start instantiation
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierTwoEnemies[BasicUtils.WrappedRandomRange(0, tierTwoEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.1f, 0.1f), BasicUtils.WrappedRandomRange(-0.1f, 0.1f)), Quaternion.identity);
                
                //Store reference to the handler
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier three enemies for the wave
            int tierThreeEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 21, 1 + (0.075f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 21, 1 + (0.125f * enemyQuantity)) - 35.7057078279f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierThreeEnemyCount; i++)
            {
                //Get location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Start instantiation
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierThreeEnemies[BasicUtils.WrappedRandomRange(0, tierThreeEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.1f, 0.1f), BasicUtils.WrappedRandomRange(-0.1f, 0.1f)), Quaternion.identity);
                
                //Store reference to the handler
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier four enemies for the wave
            int tierFourEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 45, 1 + (0.05f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 45, 1 + (0.1f * enemyQuantity)) - 70.6912011617f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierFourEnemyCount; i++)
            {
                //Get location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Start instantiation
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierFourEnemies[BasicUtils.WrappedRandomRange(0, tierFourEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.1f, 0.1f), BasicUtils.WrappedRandomRange(-0.1f, 0.1f)), Quaternion.identity);
                
                //Store reference to handler
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier five enemies for the wave
            int tierFiveEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 93, 1 + (0.025f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 93, 1 + (0.075f * enemyQuantity)) - 135.18906847f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierFiveEnemyCount; i++)
            {
                //Get location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Start instantiation
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierFiveEnemies[BasicUtils.WrappedRandomRange(0, tierFiveEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.1f, 0.1f), BasicUtils.WrappedRandomRange(-0.1f, 0.1f)), Quaternion.identity);
                
                //Store reference to handler
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier six enemies for the wave
            int tierSixEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 189, 1 + (0.01f * enemyQuantity - 0.01f)), 0) * enemyQuantity);
            for (int i = 0; i < tierFiveEnemyCount; i++)
            {
                //Get location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Start instantiation
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(fastSwarmTankDeadlySpammyRangedEnemy, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.1f, 0.1f), BasicUtils.WrappedRandomRange(-0.1f, 0.1f)), Quaternion.identity);
                
                //Store reference to handler
                enemySpawns.Add(createdEnemy);
            }

            //Store finished enemies
            List<Enemy> createdEnemies = new List<Enemy>();

            //Creates the correct amount of aura enemies for the wave
            int auraEnemyCount = (int)(enemyQuantity * wave / 10);
            for(int i = 0; i <auraEnemyCount; i++)
            {
                //Gets the location to instantiate at
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                
                //Instantiates the enemy
                AuraEnemy createdEnemy = Instantiate(auraEnemy, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity).GetComponentInChildren<AuraEnemy>();
                
                //Sets up the aura enemy
                createdEnemy.auraCount = (int)(auraEnemyCount * 0.2) + 1;

                //Stores finished enemy
                createdEnemies.Add(createdEnemy);
            }

            //Goes through all of the started enemies
            foreach(AsyncInstantiateOperation<GameObject> createdEnemy in enemySpawns)
            {
                //Ensures that all of the enemies are finished
                if (!createdEnemy.isDone)
                {
                    createdEnemy.WaitForCompletion();
                }
                //Adds the finished enemies to the enemy storage
                createdEnemies.Add(createdEnemy.Result[0].GetComponentInChildren<Enemy>());
            }

            //Stores handlers for pathfinding processes
            List<Task<(bool, Dictionary<Vector2Int, NavNode>)>> pathFinder = new List<Task<(bool, Dictionary<Vector2Int, NavNode>)>>();

            //Ensures that it will keep trying to generate paths until it finds one for every enemy
            while (createdEnemies.Count > 0)
            {
                //Starts pathfinding for ever single enemy
                foreach (Enemy enemy in createdEnemies)
                {
                    //Gets data for path generation
                    Vector2Int goal = (Vector2Int)TileManager.Instance.TraversableTilemap.WorldToCell(enemy.transform.position);
                    Vector3 position = enemy.transform.position;

                    //Begins finding the path and adds the handler to the handler storage
                    pathFinder.Add(Task<(bool, Dictionary<Vector2Int, NavNode>)>.Factory.StartNew(() => Enemy.FindPath(goal, enemy.movementSpeed, enemy.damage, enemy.firerate, position)));

                }
                //Holds enemies that did not find a path
                List<Enemy> enemyContainer = new List<Enemy>();
                for(int i = 0; i < createdEnemies.Count; i++)
                {
                    //Check to see if a path was found
                    if (pathFinder[i].Result.Item1)
                    {
                        //Adds the enemy to the list of finished enemies
                        currentEnemies.Add(createdEnemies[i]);

                        //Generate a series of checkpoints to guide the enemy
                        EnemyCheckpoint checkpoint = createdEnemies[i].GeneratePath(pathFinder[i].Result.Item2, (Vector2Int)TileManager.Instance.TraversableTilemap.WorldToCell(createdEnemies[i].transform.position));

                        //Clears the pathfinding dictionary in order t fix a memory leak
                        pathFinder[i].Result.Item2[(Vector2Int)TileManager.Instance.TraversableTilemap.WorldToCell(createdEnemies[i].transform.position)].Destroy();
                        
                        //Duplicates swarmer enemies
                        if (createdEnemies[i].swarmer)
                        {
                            Enemy duplicateEnemy = Instantiate(createdEnemies[i].transform.parent.gameObject, TileManager.Instance.TraversableTilemap.CellToWorld(TileManager.Instance.TraversableTilemap.WorldToCell(createdEnemies[i].transform.position)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity).GetComponentInChildren<Enemy>();
                            currentEnemies.Add(duplicateEnemy);
                            
                            //Activate guides on duplicated enemy
                            duplicateEnemy.ActivatePath(checkpoint);
                        }
                        //Activate guides on original enemy
                        createdEnemies[i].ActivatePath(checkpoint);
                    }
                    //If a path was not found, try again
                    else
                    {
                        //Change location since keeping it in the same place will not result in a path being found
                        int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                        createdEnemies[i].transform.parent.position = TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f));
                        
                        //Add to list of enemies with invalid paths
                        enemyContainer.Add(createdEnemies[i]);
                    }
                }
                //Replace the enemy storage with the enemies with no path
                createdEnemies = enemyContainer;

                //Clear pathfinding handles
                pathFinder.Clear();
            }
            //Max enemies in order to ensure that you can tell how many more you need to kill to do the next wave
            maxEnemiesThisWave = currentEnemies.Count;

            //Expand the map two tiles
            for (int i = 0; i < expansionRate; i++)
            {
                TileManager.Instance.Next();
            }
        }
    }
    public void Build(int type)
    {
        //Change the frames in the UI to show which you have selected
        tierOneTurretFrame.color = type == 0 ? selectedColor : unselectedColor;
        tierTwoTurretFrame.color = type == 1 ? selectedColor : unselectedColor;
        tierThreeTurretFrame.color = type == 2 ? selectedColor : unselectedColor;
        tierOneRepairFrame.color = type == 3 ? selectedColor : unselectedColor;
        tierTwoRepairFrame.color = type == 4 ? selectedColor : unselectedColor;
        tierOneWallFrame.color = type == 5 ? selectedColor : unselectedColor;
        tierTwoWallFrame.color = type == 6 ? selectedColor : unselectedColor;
        tierOneExtractorFrame.color = type == 7 ? selectedColor : unselectedColor;
        tierTwoExtractorFrame.color = type == 8 ? selectedColor : unselectedColor;
        tierThreeExtractorFrame.color = type == 9 ? selectedColor : unselectedColor;
        nextWaveFrame.color = type == 10 ? selectedColor : unselectedColor;

        BuildingManager.Instance.Build(type);

    }

    public void OpenWindow(int type, PlayerBuilding building)
    {
        switch(type)
        {
            case 0:
                {
                    //Enables the turret upgrade window
                    TurretUpgradeWindow.enabled = true;

                    //Writes the description so you know what you have selected
                    turretDescriptionText.text = building.GetDescription();

                    //Ensures that all of the upgrade panels have accurate information
                    UpdateUpgradeCost(0, 0);
                    UpdateUpgradeCost(0, 1);
                    UpdateUpgradeCost(0, 2);
                    UpdateUpgradeCost(0, 3);
                    UpdateUpgradeCost(0, 4);
                    break;
                }
            case 1:
                {
                    //Enables the repair station upgrade window
                    RepairUpgradeWindow.enabled = true;

                    //Writes the description so that you know what you have selected
                    repairDescriptionText.text = building.GetDescription();

                    //Ensures that all of the upgrade panels have accurate information
                    UpdateUpgradeCost(1, 0);
                    UpdateUpgradeCost(1, 1);
                    UpdateUpgradeCost(1, 2);
                    break;
                }
            case 2:
                {
                    //Enables the wall upgrade window
                    WallUpgradeWindow.enabled = true;

                    //Writes the description so that you know what you have selected
                    wallDescriptionText.text = building.GetDescription();

                    //Ensures that all of the upgrade panels have accurate information
                    UpdateUpgradeCost(2, 0);
                    UpdateUpgradeCost(2, 1);
                    break;
                }
            case 3:
                {
                    //Enables the extractor upgrade window
                    ExtractorUpgradeWindow.enabled = true;

                    //Writes the description so that you know what you have selected
                    extractorDescriptionText.text = building.GetDescription();

                    //Ensures that all of the upgrade panels have accurate information
                    UpdateUpgradeCost(3, 0);
                    UpdateUpgradeCost(3, 1);
                    UpdateUpgradeCost(3, 2);
                    UpdateUpgradeCost(3, 3);
                    break;
                }
            case 4:
                {
                    SellWindow.enabled = true;
                    break;
                }
            default:
                {
                    TurretUpgradeWindow.enabled = false;
                    RepairUpgradeWindow.enabled = false;
                    WallUpgradeWindow.enabled = false;
                    ExtractorUpgradeWindow.enabled = false;
                    SellWindow.enabled = false;
                    break;
                }
        }
    }

    
    //Changes the panel to reflect accurate building data
    public void UpdateUpgradeCost(int buildingType, int upgradeType)
    {
        //Ensures that it is updating the correct building type's panel
        switch(buildingType)
        {
            //Turrets
            case 0:
                {
                    //Grabs the exact turret that you want to get data from
                    Turret turret = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<Turret>();

                    //Switch based on the specific data you want to update
                    switch(upgradeType)
                    {
                        //Turret splash damage range
                        case 0:
                            {
                                turretSplashUpgradeText.text = turret.IsAligned() ? turret.GetUpgradeCost(upgradeType) >= 0 ? $"Splash Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Splash Range:\n{turret.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Turret range
                        case 1:
                            {
                                turretRangeUpgradeText.text = turret.IsAligned() ? turret.GetUpgradeCost(upgradeType) >= 0 ? $"Range Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Range:\n{turret.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Turret damage
                        case 2:
                            {
                                turretDamageUpgradeText.text = turret.IsAligned() ? turret.GetUpgradeCost(upgradeType) >= 0 ? $"Damage Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Damage:\n{turret.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Turret firerate
                        case 3:
                            {
                                turretFirerateUpgradeText.text = turret.IsAligned() ? turret.GetUpgradeCost(upgradeType) >= 0 ? $"Firerate Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Firerate:\n{turret.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Turret health
                        case 4:
                            {
                                turretHealthUpgradeText.text = turret.IsAligned() ? turret.GetUpgradeCost(upgradeType) >= 0 ? $"Health Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Health:\n{turret.GetUpgradeEffects(upgradeType)}";
                                break;
                            }

                    }
                    break;
                }
            //Repair Stations
            case 1:
                {
                    //Grabs the exact repair station that you want to get data from
                    RepairStation repairStation = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<RepairStation>();

                    //Switch based on the specific data you want to update
                    switch(upgradeType)
                    {
                        //Repair station range
                        case 0:
                            {
                                repairRangeUpgradeText.text = repairStation.IsAligned() ? repairStation.GetUpgradeCost(upgradeType) >= 0 ? $"Range Upgrade:\n{repairStation.GetUpgradeEffects(upgradeType)}\nCost:\n{repairStation.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Range:\n{repairStation.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Repair station healing
                        case 1:
                            {
                                repairHealingUpgradeText.text = repairStation.IsAligned() ? repairStation.GetUpgradeCost(upgradeType) >= 0 ? $"Healing Upgrade:\n{repairStation.GetUpgradeEffects(upgradeType)}\nCost:\n{repairStation.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Healing:\n{repairStation.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Repair station health
                        case 2:
                            {
                                repairHealthUpgradeText.text = repairStation.IsAligned() ? repairStation.GetUpgradeCost(upgradeType) >= 0 ? $"Health Upgrade:\n{repairStation.GetUpgradeEffects(upgradeType)}\nCost:\n{repairStation.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Health:\n{repairStation.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                    }
                    break;
                }
            //Walls
            case 2:
                {
                    //Grabs the exact wall you want to get data from
                    Wall wall = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<Wall>();

                    //Only two upgrade options, so I am using an if instead of a switch
                    //Health
                    if(upgradeType == 0)
                    {
                        wallHealthUpgradeText.text = wall.IsAligned() ? wall.GetUpgradeCost(upgradeType) >= 0 ? $"Health Upgrade:\n{wall.GetUpgradeEffects(upgradeType)}\nCost:\n{wall.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Health:\n{wall.GetUpgradeEffects(upgradeType)}";
                    }
                    //Healing boost (makes it so that walls heal more from repair stations)
                    else
                    {
                        wallHealingUpgradeText.text = wall.IsAligned() ? wall.GetUpgradeCost(upgradeType) >= 0 ? $"Healing Upgrade:\n{wall.GetUpgradeEffects(upgradeType)}\nCost:\n{wall.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Healing:\n{wall.GetUpgradeEffects(upgradeType)}";
                    }
                    break;
                }
            //Extractors
            case 3:
                {
                    //Grabs the exact extractor that you want to get data from
                    ResourceExtractor extractor = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<ResourceExtractor>();

                    //Switch based on the specific data you want to update
                    switch (upgradeType)
                    {
                        //Extractor range
                        case 0:
                            {
                                extractorExtractionUpgradeText.text = extractor.IsAligned() ? extractor.GetUpgradeCost(upgradeType) >= 0 ? $"Extraction Upgrade:\n{extractor.GetUpgradeEffects(upgradeType)}\nCost:\n{extractor.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Extraction:\n{extractor.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Extractor healing
                        case 1:
                            {
                                extractorEnergyUpgradeText.text = extractor.IsAligned() ? extractor.GetUpgradeCost(upgradeType) >= 0 ? $"Energy Upgrade:\n{extractor.GetUpgradeEffects(upgradeType)}\nCost:\n{extractor.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Energy:\n{extractor.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Extractor protection
                        case 2:
                            {
                                extractorProtectionUpgradeText.text = extractor.IsAligned() ? extractor.GetUpgradeCost(upgradeType) >= 0 ? $"Protection Upgrade:\n{extractor.GetUpgradeEffects(upgradeType)}\nCost:\n{extractor.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Protection:\n{extractor.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                        //Extractor health
                        case 3:
                            {
                                extractorHealthUpgradeText.text = extractor.IsAligned() ? extractor.GetUpgradeCost(upgradeType) >= 0 ? $"Health Upgrade:\n{extractor.GetUpgradeEffects(upgradeType)}\nCost:\n{extractor.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE" : $"Health:\n{extractor.GetUpgradeEffects(upgradeType)}";
                                break;
                            }
                    }
                    break;
                }
        }
    }

    //Updates the upgrade panels to show which upgrade you have selected
    public void UpdateSelection(int selection, GameObject building)
    {
        //Identifies type of building you are upgrading
        Turret turret = building.GetComponentInChildren<Turret>();
        if (turret != null)
        {
            //Goes through all of the turret upgrade panel frames and sets them to the appropriate color
            turretSplashFrame.color = selection == 0 ? selectedColor : unselectedColor;
            turretRangeFrame.color = selection == 1 ? selectedColor : unselectedColor;
            turretDamageFrame.color = selection == 2 ? selectedColor : unselectedColor;
            turretFirerateFrame.color = selection == 3 ? selectedColor : unselectedColor;
            turretHealthFrame.color = selection == 4 ? selectedColor : unselectedColor;
        }
        else
        {
            RepairStation repair = building.GetComponentInChildren<RepairStation>();
            if (repair != null)
            {
                //Goes through all of the repair station upgrade panel frames and sets them to the appropriate color
                repairRangeFrame.color = selection == 0 ? selectedColor : unselectedColor;
                repairHealingFrame.color = selection == 1 ? selectedColor : unselectedColor;
                repairHealthFrame.color = selection == 2 ? selectedColor : unselectedColor;
            }
            else
            {
                Wall wall = building.GetComponentInChildren<Wall>();
                if (wall != null)
                {
                    //Goes through all of the wall upgrade panel frames and sets them to the appropriate color
                    wallHealthFrame.color = selection == 0 ? selectedColor : unselectedColor;
                    wallHealingFrame.color = selection == 1 ? selectedColor : unselectedColor;
                }
                else
                {
                    ResourceExtractor extractor = building.GetComponentInChildren<ResourceExtractor>();
                    if(extractor != null)
                    {
                        //Goes through all of the extractor upgrade panel frames and sets them to the appropriate color
                        extractorExtractionFrame.color = selection == 0 ? selectedColor : unselectedColor;
                        extractorEnergyFrame.color = selection == 1 ? selectedColor : unselectedColor;
                        extractorProtectionFrame.color = selection == 2 ? selectedColor : unselectedColor;
                        extractorHealthFrame.color = selection == 3 ? selectedColor : unselectedColor;
                    }
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //Most actions only happen once you start a map
        if (active)
        {
            float budget = EconomyManager.Instance.budget;

            //Sets the colors of the backgrounds to show you how close you are to being able to afford them
            tierOneTurretBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[0], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[0], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[0], 0, 1)));
            tierTwoTurretBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[1], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[1], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[1], 0, 1)));
            tierThreeTurretBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[2], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[2], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[2], 0, 1)));
            tierOneRepairBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[3], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[3], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[3], 0, 1)));
            tierTwoRepairBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[4], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[4], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[4], 0, 1)));
            tierOneWallBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[5], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[5], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[5], 0, 1)));
            tierTwoWallBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[6], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[6], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[6], 0, 1)));
            tierOneExtractorBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[7], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[7], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[7], 0, 1)));
            tierTwoExtractorBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[8], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[8], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[8], 0, 1)));
            tierThreeExtractorBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[9], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[9], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / EconomyManager.Instance.budgetCosts[9], 0, 1)));

            //Only bother updating the turret upgrade backgrounds if you have the window open
            if (TurretUpgradeWindow.enabled)
            {
                //Gets a reference to the specific turret you have selected
                Turret turret = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<Turret>();

                //Sets the background color based on how close you are to being able to afford it
                turretSplashBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(0), 0, 1)));
                turretRangeBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(1), 0, 1)));
                turretDamageBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(2), 0, 1)));
                turretFirerateBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(3), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(3), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(3), 0, 1)));
                turretHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(4), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(4), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(4), 0, 1)));
            }
            //Only bother updating the repair station upgrade backgrounds if you have the window open, also skip this check if you know that another window is open
            else if (RepairUpgradeWindow.enabled)
            {
                //Gets a reference to the specific repair station you have selected
                RepairStation repair = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<RepairStation>();

                //Sets the background color based on how close you are to being able to afford it
                repairRangeBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / repair.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / repair.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / repair.GetUpgradeCost(0), 0, 1)));
                repairHealingBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / repair.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / repair.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / repair.GetUpgradeCost(1), 0, 1)));
                repairHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / repair.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / repair.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / repair.GetUpgradeCost(2), 0, 1)));
            }
            //Only bother updating the wall upgrade backgrounds if you have the window open, also skip this check if you know that another window is open
            else if (WallUpgradeWindow.enabled)
            {
                //Gets a reference to the specific wall you have selected
                Wall wall = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<Wall>();

                //Sets the background color based on how close you are to being able to afford it
                wallHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)));
                wallHealingBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)));
            }
            //Only bother updating the extractor upgrade backgrounds if you have the window open, also skip this check if you know that another window is open
            else if (ExtractorUpgradeWindow.enabled)
            {
                //Gets a regerence to the specific extractor you have selected
                ResourceExtractor extractor = BuildingManager.Instance.selectedBuilding.GetComponentInChildren<ResourceExtractor>();

                //Sets the backgroun color based on how close you are to being able to afford it
                extractorExtractionBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / extractor.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / extractor.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / extractor.GetUpgradeCost(0), 0, 1)));
                extractorEnergyBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / extractor.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / extractor.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / extractor.GetUpgradeCost(1), 0, 1)));
                extractorProtectionBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / extractor.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / extractor.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / extractor.GetUpgradeCost(2), 0, 1)));
                extractorHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / extractor.GetUpgradeCost(3), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / extractor.GetUpgradeCost(3), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / extractor.GetUpgradeCost(3), 0, 1)));

            }
            //Updates your budget text to match your actual budget
            budgetText.text = $"Budget: {budget:F2}";

            //Allows you to zoom in and out using mouse scrollwheel
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                Camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 25 * Time.deltaTime * Camera.orthographicSize;
            }
            //Stores movement data for the camera
            Vector3 cameraModifier = Vector3.zero;

            //Moves your camera up
            if (Input.GetKey(forwardKey))
            {
                cameraModifier.y += Camera.orthographicSize * Time.deltaTime;
            }
            //Moves your camera left
            if (Input.GetKey(leftKey))
            {
                cameraModifier.x -= Camera.orthographicSize * Time.deltaTime;
            }
            //Moves your camera down
            if (Input.GetKey(backKey))
            {
                cameraModifier.y -= Camera.orthographicSize * Time.deltaTime;
            }
            //Moves your damera right
            if (Input.GetKey(rightKey))
            {
                cameraModifier.x += Camera.orthographicSize * Time.deltaTime;
            }
            //Uses the stored camera movement data to move the camera
            Camera.transform.position += cameraModifier;

            if (betweenWaves)
            {
                //Pause menu
                if(Input.GetKeyDown(cancelKey) && BuildingManager.Instance.selectedConstruction == null && BuildingManager.Instance.selectedBuilding == null)
                {
                    if(paused)
                    {
                        MenuManager.Instance.UnPause();
                    }
                    else
                    {
                        MenuManager.Instance.Pause();
                    }
                }
                if (!paused)
                {
                    //Allows you to cancel construction
                    if (BuildingManager.Instance.selectedConstruction != null && (Input.GetKeyDown(cancelKey) || budget < EconomyManager.Instance.budgetCosts[BuildingManager.Instance.selectedConstructionIndex]))
                    {
                        BuildingManager.Instance.CancelConstruction();
                    }
                    //Allows you to close the upgrade window
                    if ((Input.GetKeyDown(cancelKey) || Input.GetMouseButtonDown(1)) && BuildingManager.Instance.selectedBuilding != null)
                    {
                        BuildingManager.Instance.CancelUpgrade();
                    }
                    //Allows you to start the next wave with hotkeys
                    if (Input.GetKeyDown(nextWaveKey) || (BuildingManager.Instance.selectedConstructionIndex == 10 && Input.GetKeyDown(confirmKey)))
                    {
                        NextWave();
                        Debug.Log("Finished starting wave");
                    }
                    //Move the selected construction one to the right
                    if (Input.GetKeyDown(moveSelectionRightKey))
                    {
                        BuildingManager.Instance.ChangeConstructionSelection(1);
                    }
                    //Move the selected construction one to the left
                    if (Input.GetKeyDown(moveSelectionLeftKey))
                    {
                        BuildingManager.Instance.ChangeConstructionSelection(-1);
                    }
                    //Stores the tile that your mouse is above
                    Vector2Int hoveredTile = (Vector2Int)TileManager.Instance.TraversableTilemap.WorldToCell(Camera.ScreenToWorldPoint(Input.mousePosition));

                    //Actions for when you have a selected building
                    if (BuildingManager.Instance.selectedBuilding != null)
                    {
                        //Moves selected upgrade down when you use your down navigation key
                        if (Input.GetKeyDown(moveSelectionDownKey))
                        {
                            BuildingManager.Instance.ChangeUpgradeSelection(1);
                        }
                        //Moves selected upgrade up when you use your up navigation key
                        if (Input.GetKeyDown(moveSelectionUpKey))
                        {
                            BuildingManager.Instance.ChangeUpgradeSelection(-1);
                        }
                        //Allows you to upgrade using hotkeys
                        if (Input.GetKeyDown(confirmKey))
                        {
                            BuildingManager.Instance.Upgrade();
                        }
                        //Allows you to sell buildings with hotkeys
                        if (Input.GetKeyDown(sellKey))
                        {
                            BuildingManager.Instance.Sell();
                        }
                    }
                    //Do things if you either have left clicked or used the confirm button and you are not above the construction panel
                    if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(confirmKey)) && Input.mousePosition.y > Screen.height * 0.1851851852 && betweenWaves)
                    {
                        //Checks the place you are over to see if there is a building there, also ensures that you are not changing selections when upgrading with mouse
                        if (BuildingManager.Instance.playerBuildings.TryGetValue(hoveredTile, out GameObject building) && (Input.GetMouseButtonDown(0) || BuildingManager.Instance.selectedBuilding == null) && Input.mousePosition.x < Screen.width - Screen.width * 0.2375)
                        {
                            //Clears building data
                            Build(-1);
                            BuildingManager.Instance.SelectBuilding(building);
                        }
                        //If you can place a building, do so
                        else
                        {
                            BuildingManager.Instance.AttemptPlacement(hoveredTile);
                        }
                    }
                    //Allows you to cancel constructions using right click
                    if (Input.GetMouseButtonDown(1))
                    {
                        BuildingManager.Instance.CancelConstruction();
                    }
                    BuildingManager.Instance.TriggerFollow(hoveredTile);
                    //Select tier one turret with hotkey
                    if (Input.GetKeyDown(tierOneTurretKey))
                    {
                        Build(0);
                    }
                    //Select tier two turret with hotkey
                    if (Input.GetKeyDown(tierTwoTurretKey))
                    {
                        Build(1);
                    }
                    //Select tier three turret with hotkey
                    if (Input.GetKeyDown(tierThreeTurretKey))
                    {
                        Build(2);
                    }
                    //Select tier one repair station with hotkey
                    if (Input.GetKeyDown(tierOneRepairKey))
                    {
                        Build(3);
                    }
                    //Select tier two repair station with hotkey
                    if (Input.GetKeyDown(tierTwoRepairKey))
                    {
                        Build(4);
                    }
                    //Select tier one wall with hotkey
                    if (Input.GetKeyDown(tierOneWallKey))
                    {
                        Build(5);
                    }
                    //Select tier two wall with hotkey
                    if (Input.GetKeyDown(tierTwoWallKey))
                    {
                        Build(6);
                    }
                    //Select tier one resource extractor with hotkey
                    if (Input.GetKeyDown(tierOneExtractorKey))
                    {
                        Build(7);
                    }
                    //Select tier two resource extractor with hotkey
                    if (Input.GetKeyDown(tierTwoExtractorKey))
                    {
                        Build(8);
                    }
                    //Select tier three resource extractor with hotkey
                    if (Input.GetKeyDown(tierThreeExtractorKey))
                    {
                        Build(9);
                    }
                }
            }
            //Only apply income if you are actively in a wave
            else
            {
                EconomyManager.Instance.TickIncome();
            }
        }
	}

    //Function to remove an enemy in order to improve performance of constant checks
    public void KillEnemy(Enemy enemy)
    {
        //Removes from stored enemies
        currentEnemies.Remove(enemy);

        //Ensures no remaining null enemies
        currentEnemies.RemoveAll(item => item == null);

        //Checks to see if the wave is over
        betweenWaves = currentEnemies.Count == 0;

        //If wave over swap music
        if(betweenWaves)
        {
            MusicManager.Instance.StopBattle();
            MusicManager.Instance.PlayBetween();
        }

        //Sets the wave background to show what the remaining enemy status is
        nextWaveBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)));
    }

    //Gets the data so that it can be saved
    public GameData GetSaveData()
    {
        GameData data = new GameData();

        //Seed data
        data.simplifiedSeed = simplifiedSeed;
        data.seedA = TileManager.Instance.seedA;
        data.seedB = TileManager.Instance.seedB;
        data.seedC = TileManager.Instance.seedC;
        data.seedD = TileManager.Instance.seedD;
        data.seedE = TileManager.Instance.seedE;
        data.seedF = TileManager.Instance.seedF;
        data.seedG = TileManager.Instance.seedG;
        data.seedH = TileManager.Instance.seedH;
        data.seedI = TileManager.Instance.seedI;
        data.startSize = TileManager.Instance.size;
        data.expansionRate = expansionRate;


        //Apply building data
        BuildingManager.Instance.GetSaveData(ref data);
        //Economy info
        EconomyManager.Instance.GetSaveData(ref data);
        //Other
        data.wave = wave;
        data.generatedNumbers = BasicUtils.generatedNumbers;
        data.swappedTiles = TileManager.Instance.subbedTiles.ToArray();

        //Difficulty
        data.enemyQuantity = enemyQuantity;
        data.enemyStrength = enemyStrength;
        data.playerStrength = playerStrength;
        data.playerHealth = playerHealth;

        return data;
    }

    //Deactivate the manager in order to make a new map
    public void Deactivate()
    {
        active = false;

        EconomyManager.Instance.Deactivate();
        BuildingManager.Instance.Deactivate();
        TileManager.Instance.Deactivate();
    }
}