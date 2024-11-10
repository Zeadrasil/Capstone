using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

//GameManager manages everything inside the game that is not directly related to the main menu or tiles
public class GameManager : Singleton<GameManager>
{
    //Keeps track of upgrade related data
    private int selectedUpgrade;
    private GameObject selectedBuilding;

    //Building array, null because prefabs are not yet passed in when this is created so it need to be done in Initialize()
    private GameObject[] buildings = null;

    //Other data
    public PlayerBase PlayerBase;
    public GameObject EnemyCheckpointPrefab;
    public Camera Camera;
    public bool paused;

    //Stores data for building new buildings
    private GameObject selectedConstruction;
    private int selectedConstructionIndex;

    //Cost data for new buildings
    public float[] budgetCosts;
    public float[] energyCosts;

    public Dictionary<Vector2Int, GameObject> playerBuildings = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, float> playerHealths = new Dictionary<Vector2Int, float>();
    public Dictionary<Vector2Int, float> playerExtractionData = new Dictionary<Vector2Int, float>();
    public Dictionary<Vector2Int, float> playerDamageData = new Dictionary<Vector2Int, float>();
    public Dictionary<Vector2Int, float> playerRepairData = new Dictionary<Vector2Int, float>();

    //Economy data
    public float budget;
    private float income;
    public float energy { get; private set; }
    public float usedEnergy { get; private set; }
    public float energyDeficit;
    public PlayerBuilding mostRecentEnergyDecrease;

    //Stores which wave you are on
    public int wave;

    //Stores enemies so that it is known when you have killed them all
    private List<Enemy> currentEnemies = new List<Enemy>();

    //Stores checkpoints so that they can be culled for the new wave
    public List<GameObject> checkpoints = new List<GameObject>();
    public bool betweenWaves = true;

    //Stores TileManager instance because it is used very often and this is shorter to type
    private TileManager tileManager;

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

    //Building prefabs
    public GameObject turretTierOne;
    public GameObject turretTierTwo;
    public GameObject turretTierThree;
    public GameObject repairTierOne;
    public GameObject repairTierTwo;
    public GameObject wallTierOne;
    public GameObject wallTierTwo;
    public GameObject extractorTierOne;
    public GameObject extractorTierTwo;
    public GameObject extractorTierThree;

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

    public float enemyQuantity = 1;
    public float enemyStrength = 1;
    public float playerStrength = 1;
    public float playerHealth = 1;
    public float playerIncome = 1;
    public float playerCosts = 1;
    public float energyProduction = 1;
    public float energyConsumption = 1;

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

    //Sets the TileManager instance
    void Start()
    {
        tileManager = TileManager.Instance;
    }

    public void Initialize(int simplifiedSeed, float enemyDifficulty, float playerPower, float playerEconomy)
    {
        //Sets the passed in data
        this.simplifiedSeed = simplifiedSeed;
        enemyQuantity = enemyDifficulty;
        enemyStrength = enemyDifficulty;
        playerStrength = playerPower;
        playerHealth = playerPower;
        playerIncome = playerEconomy;
        playerCosts = 1 / playerEconomy;
        energyConsumption = 1 / playerEconomy;
        energyProduction = playerEconomy;

        //Further events are identical between advanced and basic, so pass to another function
        midInit();
    }

    private void midInit()
    {
        //Sets the RNG seed so that you can generate the same map every time if you use the same seed
        BasicUtils.WrappedInitState(simplifiedSeed);

        //Sets defaults
        wave = 0;
        budgetCosts = new float[] { 10, 15, 25, 10, 15, 10, 15, 10, 15, 25 };
        energyCosts = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        energy = 10;
        maxEnemiesThisWave = 1;
        usedEnergy = 0;
        income = 0;
        energyDeficit = 0;
        mostRecentEnergyDecrease = null;
        playerBuildings.Clear();
        paused = false;
        selectedUpgrade = 0;
        selectedConstructionIndex = -1;
        selectedBuilding = null;
        selectedConstruction = null;
        budget = 100;


        //Generates the map seeds
        tileManager.seedA = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
        tileManager.seedB = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
        tileManager.seedC = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
        tileManager.seedD = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
        tileManager.seedE = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;
        tileManager.seedF = ((uint)BasicUtils.WrappedRandomRange(int.MinValue, int.MaxValue)) + int.MaxValue;

        //Modifies starting values by the difficulty modifiers
        budget *= playerIncome;
        energy *= energyProduction;

        //Modifies building costs by difficulty modifier
        for (int i = 0; i < budgetCosts.Length; i++)
        {
            budgetCosts[i] *= playerCosts;
            energyCosts[i] *= energyConsumption;
        }

        //Calls common initialization events
        Initialize();
    }

    public void Initialize(int simplifiedSeed, float enemyQuantity, float enemyStrength, float playerStrength, float playerHealth, float playerCosts, float playerIncome, float energyProduction, float energyConsumption)
    {
        //Sets the passed in data
        this.simplifiedSeed = simplifiedSeed;
        this.enemyQuantity = enemyQuantity;
        this.enemyStrength = enemyStrength;
        this.playerStrength = playerStrength;
        this.playerHealth = playerHealth;
        this.playerIncome = playerIncome;
        this.playerCosts = playerCosts;
        this.energyConsumption = energyConsumption;
        this.energyProduction = energyProduction;

        //Further events are identical between advanced and basic, so pass to another function
        midInit();
    }

