using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GameManager : Singleton<GameManager>
{
    public PlayerBase PlayerBase;
    public GameObject EnemyCheckpointPrefab;
    public Camera Camera;

    private GameObject selectedConstruction;
    private int selectedConstructionIndex = -1;
    private GameObject selectedBuilding;

    public float[] budgetCosts = new float[] {10, -1, -1, 10, -1, 10, -1, 10, -1, -1 };

    public Dictionary<Vector2Int, GameObject> playerBuildings = new Dictionary<Vector2Int, GameObject>();
    public float budget = 100;
    private float income = 0;
    private Vector3 mouseOriginalPosition;
    private Vector3 cameraOriginalPosition;
    int wave = 0;
    private List<Enemy> currentEnemies = new List<Enemy>();
    public List<GameObject> checkpoints = new List<GameObject>();
    public bool betweenWaves = true;
    private TileManager tileManager;
    int maxEnemiesThisWave = 1;

    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

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

    public KeyCode nextWaveKey = KeyCode.N;
    public KeyCode cancelKey = KeyCode.Escape;
    public KeyCode moveSelectionDownKey = KeyCode.DownArrow;
    public KeyCode moveSelectionUpKey = KeyCode.UpArrow;
    public KeyCode moveSelectionRightKey = KeyCode.RightArrow;
    public KeyCode moveSelectionLeftKey = KeyCode.LeftArrow;
    public KeyCode confirmKey = KeyCode.Return;

    public TMP_Text nextWaveLabel;
    public GameObject turretTierOne;
    public GameObject repairTierOne;
    public GameObject wallTierOne;
    public GameObject extractorTierOne;
    public GameObject baseEnemy;

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

    public TMP_Text budgetText;
    public TMP_Text incomeText;

    public Color unselectedColor;
    public Color selectedColor;
    public Vector3 unavailableColor;
    public Vector3 availableColor;

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

    public int simplifiedSeed;
    public float enemyDifficulty = 1;
    public float playerPower = 1;
    public float playerIncome = 1;
    public float playerCosts = 1;

    private bool active = false;
    public int queueInitialize = -1;

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

    public Canvas WallUpgradeWindow;

    public TMP_Text wallHealthUpgradeText;
    public Image wallHealthFrame;
    public Image wallHealthBackground;

    public TMP_Text wallHealingUpgradeText;
    public Image wallHealingFrame;
    public Image wallHealingBackground;

    public TMP_Text wallDescriptionText;

    public Canvas ExtractorUpgradeWindow;

    public TMP_Text extractorEnergyModeText;
    public TMP_Text extractorRateUpgradeText;
    public TMP_Text extractorHealthUpgradeText;

    private int selectedUpgrade = 0;
    private GameObject[] buildings = null;


    void Start()
    {
        tileManager = TileManager.Instance;
    }
    public void Initialize()
    {
        Random.InitState(simplifiedSeed);
        budget *= playerIncome;
        for(int i = 0; i < budgetCosts.Length; i++)
        {
            budgetCosts[i] *= playerCosts;
        }
        active = true;
        TurretUpgradeWindow.enabled = false;
        WallUpgradeWindow.enabled = false;
        tierOneTurretLabel.text = tierOneTurretKey.ToString().Contains("Alpha") ? tierOneTurretKey.ToString().Remove(0, 5) : tierOneTurretKey.ToString();
        tierOneExtractorLabel.text = tierOneExtractorKey.ToString().Contains("Alpha") ? tierOneExtractorKey.ToString().Remove(0, 5) : tierOneExtractorKey.ToString();
        nextWaveLabel.text = nextWaveKey.ToString().Contains("Alpha") ? nextWaveKey.ToString().Remove(0, 5) : nextWaveKey.ToString();
        nextWaveBackground.color = new Color(availableColor.x, availableColor.y, availableColor.z);
        buildings = new GameObject[] { turretTierOne, null, null, repairTierOne, null, wallTierOne, null, extractorTierOne, null, null};
        TileManager.Instance.Initialize();
    }

    public void clickTest()
    {
        Debug.Log("Click Works");
    }

    public void NextWave()
    {
        if(betweenWaves)
        {
            selectedConstructionIndex = 10;
            nextWaveFrame.color = selectedColor;
            nextWaveBackground.color = new Color(unavailableColor.x, unavailableColor.y, unavailableColor.z);
            standardConstructionEvents();
            selectedBuilding = null;
            standardUpgradeEvents();
            wave++;
            betweenWaves = false;
            EnemyCheckpoint.positions.Clear();
            foreach (GameObject checkpoint in checkpoints)
            {
                Destroy(checkpoint);
            }
            checkpoints.Clear();
            List<List<Enemy>> enemySpawns = new List<List<Enemy>>();

            for (int i = 0; i < TileManager.Instance.potentialSpawnpoints.Count; i++)
            {
                enemySpawns.Add(new List<Enemy>());
            }
            for (int i = 0; i < 5 * Mathf.Pow(wave, 1 + (0.25f * enemyDifficulty)) * enemyDifficulty; i++)
            {
                enemySpawns[i % enemySpawns.Count].Add(Instantiate(baseEnemy, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[i % enemySpawns.Count].x, TileManager.Instance.potentialSpawnpoints[i % enemySpawns.Count].y)) + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f)), Quaternion.identity).GetComponent<Enemy>());
            }
            for (int i = 0; i < enemySpawns.Count; i++)
            {
                if (enemySpawns[i].Count == 0)
                {
                    enemySpawns.RemoveAt(i);
                    i--;
                    continue;
                }
                EnemyCheckpoint checkpoint = enemySpawns[i][0].GeneratePath();
                if (checkpoint != null)
                {
                    foreach (Enemy enemy in enemySpawns[i])
                    {
                        enemy.activatePath(checkpoint);
                        currentEnemies.Add(enemy);
                    }
                }
                else
                {
                    List<Enemy> enemies = enemySpawns[i];
                    enemySpawns.RemoveAt(i);
                    i--;
                    for (int j = 0, k = 0; j < enemies.Count; j++)
                    {
                        k = Mathf.Max(k, 0);
                        while (enemySpawns[(j + k) % enemySpawns.Count].Count == 0)
                        {
                            k++;
                        }
                        if (enemySpawns[(j + k) % enemySpawns.Count][0].currentGuide != null)
                        {
                            enemies[j].activatePath(enemySpawns[(j + k) % enemySpawns.Count][0].currentGuide);
                            currentEnemies.Add(enemies[j]);
                        }
                        enemies[j].transform.position = enemySpawns[(j + k) % enemySpawns.Count][0].transform.position;
                        enemySpawns[j % enemySpawns.Count].Add(enemies[j]);
                    }
                }
                maxEnemiesThisWave = currentEnemies.Count;
            }
            TileManager.Instance.Next();
            TileManager.Instance.Next();
        }
    }
    public void Build(int type)
    {
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
        selectedConstructionIndex = type;
        standardConstructionEvents();
        if (type != -1 && type != 10 && budget >= budgetCosts[type] && buildings[type] != null && betweenWaves)
        {
            selectedConstruction = Instantiate(buildings[type]);
            Destroy(selectedConstruction.GetComponent<PlayerBuilding>());
        }
    }

    private void standardConstructionEvents()
    {
        if (selectedConstruction != null)
        {
            Destroy(selectedConstruction);
        }
        selectedBuilding = null;
    }
    private void standardUpgradeEvents()
    {
        if(selectedBuilding == null)
        {
            TurretUpgradeWindow.enabled = false;
            WallUpgradeWindow.enabled = false;
        }
        else
        {
            if(selectedBuilding.TryGetComponent(out Turret turret))
            {
                TurretUpgradeWindow.enabled = true;
                turretDescriptionText.text = turret.GetDescription();
                updateUpgradeCost(0, 0);
                updateUpgradeCost(0, 1);
                updateUpgradeCost(0, 2);
                updateUpgradeCost(0, 3);
                updateUpgradeCost(0, 4);
            }
            else if(selectedBuilding.TryGetComponent(out Wall wall))
            {
                WallUpgradeWindow.enabled = true;
                wallDescriptionText.text = wall.GetDescription();
                updateUpgradeCost(2, 0);
                updateUpgradeCost(2, 1);
            }
            else if (selectedBuilding.TryGetComponent(out ResourceExtractor extractor))
            {
                
            }
        }
    }
    public void UpgradeTurret(int upgrade)
    {
        upgradeBuilding(0, upgrade);
    }
    public void UpgradeWall(int upgrade)
    {
        upgradeBuilding(2, upgrade);
    }
    private void upgradeBuilding(int type, int upgrade)
    {
        IUpgradeable subject = selectedBuilding.GetComponent<IUpgradeable>();
        if (budget >= subject.GetUpgradeCost(upgrade))
        {
            subject.Upgrade(upgrade);
            updateUpgradeCost(type, upgrade);
        }
    }

    private bool checkPlacement(Vector2Int location)
    {
        switch(selectedConstructionIndex)
        {
            case 0:
                {
                    return tileManager.Check(location) >= tileManager.traversableCutoff && tileManager.CheckResource(location) < tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            case 3:
                {
                    return tileManager.CheckResource(location) < tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            case 5:
                {
                    return tileManager.Check(location) >= tileManager.traversableCutoff && tileManager.CheckResource(location) < tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            case 7:
                {
                    return tileManager.CheckResource(location) >= tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            default:
                return false;
        }
    }

    private void placeBuilding(Vector2Int hoveredTile)
    {
        GameObject go;
        switch(selectedConstructionIndex)
        {
            case 0:
                {
                    go = Instantiate(turretTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            case 3:
                {
                    go = Instantiate(repairTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            case 5:
                {
                    go = Instantiate(wallTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            case 7:
                {
                    go = Instantiate(extractorTierOne, tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            default:
                return;
        }
        go.GetComponent<PlayerBuilding>().location = hoveredTile;
        playerBuildings.Add(hoveredTile, go);
    }

    private void updateUpgradeCost(int buildingType, int upgradeType)
    {
        switch(buildingType)
        {
            case 0:
                {
                    Turret turret = selectedBuilding.GetComponent<Turret>();
                    switch(upgradeType)
                    {
                        case 0:
                            {
                                turretSplashUpgradeText.text = turret.GetUpgradeCost(upgradeType) >= 0 ? $"Splash Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE";
                                break;
                            }
                        case 1:
                            {
                                turretRangeUpgradeText.text = turret.GetUpgradeCost(upgradeType) >= 0 ? $"Range Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE";
                                break;
                            }
                        case 2:
                            {
                                turretDamageUpgradeText.text = turret.GetUpgradeCost(upgradeType) >= 0 ? $"Damage Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE";
                                break;
                            }
                        case 3:
                            {
                                turretFirerateUpgradeText.text = turret.GetUpgradeCost(upgradeType) >= 0 ? $"Firerate Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE";
                                break;
                            }
                        case 4:
                            {
                                turretHealthUpgradeText.text = turret.GetUpgradeCost(upgradeType) >= 0 ? $"Health Upgrade:\n{turret.GetUpgradeEffects(upgradeType)}\nCost:\n{turret.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE";
                                break;
                            }

                    }
                    break;
                }
            case 2:
                {
                    Wall wall = selectedBuilding.GetComponent<Wall>();
                    if(upgradeType == 0)
                    {
                        wallHealthUpgradeText.text = wall.GetUpgradeCost(upgradeType) >= 0 ? $"Health Upgrade:\n{wall.GetUpgradeEffects(upgradeType)}\nCost:\n{wall.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE";
                    }
                    else
                    {
                        wallHealingUpgradeText.text = wall.GetUpgradeCost(upgradeType) >= 0 ? $"Healing Upgrade:\n{wall.GetUpgradeEffects(upgradeType)}\nCost:\n{wall.GetUpgradeCost(upgradeType):F2}" : "NOT\nAVAILABLE";
                    }
                    break;
                }
        }
    }
    private void updateSelection()
    {
        if (selectedBuilding.TryGetComponent(out Turret ignoreableTurret))
        {
            turretSplashFrame.color = selectedUpgrade == 0 ? selectedColor : unselectedColor;
            turretRangeFrame.color = selectedUpgrade == 1 ? selectedColor : unselectedColor;
            turretDamageFrame.color = selectedUpgrade == 2 ? selectedColor : unselectedColor;
            turretFirerateFrame.color = selectedUpgrade == 3 ? selectedColor : unselectedColor;
            turretHealthFrame.color = selectedUpgrade == 4 ? selectedColor : unselectedColor;
        }
        else if(selectedBuilding.TryGetComponent(out Wall ignoreableWall))
        {
            wallHealthFrame.color = selectedUpgrade == 0 ? selectedColor : unselectedColor;
            wallHealingFrame.color = selectedUpgrade == 1 ? selectedColor : unselectedColor;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (active)
        {

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

            if (TurretUpgradeWindow.enabled)
            {
                Turret turret = selectedBuilding.GetComponent<Turret>();
                turretSplashBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(0), 0, 1)));
                turretRangeBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(1), 0, 1)));
                turretDamageBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(2), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(2), 0, 1)));
                turretFirerateBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(3), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(3), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(3), 0, 1)));
                turretHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / turret.GetUpgradeCost(4), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / turret.GetUpgradeCost(4), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / turret.GetUpgradeCost(4), 0, 1)));
            }
            else if (WallUpgradeWindow.enabled)
            {
                Wall wall = selectedBuilding.GetComponent<Wall>();
                wallHealthBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / wall.GetUpgradeCost(0), 0, 1)));
                wallHealingBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / wall.GetUpgradeCost(1), 0, 1)));
            }

            budgetText.text = $"Budget: {budget:F2}";

            if (selectedConstruction != null && (Input.GetKeyDown(cancelKey) || budget < budgetCosts[selectedConstructionIndex]))
            {
                selectedConstructionIndex = -1;
                Destroy(selectedConstruction);
                selectedConstruction = null;
            }
            if((Input.GetKeyDown(cancelKey) || Input.GetMouseButtonDown(1))&& selectedBuilding != null)
            {
                selectedBuilding = null;
                standardUpgradeEvents();
            }
            if (Input.GetKeyDown(nextWaveKey) || (selectedConstructionIndex == 10 && Input.GetKeyDown(confirmKey)))
            {
                NextWave();
            }
            if(Input.GetKeyDown(confirmKey) && selectedBuilding != null)
            {
                if(selectedBuilding.TryGetComponent(out Turret ignoreableTurret))
                {
                    UpgradeTurret(selectedUpgrade);
                }
            }
            if(Input.GetKeyDown(moveSelectionRightKey))
            {
                if(selectedConstructionIndex == -1)
                {
                    selectedBuilding = null;
                    standardUpgradeEvents();
                }
                selectedConstructionIndex++;
                if(selectedConstructionIndex == 10)
                {
                    //Next Wave stuff here later
                }
                else if(selectedConstructionIndex == 11)
                {
                    selectedConstructionIndex = -1;
                }
                Build(selectedConstructionIndex);
            }
            if(Input.GetKeyDown(moveSelectionLeftKey))
            {
                if (selectedConstructionIndex == -1)
                {
                    selectedBuilding = null;
                    standardUpgradeEvents();
                }
                selectedConstructionIndex--;
                if (selectedConstructionIndex == -2)
                {
                    //Next Wave stuff here later
                    selectedConstructionIndex = 10;
                }
                Build(selectedConstructionIndex);
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                Camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 25 * Time.deltaTime * Camera.orthographicSize;
            }
            Vector3 cameraModifier = Vector3.zero;
            if (Input.GetKey(forwardKey))
            {
                cameraModifier.y += Camera.orthographicSize * Time.deltaTime;
            }
            if (Input.GetKey(leftKey))
            {
                cameraModifier.x -= Camera.orthographicSize * Time.deltaTime;
            }
            if (Input.GetKey(backKey))
            {
                cameraModifier.y -= Camera.orthographicSize * Time.deltaTime;
            }
            if (Input.GetKey(rightKey))
            {
                cameraModifier.x += Camera.orthographicSize * Time.deltaTime;
            }
            Camera.transform.position += cameraModifier;

            Vector2Int hoveredTile = (Vector2Int)tileManager.TraversableTilemap.WorldToCell(Camera.ScreenToWorldPoint(Input.mousePosition));
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(confirmKey)) && Input.mousePosition.y > 200 && betweenWaves)
            {
                if(playerBuildings.TryGetValue(hoveredTile, out GameObject building))
                {
                    Build(-1);
                    selectedUpgrade = 0;
                    selectedBuilding = building;
                    standardUpgradeEvents();
                    updateSelection();
                }
                else if (checkPlacement(hoveredTile))
                {
                    placeBuilding(hoveredTile);
                    budget -= budgetCosts[selectedConstructionIndex];
                }
            }
            if (Input.GetKeyDown(moveSelectionDownKey) && selectedBuilding != null)
            {
                int cap = 0;
                if (selectedBuilding.TryGetComponent(out Turret ignoreableTurret))
                {
                    cap = 5;
                }
                else if(selectedBuilding.TryGetComponent(out Wall ignoreableWall))
                {
                    cap = 2;
                }
                selectedUpgrade++;
                if(selectedUpgrade == cap)
                {
                    selectedUpgrade = 0;
                }
                updateSelection();
            }
            if (Input.GetKeyDown(moveSelectionUpKey) && selectedBuilding != null)
            {
                int cap = 0;
                if (selectedBuilding.TryGetComponent(out Turret ignoreableTurret))
                {
                    cap = 4;
                }
                else if (selectedBuilding.TryGetComponent(out Wall ignoreableWall))
                {
                    cap = 1;
                }
                selectedUpgrade--;
                if(selectedUpgrade == -1)
                {
                    selectedUpgrade = cap;
                }
                updateSelection();
            }
            if(Input.GetKeyDown(confirmKey) && selectedBuilding != null)
            {
                if(selectedBuilding.TryGetComponent(out Turret ignoreableTurret))
                {
                    upgradeBuilding(0, selectedUpgrade);
                }
                else if(selectedBuilding.TryGetComponent(out Wall ignoreableWall))
                {
                    upgradeBuilding(2, selectedUpgrade);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                selectedConstructionIndex = -1;
                standardConstructionEvents();
            }
            if (selectedConstruction != null)
            {
                selectedConstruction.transform.position = tileManager.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y));
            }
            if (Input.GetKeyDown(tierOneTurretKey))
            {
                Build(0);
            }
            if(Input.GetKeyDown(tierOneRepairKey))
            {
                Build(3);
            }
            if(Input.GetKeyDown(tierOneWallKey))
            {
                Build(5);
            }
            if (Input.GetKeyDown(tierOneExtractorKey))
            {
                Build(7);
            }
            if (!betweenWaves)
            {
                budget += income * Time.deltaTime;
            }
        }
        else if (queueInitialize > 0)
        {
            if(queueInitialize == 1)
            {
                Initialize();
            }
            queueInitialize--;
        }
	}

    public void IncreaseIncome(float increase)
    {
        income += increase;
        incomeText.text = $"Income: {income:F2} / second";
    }

    public void KillEnemy(Enemy enemy)
    {
        currentEnemies.Remove(enemy);
        betweenWaves = currentEnemies.Count == 0;
        nextWaveBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)));
    }
}