    public void InitializeFromLoad(GameData data)
    {
        //Load seed data
        simplifiedSeed = data.simplifiedSeed;
        tileManager.seedA = data.seedA;
        tileManager.seedB = data.seedB;
        tileManager.seedC = data.seedC;
        tileManager.seedD = data.seedD;
        tileManager.seedE = data.seedE;
        tileManager.seedF = data.seedF;

        //Load difficulty settings
        enemyQuantity = data.enemyQuantity;
        enemyStrength = data.enemyStrength;
        playerStrength = data.playerStrength;
        playerHealth = data.playerHealth;
        playerCosts = data.playerCosts;
        playerIncome = data.playerIncome;
        energyProduction = data.energyProduction;
        energyConsumption = data.energyConsumption;

        //Default values
        energy = 10;
        maxEnemiesThisWave = 1;
        usedEnergy = 0;
        income = 0;
        energyDeficit = 0;
        mostRecentEnergyDecrease = null;
        playerBuildings.Clear();
        paused = false;
        selectedUpgrade = 0;
        selectedConstructionIndex = -1;
        selectedBuilding = null;
        selectedConstruction = null;

        //Load economic data
        budget = data.budget;
        budgetCosts = data.budgetCosts;
        energyCosts = data.energyCosts;

        //Initiate RNG and move it to the appropriate RNG stream position
        BasicUtils.WrappedInitState(simplifiedSeed);
        BasicUtils.SpamRNGUntil(data.generatedNumbers);

        //Main initilization phase
        Initialize();

        //Load other data
        wave = data.wave;
        tileManager.subbedTiles = new List<Vector2Int>(data.swappedTiles);
        
        //Ensure that all tiles have been generated properly
        for(int i = 0; i < wave; i++)
        {
            tileManager.Next();
        }

        //Ensure that all subbed tiles have been properly placed
        foreach(Vector2Int tile in tileManager.subbedTiles)
        {
            tileManager.Generate(tile);
        }

        //Go through all of the loaded buildings in order to place them on the map
        foreach (BuildingData buildingData in data.buildings)
        {
            //Create the gameObject and get the building from it
            GameObject go = Instantiate(buildings[buildingData.type + buildingData.maxAlignments], tileManager.TraversableTilemap.CellToWorld(new Vector3Int(buildingData.location.x, buildingData.location.y)), Quaternion.identity);
            PlayerBuilding pb = go.GetComponentInChildren<PlayerBuilding>();

            //Add to the very end of enabling queue
            PlayerBuilding holder = mostRecentEnergyDecrease;
            while (holder.nextChanged != null)
            {
                holder = holder.nextChanged;
            }
            holder.nextChanged = pb;
            pb.previousChanged = holder;

            //Load saved data
            pb.LoadData(buildingData);

            //Add to the tracked building dictionary
            playerBuildings.Add(buildingData.location, go);
        }
    }

    //Initialize the manager since you cannot pass in most of the data until you open the main scene
    public void Initialize()
    {

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

        //Sets the buildings array since the data has now been passed in
        buildings = new GameObject[] { turretTierOne, turretTierTwo, turretTierThree, repairTierOne, repairTierTwo, wallTierOne, wallTierTwo, extractorTierOne, extractorTierTwo, extractorTierThree};
        
        //Initialize the tilemanager for the same reason that this needs to be initialized
        TileManager.Instance.Initialize();

        //Sets the player base as the first item that is affecting energy
        mostRecentEnergyDecrease = PlayerBase;
        playerBuildings.Clear();
        playerBuildings.Add(new Vector2Int(0, 0), PlayerBase.gameObject);
    }

    //Start a new wave
    public void NextWave()
    {
        //You cannot start a wave if you have not finished your current wave, might change this later
        if(betweenWaves)
        {
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

            //List where new enemies will be kept, each inside list is all of the enemies that will spawn at the same spot
            List<AsyncInstantiateOperation<GameObject>> enemySpawns = new List<AsyncInstantiateOperation<GameObject>>();

            //Creates the correct amount of tier zero enemies for the wave
            int tierZeroEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave, 1 + (0.25f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave, 1 + (0.3f * enemyQuantity)) - 8.10328298346f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierZeroEnemyCount; i++)
            {
                
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(rangedEnemy, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier one enemies for the wave
            int tierOneEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 6, 1 + (0.21f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 6, 1 + (0.25f * enemyQuantity)) - 17.7827941004f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierOneEnemyCount; i++)
            {
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierOneEnemies[BasicUtils.WrappedRandomRange(0, tierOneEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier two enemies for the wave
            int tierTwoEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 16, 1 + (0.17f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 16, 1 + (0.2f * enemyQuantity)) - 36.4112840605f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierTwoEnemyCount; i++)
            {
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierTwoEnemies[BasicUtils.WrappedRandomRange(0, tierTwoEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier three enemies for the wave
            int tierThreeEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 36, 1 + (0.13f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 36, 1 + (0.15f * enemyQuantity)) - 69.5615082681f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierThreeEnemyCount; i++)
            {
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierThreeEnemies[BasicUtils.WrappedRandomRange(0, tierThreeEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier four enemies for the wave
            int tierFourEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 76, 1 + (0.09f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 76, 1 + (0.1f * enemyQuantity)) - 123.993519004f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierFourEnemyCount; i++)
            {
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierFourEnemies[BasicUtils.WrappedRandomRange(0, tierFourEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier five enemies for the wave
            int tierFiveEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 156, 1 + (0.05f * enemyQuantity)) - Mathf.Max(Mathf.Pow(wave - 156, 1 + (0.06f * enemyQuantity)) - 216.953760189f, 0), 0) * enemyQuantity);
            for (int i = 0; i < tierFiveEnemyCount; i++)
            {
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(tierFiveEnemies[BasicUtils.WrappedRandomRange(0, tierFiveEnemies.Length)], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                enemySpawns.Add(createdEnemy);
            }
            //Creates the correct amount of tier six enemies for the wave
            int tierSixEnemyCount = (int)(2 * Mathf.Max(Mathf.Pow(wave - 316, 1 + (0.01f * enemyQuantity - 0.01f)), 0) * enemyQuantity);
            for (int i = 0; i < tierFiveEnemyCount; i++)
            {
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AsyncInstantiateOperation<GameObject> createdEnemy = InstantiateAsync(fastSwarmTankDeadlySpammyRangedEnemy, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                enemySpawns.Add(createdEnemy);
            }

            List<Enemy> createdEnemies = new List<Enemy>();
            //Creates the correct amount of aura enemies for the wave
            int auraEnemyCount = (int)(enemyQuantity * wave / 50);
            for(int i = 0; i <auraEnemyCount; i++)
            {
                int at = BasicUtils.WrappedRandomRange(0, TileManager.Instance.potentialSpawnpoints.Count);
                AuraEnemy createdEnemy = Instantiate(auraEnemy, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[at].x, TileManager.Instance.potentialSpawnpoints[at].y)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity).GetComponentInChildren<AuraEnemy>();
                createdEnemy.auraCount = auraEnemyCount;
                createdEnemies.Add(createdEnemy);
            }

            foreach(AsyncInstantiateOperation<GameObject> createdEnemy in enemySpawns)
            {
                if (!createdEnemy.isDone)
                {
                    createdEnemy.WaitForCompletion();
                }
                createdEnemies.Add(createdEnemy.Result[0].GetComponentInChildren<Enemy>());
            }

            List<Task<(bool, Dictionary<Vector2Int, NavNode>)>> pathFinder = new List<Task<(bool, Dictionary<Vector2Int, NavNode>)>>();

            //Goes through every created enemy
            while (createdEnemies.Count > 0)
            {
                foreach (Enemy enemy in createdEnemies)
                {
                    //Adds the enemy to the list of current enemies
                    currentEnemies.Add(enemy);

                    //Generates a path for the enemy
                    Vector2Int goal = (Vector2Int)TileManager.Instance.TraversableTilemap.WorldToCell(enemy.transform.position);
                    Vector3 position = enemy.transform.position;
                    pathFinder.Add(Task<(bool, Dictionary<Vector2Int, NavNode>)>.Factory.StartNew(() => Enemy.FindPath(goal, enemy.movementSpeed, enemy.damage, enemy.firerate, position)));

                }
                List<Enemy> enemyContainer = new List<Enemy>();
                for(int i = 0; i < createdEnemies.Count; i++)
                {
                    
                    if (pathFinder[i].Result.Item1)
                    {
                        EnemyCheckpoint checkpoint = createdEnemies[i].GeneratePath(pathFinder[i].Result.Item2, (Vector2Int)TileManager.Instance.TraversableTilemap.WorldToCell(createdEnemies[i].transform.position));
                        //Duplicates swarmer enemies
                        if (createdEnemies[i].swarmer)
                        {
                            Enemy duplicateEnemy = Instantiate(createdEnemies[i], TileManager.Instance.TraversableTilemap.CellToWorld(TileManager.Instance.TraversableTilemap.WorldToCell(createdEnemies[i].transform.position)) + new Vector3(BasicUtils.WrappedRandomRange(-0.2f, 0.2f), BasicUtils.WrappedRandomRange(-0.2f, 0.2f)), Quaternion.identity);
                            currentEnemies.Add(duplicateEnemy);
                            duplicateEnemy.ActivatePath(checkpoint);
                        }
                        createdEnemies[i].ActivatePath(checkpoint);
                    }
                    else
                    {
                        enemyContainer.Add(createdEnemies[i]);
                    }
                }
                createdEnemies = enemyContainer;
                Debug.Log(createdEnemies.Count);
                pathFinder.Clear();
            }
            //Max enemies in order to ensure that you can tell how many more you need to kill to do the next wave
            maxEnemiesThisWave = currentEnemies.Count;

            //Expand the map two tiles
            TileManager.Instance.Next();
            TileManager.Instance.Next();
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

        //Ensure that stored data is the same as the new type
        selectedConstructionIndex = type;

        //Ensure that upgrades get cleared
        selectedBuilding = null;
        standardUpgradeEvents();

        //Clear construction data
        standardConstructionEvents();
        //Do not try to instantiate a new wave or no selection
        if (type != -1 && type != 10 && budget >= budgetCosts[type] && buildings[type] != null && betweenWaves)
        {
            //Create a building that will track your cursor to show what it will look like when you place it
            selectedConstruction = Instantiate(buildings[type]);

            //Ensures that there is no functionality to the building tracker
            Destroy(selectedConstruction.GetComponentInChildren<PlayerBuilding>());
        }
    }

    //Clear out construction data
    private void standardConstructionEvents()
    {
        //If a building is already being previewed, destroy it
        if (selectedConstruction != null)
        {
            Destroy(selectedConstruction);
        }
        //Set the building to null in order to ensure that there are no accidental references
        selectedBuilding = null;
    }

    //Updates the upgrade windows
    private void standardUpgradeEvents()
    {
        //If no building is selected to upgrade, close the windows
        if(selectedBuilding == null)
        {
            TurretUpgradeWindow.enabled = false;
            RepairUpgradeWindow.enabled = false;
            WallUpgradeWindow.enabled = false;
            ExtractorUpgradeWindow.enabled = false;
            SellWindow.enabled = false;
        }
        else
        {
            //Open the sell window
            SellWindow.enabled = true;

            //Identifies what type of building you selected
            Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
            if (turret != null)
            {
                //Enables the turret upgrade window
                TurretUpgradeWindow.enabled = true;

                //Writes the description so you know what you have selected
                turretDescriptionText.text = turret.GetDescription();

                //Ensures that all of the upgrade panels have accurate information
                updateUpgradeCost(0, 0);
                updateUpgradeCost(0, 1);
                updateUpgradeCost(0, 2);
                updateUpgradeCost(0, 3);
                updateUpgradeCost(0, 4);
            }
            else
            {
                RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
                if (repair != null)
                {
                    //Enables the repair station upgrade window
                    RepairUpgradeWindow.enabled = true;

                    //Writes the description so that you know what you have selected
                    repairDescriptionText.text = repair.GetDescription();

                    //Ensures that all of the upgrade panels have accurate information
                    updateUpgradeCost(1, 0);
                    updateUpgradeCost(1, 1);
                    updateUpgradeCost(1, 2);
                }
                else
                {
                    Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                    if (wall != null)
                    {
                        //Enables the wall upgrade window
                        WallUpgradeWindow.enabled = true;

                        //Writes the description so that you know what you have selected
                        wallDescriptionText.text = wall.GetDescription();

                        //Ensures that all of the upgrade panels have accurate information
                        updateUpgradeCost(2, 0);
                        updateUpgradeCost(2, 1);
                    }
                    else
                    {
                        ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                        if(extractor != null)
                        {
                            //Enables the extractor upgrade window
                            ExtractorUpgradeWindow.enabled = true;

                            //Writes the description so that you know what you have selected
                            extractorDescriptionText.text = extractor.GetDescription();

                            //Ensures that all of the upgrade panels have accurate information
                            updateUpgradeCost(3, 0);
                            updateUpgradeCost(3, 1);
                            updateUpgradeCost(3, 2);
                            updateUpgradeCost(3, 3);
                        }
                        else
                        {
                            SellWindow.enabled = false;
                        }
                    }
                }
            }
        }
    }

    //Upgrade a building
    public void UpgradeBuilding(int type, int upgrade)
    {
        //Ensures that selection is up to date
        selectedUpgrade = upgrade;
        updateSelection();

        //Gets the building to upgrade
        IUpgradeable subject = selectedBuilding.GetComponentInChildren<IUpgradeable>();

        //Ensure that alignment is finished processing
        if (subject.IsAligned())
        {
            //Ensures tht you can afford the upgrade
            if (budget >= subject.GetUpgradeCost(upgrade))
            {
                //Upgrade the building with the desired upgrade
                subject.Upgrade(upgrade);

                //Update the panel to show the new data
                updateUpgradeCost(type, upgrade);
            }
        }
        //If it is not, then this is going to be where it gets aligned
        else
        {
            //Identify how many upgrades you will need to update after alignment
            int maxTypes = 0;
            switch(type)
            {
                case 0:
                    {
                        maxTypes = 5;
                        break;
                    }
                case 1:
                    {
                        maxTypes = 3;
                        break;
                    }
                case 2:
                    {
                        maxTypes = 2;
                        break;
                    }
                case 3:
                    {
                        maxTypes = 4;
                        break;
                    }
                default:
                    {
                        maxTypes = 0;
                        break;
                    }

            }
            //Align the building
            subject.Align(upgrade);

            //Update upgrades
            for(int i = 0; i < maxTypes; i++)
            {
                updateUpgradeCost(type, i);
            }
        }
    }

    //Find if a spot is a valid place to put a given building
    private bool checkPlacement(Vector2Int location)
    {
        //Use a switch depending on type of building
        switch(selectedConstructionIndex)
        {
            //Turrets
            case 0:
            case 1:
            case 2:
                {
                    return tileManager.Check(location) >= tileManager.traversableCutoff && tileManager.CheckResource(location) < tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //Repair Stations
            case 3:
            case 4:
                {
                    return tileManager.CheckResource(location) < tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //Walls
            case 5:
            case 6:
                {
                    return tileManager.Check(location) >= tileManager.traversableCutoff && tileManager.CheckResource(location) < tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //Resource Extractors
            case 7:
            case 8:
            case 9:
                {
                    return tileManager.CheckResource(location) >= tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //If none of these, it is not a valid building so you cannot place it
            default:
                return false;
        }
    }

    //Build a building
    private void placeBuilding(Vector2Int hoveredTile)
    {
        //Stores the created building
        GameObject go;
        
        //Creates the selected building
        switch(selectedConstructionIndex)
        {
            //Tier One Turret
            case 0:
                {
                    go = Instantiate(turretTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Turret
            case 1:
                {
                    go = Instantiate(turretTierTwo, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Three Turret
            case 2:
                {
                    go = Instantiate(turretTierThree, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier One Repair Station
            case 3:
                {
                    go = Instantiate(repairTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Repair Station
            case 4:
                {
                    go = Instantiate(repairTierTwo, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier One Wall
            case 5:
                {
                    go = Instantiate(wallTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Wall
            case 6:
                {
                    go = Instantiate(wallTierTwo, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier One Resource Extractor
            case 7:
                {
                    go = Instantiate(extractorTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Resource Extractor
            case 8:
                {
                    go = Instantiate(extractorTierTwo, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Three Resource Extractor
            case 9:
                {
                    go = Instantiate(extractorTierThree, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Not a valid building, so just skip the rest
            default:
                return;
        }
        //Sets the location so that the building can know where it is
        PlayerBuilding pb = go.GetComponentInChildren<PlayerBuilding>();
        pb.location = hoveredTile;

        //Add to the very end of enabling queue
        PlayerBuilding holder = mostRecentEnergyDecrease;
        while (holder.nextChanged != null)
        {
            holder = holder.nextChanged;
        }
        holder.nextChanged = pb;
        pb.previousChanged = holder;

        //Energy management
        pb.energyCost = energyCosts[pb.GetBuildingType()];
        energyCosts[pb.GetBuildingType()] += energyConsumption * 0.5f;
        energyDeficit += pb.Disable();
        ChangeEnergyUsage(pb.energyCost);

        //Add to the tracked building dictionary
        playerBuildings.Add(hoveredTile, go);

    }

    //Changes the panel to reflect accurate building data
    private void updateUpgradeCost(int buildingType, int upgradeType)
    {
        //Ensures that it is updating the correct building type's panel
        switch(buildingType)
        {
            //Turrets
            case 0:
                {
                    //Grabs the exact turret that you want to get data from
                    Turret turret = selectedBuilding.GetComponentInChildren<Turret>();

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
                    RepairStation repairStation = selectedBuilding.GetComponentInChildren<RepairStation>();

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
                    Wall wall = selectedBuilding.GetComponentInChildren<Wall>();

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
                    ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();

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
    private void updateSelection()
    {
        //Identifies type of building you are upgrading
        Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
        if (turret != null)
        {
            //Goes through all of the turret upgrade panel frames and sets them to the appropriate color
            turretSplashFrame.color = selectedUpgrade == 0 ? selectedColor : unselectedColor;
            turretRangeFrame.color = selectedUpgrade == 1 ? selectedColor : unselectedColor;
            turretDamageFrame.color = selectedUpgrade == 2 ? selectedColor : unselectedColor;
            turretFirerateFrame.color = selectedUpgrade == 3 ? selectedColor : unselectedColor;
            turretHealthFrame.color = selectedUpgrade == 4 ? selectedColor : unselectedColor;
        }
        else
        {
            RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
            if (repair != null)
            {
                //Goes through all of the repair station upgrade panel frames and sets them to the appropriate color
                repairRangeFrame.color = selectedUpgrade == 0 ? selectedColor : unselectedColor;
                repairHealingFrame.color = selectedUpgrade == 1 ? selectedColor : unselectedColor;
                repairHealthFrame.color = selectedUpgrade == 2 ? selectedColor : unselectedColor;
            }
            else
            {
                Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                if (wall != null)
                {
                    //Goes through all of the wall upgrade panel frames and sets them to the appropriate color
                    wallHealthFrame.color = selectedUpgrade == 0 ? selectedColor : unselectedColor;
                    wallHealingFrame.color = selectedUpgrade == 1 ? selectedColor : unselectedColor;
                }
                else
                {
                    ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                    if(extractor != null)
                    {
                        //Goes through all of the extractor upgrade panel frames and sets them to the appropriate color
                        extractorExtractionFrame.color = selectedUpgrade == 0 ? selectedColor : unselectedColor;
                        extractorEnergyFrame.color = selectedUpgrade == 1 ? selectedColor : unselectedColor;
                        extractorProtectionFrame.color = selectedUpgrade == 2 ? selectedColor : unselectedColor;
                        extractorHealthFrame.color = selectedUpgrade == 3 ? selectedColor : unselectedColor;
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
            //Sets the colors of the backgrounds to show you how close you are to being able to afford them
            tierOneTurretBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[0], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[0], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[0], 0, 1)));
            tierTwoTurretBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[1], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[1], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[1], 0, 1)));
            tierThreeTurretBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[2], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[2], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[2], 0, 1)));
            tierOneRepairBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[3], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[3], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[3], 0, 1)));
            tierTwoRepairBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[4], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[4], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[4], 0, 1)));
            tierOneWallBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[5], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[5], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[5], 0, 1)));
            tierTwoWallBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[6], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[6], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[6], 0, 1)));
            tierOneExtractorBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[7], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[7], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[7], 0, 1)));
            tierTwoExtractorBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[8], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[8], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[8], 0, 1)));
            tierThreeExtractorBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[9], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[9], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[9], 0, 1)));

            //Only bother updating the turret upgrade backgrounds if you have the window open
            if (TurretUpgradeWindow.enabled)
            {
                //Gets a reference to the specific turret you have selected
                Turret turret = selectedBuilding.GetComponentInChildren<Turret>();

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
                RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();

                //Sets the background color based on how close you are to being able to afford it
                repairRangeBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / repair.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / repair.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / repair.GetUpgradeCost(0), 0, 1)));
                repairHealingBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / repair.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / repair.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / repair.GetUpgradeCost(1), 0, 1)));
                repairHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / repair.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / repair.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / repair.GetUpgradeCost(2), 0, 1)));
            }
            //Only bother updating the wall upgrade backgrounds if you have the window open, also skip this check if you know that another window is open
            else if (WallUpgradeWindow.enabled)
            {
                //Gets a reference to the specific wall you have selected
                Wall wall = selectedBuilding.GetComponentInChildren<Wall>();

                //Sets the background color based on how close you are to being able to afford it
                wallHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)));
                wallHealingBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)));
            }
            //Only bother updating the extractor upgrade backgrounds if you have the window open, also skip this check if you know that another window is open
            else if (ExtractorUpgradeWindow.enabled)
            {
                //Gets a regerence to the specific extractor you have selected
                ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();

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
                if(Input.GetKeyDown(cancelKey) && selectedConstruction == null && selectedBuilding == null)
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
                    if (selectedConstruction != null && (Input.GetKeyDown(cancelKey) || budget < budgetCosts[selectedConstructionIndex]))
                    {
                        //Ensure that all construction data is cleared
                        selectedConstructionIndex = -1;
                        Destroy(selectedConstruction);
                        selectedConstruction = null;
                    }
                    //Allows you to close the upgrade window
                    if ((Input.GetKeyDown(cancelKey) || Input.GetMouseButtonDown(1)) && selectedBuilding != null)
                    {
                        //Clear all of the selected building data
                        selectedBuilding = null;
                        standardUpgradeEvents();
                    }
                    //Allows you to start the next wave with hotkeys
                    if (Input.GetKeyDown(nextWaveKey) || (selectedConstructionIndex == 10 && Input.GetKeyDown(confirmKey)))
                    {
                        NextWave();
                        Debug.Log("Finished starting wave");
                    }
                    //Move the selected construction one to the right
                    if (Input.GetKeyDown(moveSelectionRightKey))
                    {
                        //Increase selected index
                        selectedConstructionIndex++;

                        //New wave is not construction, but on the bar so it has an index
                        if (selectedConstructionIndex == 10)
                        {
                            //Next Wave stuff here later
                        }
                        //Loop back around to no selection
                        else if (selectedConstructionIndex == 11)
                        {
                            selectedConstructionIndex = -1;
                        }
                        //Update selected construction data
                        Build(selectedConstructionIndex);
                    }
                    //Move the selected construction one to the left
                    if (Input.GetKeyDown(moveSelectionLeftKey))
                    {
                        //Decrease selected index
                        selectedConstructionIndex--;

                        //Loop back around to new wave
                        if (selectedConstructionIndex == -2)
                        {
                            //Next Wave stuff here later
                            selectedConstructionIndex = 10;
                        }
                        //Update selected construction data
                        Build(selectedConstructionIndex);
                    }
                    //Stores the tile that your mouse is above
                    Vector2Int hoveredTile = (Vector2Int)tileManager.TraversableTilemap.WorldToCell(Camera.ScreenToWorldPoint(Input.mousePosition));

                    //Actions for when you have a selected building
                    if (selectedBuilding != null)
                    {
                        //Moves selected upgrade down when you use your down navigation key
                        if (Input.GetKeyDown(moveSelectionDownKey))
                        {
                            //Sets the number of different upgrades based on the type of building
                            int cap = 0;

                            //Turret has 5 upgrades, subtract one due to zero based index
                            Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
                            if (turret != null)
                            {
                                cap = 5;
                            }
                            else
                            {
                                //Repair Station has 3 upgrade, subtract one due to zero based index
                                RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
                                if (repair != null)
                                {
                                    cap = 3;
                                }
                                else
                                {
                                    //Wall has 2 upgrades, subtract one due to zreo based index
                                    Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                                    if (wall != null)
                                    {
                                        cap = 2;
                                    }
                                    else
                                    {
                                        //Resource Extractor has 4 upgrades, subtract one due to zero based index
                                        ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                                        if (extractor != null)
                                        {
                                            cap = 4;
                                        }
                                    }
                                }
                            }
                            //Increase the index of the selected upgrade
                            selectedUpgrade++;

                            //Wrap around based on the cap
                            if (selectedUpgrade == cap)
                            {
                                selectedUpgrade = 0;
                            }
                            //Update the shown data to reflect new selection
                            updateSelection();
                        }
                        //Moves selected upgrade up when you use your up navigation key
                        if (Input.GetKeyDown(moveSelectionUpKey))
                        {
                            //Sets the number of different upgrades based on the type of building
                            int cap = 0;

                            //Turret has 5 upgrades, subtract one due to zero based index
                            Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
                            if (turret != null)
                            {
                                cap = 4;
                            }
                            else
                            {
                                //Repair Station has 3 upgrade, subtract one due to zero based index
                                RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
                                if (repair != null)
                                {
                                    cap = 2;
                                }
                                else
                                {
                                    //Wall has 2 upgrades, subtract one due to zreo based index
                                    Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                                    if (wall != null)
                                    {
                                        cap = 1;
                                    }
                                    else
                                    {
                                        //Resource Extractor has 4 upgrades, subtract one due to zero based index
                                        ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                                        if (extractor != null)
                                        {
                                            cap = 3;
                                        }
                                    }
                                }
                            }
                            //Decrease the index of the selected upgrade
                            selectedUpgrade--;

                            //Wrap around based on the cap
                            if (selectedUpgrade == -1)
                            {
                                selectedUpgrade = cap;
                            }
                            //Update the shown data to reflect new data
                            updateSelection();
                        }
                        //Allows you to upgrade using hotkeys
                        if (Input.GetKeyDown(confirmKey))
                        {
                            //Identify building type that you are trying to upgrade
                            Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
                            if (turret != null)
                            {
                                //Upgrade turret with selected upgrade
                                UpgradeBuilding(0, selectedUpgrade);
                            }
                            else
                            {
                                RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
                                if (repair != null)
                                {
                                    //upgrade repair station with selected upgrade
                                    UpgradeBuilding(1, selectedUpgrade);
                                }
                                else
                                {
                                    Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                                    if (wall != null)
                                    {
                                        //Upgrade wall with selected upgrade
                                        UpgradeBuilding(2, selectedUpgrade);
                                    }
                                    else
                                    {
                                        ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                                        if (extractor != null)
                                        {
                                            //Upgrade Extractor with selected upgrade
                                            UpgradeBuilding(3, selectedUpgrade);
                                        }
                                    }
                                }
                            }
                        }
                        //Allows you to sell buildings with hotkeys
                        if (Input.GetKeyDown(sellKey))
                        {
                            Sell();
                        }
                    }
                    //Do things if you either have left clicked or used the confirm button and you are not above the construction panel
                    if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(confirmKey)) && Input.mousePosition.y > 200 && betweenWaves)
                    {
                        //Checks the place you are over to see if there is a building there, also ensures that you are not changing selections when upgrading with mouse
                        if (playerBuildings.TryGetValue(hoveredTile, out GameObject building))
                        {
                            //Clears building data
                            Build(-1);

                            //Updates upgrade data
                            selectedUpgrade = 0;
                            selectedBuilding = building;
                            standardUpgradeEvents();
                            updateSelection();
                        }
                        //If you can place a building, do so
                        else if (checkPlacement(hoveredTile))
                        {
                            placeBuilding(hoveredTile);
                            budget -= budgetCosts[selectedConstructionIndex];

                            //Increases price of new building of that tier in order to encourage using a variety of tiers and upgrading things
                            budgetCosts[selectedConstructionIndex] *= 1.2f;
                        }
                    }
                    //Allows you to cancel constructions using right click
                    if (Input.GetMouseButtonDown(1))
                    {
                        //Clears all construction data
                        selectedConstructionIndex = -1;
                        Build(-1);
                    }
                    //Makes the building preview follow the mouse if it exists
                    if (selectedConstruction != null)
                    {
                        //Snaps the position to be centered on the tilemap's hexagonal grid
                        selectedConstruction.transform.position = tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y));
                    }
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
                budget += income * Time.deltaTime;
            }
        }
	}

    //Function to increase income in order to improve performance of constant checks
    public void IncreaseIncome(float increase)
    {
        //Increase/decrease income
        income += increase;
        //Update income text to reflect the updated income
        incomeText.text = $"Income: {income:F2} / second";
    }

    //Function to remove an enemy in order to improve performance of constant checks
    public void KillEnemy(Enemy enemy)
    {
        //Removes from stored enemies
        currentEnemies.Remove(enemy);

        //Checks to see if the wave is over
        betweenWaves = currentEnemies.Count == 0;


        //Sets the wave background to show what the remaining enemy status is
        nextWaveBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)));
    }

    //Function to remove a building
    public void Sell()
    {
        //Do not attempt to remove a building if you do not have a building selected
        if(selectedBuilding != null)
        {
            //Checks to see if you can remove the building
            PlayerBuilding building = selectedBuilding.GetComponentInChildren<PlayerBuilding>();
            if (building)
            {
                if (building.Sell())
                {
                    //Clears building selection
                    selectedBuilding = null;
                    standardUpgradeEvents();
                }
            }
        }
    }

    //Incrase or decrease max energy
    public void ChangeEnergyCap(float difference)
    {
        energy += difference;
        updateEnergy();
    }

    //Increase or decrease energy usage
    public void ChangeEnergyUsage(float difference)
    {
        usedEnergy += difference;
        updateEnergy();
    }

    //Update all of the energy details
    private void updateEnergy()
    {
        //Update UI
        energyText.text = $"Energy Usage: {usedEnergy:F2} / {energy:F2}";

        //If you are now in an energy deficit not already accounted for
        if(energy - usedEnergy < energyDeficit)
        {
            //Disables one building if possible and runs through checks again
            if (mostRecentEnergyDecrease != null)
            {
                energyDeficit += mostRecentEnergyDecrease.Disable();
                mostRecentEnergyDecrease = mostRecentEnergyDecrease.previousChanged;
                updateEnergy();
            }
        }
        //Otherwise if you have enough energy to reenable a building
        else if(mostRecentEnergyDecrease.nextChanged != null && energy - (usedEnergy + mostRecentEnergyDecrease.nextChanged.energyCost) >= energyDeficit /*- (mostRecentEnergyDecrease.nextChanged.gameObject.TryGetComponent(out ResourceExtractor ext) ? ext.energyRate : 0)*/)
        {
            //Reenable a building and run through checks again
            energyDeficit += mostRecentEnergyDecrease.nextChanged.Enable();
            mostRecentEnergyDecrease = mostRecentEnergyDecrease.nextChanged;
            updateEnergy();
        }
        //Should only run on the very first building as it checks to see if there is no previous or next, which should only happen then
        //Enables the building if it runs
        else if(mostRecentEnergyDecrease.nextChanged == null && mostRecentEnergyDecrease.previousChanged == null && energy - usedEnergy >= mostRecentEnergyDecrease.energyCost && energyDeficit < 0)
        {
            energyDeficit += mostRecentEnergyDecrease.Enable();
            updateEnergy();
        }
    }
    //Events for building removal
    public void RemoveBuilding(PlayerBuilding building)
    {
        //Ensures that the previous changed building does not lose its next link
        if (building.previousChanged != null)
        {
            building.previousChanged.nextChanged = building.nextChanged;
        }
        //Ensures that the next changed building does not lose its previous link
        if (building.nextChanged != null)
        {
            building.nextChanged.previousChanged = building.previousChanged;
        }
        //Updates energy data
        if (building.active)
        {
            //If it is an active building remove the energy cost from it
            ChangeEnergyUsage(-building.energyCost);

            //Ensure that all economy data from extractors is cleared
            if(building.GetBuildingType() >= 7)
            {
                ResourceExtractor extractor = building.gameObject.GetComponentInChildren<ResourceExtractor>();
                IncreaseIncome(-extractor.extractionRate);
                ChangeEnergyCap(-extractor.energyRate);
            }
        }
        //Remove from building tracker
        playerBuildings.Remove(building.location);
        int type = building.GetBuildingType();

        //Reduce construction energy cost due to the building being destroyed
        energyCosts[type] -= energyConsumption * 0.5f;

        //Reduce energy costs of connected buildings after it is gone to ensure that they take the appropriate amount
        while (building.nextChanged != null)
        {
            building = building.nextChanged;
            if (type == building.GetBuildingType())
            {
                building.energyCost -= energyConsumption * 0.5f;
                ChangeEnergyUsage(-energyConsumption * 0.5f);
            }
        }
    }

    //Gets the data so that it can be saved
    public GameData GetSaveData()
    {
        GameData data = new GameData();

        //Seed data
        data.simplifiedSeed = simplifiedSeed;
        data.seedA = tileManager.seedA;
        data.seedB = tileManager.seedB;
        data.seedC = tileManager.seedC;
        data.seedD = tileManager.seedD;
        data.seedE = tileManager.seedE;
        data.seedF = tileManager.seedF;

        //Economy info
        data.budgetCosts = budgetCosts;
        data.budget = budget;
        data.energyCosts = energyCosts;

        //Other
        data.wave = wave;
        data.generatedNumbers = BasicUtils.generatedNumbers;
        data.swappedTiles = tileManager.subbedTiles.ToArray();

        //Difficulty
        data.enemyQuantity = enemyQuantity;
        data.enemyStrength = enemyStrength;
        data.playerStrength = playerStrength;
        data.playerHealth = playerHealth;
        data.playerIncome = playerIncome;
        data.playerCosts = playerCosts;
        data.energyProduction = energyProduction;
        data.energyConsumption = energyConsumption;

        //Buildings
        data.buildings = new BuildingData[playerBuildings.Count - 1];
        GameObject[] buildingArray = playerBuildings.Values.ToArray();
        for(int i = 0; i < data.buildings.Length; i++)
        {
            data.buildings[i] = buildingArray[i + 1].GetComponentInChildren<PlayerBuilding>().GetAsData();
        }
        return data;
    }

    //Deactivate the manager in order to make a new map
    public void Deactivate()
    {
        active = false;
        TileManager.Instance.Deactivate();
    }
}