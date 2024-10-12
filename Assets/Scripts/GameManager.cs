using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public PlayerBase PlayerBase;
    public GameObject EnemyCheckpointPrefab;
    public Camera Camera;

    
    [SerializeField] GameObject turretTierOne;
    [SerializeField] GameObject extractorTierOne;
    [SerializeField] GameObject baseEnemy;

    [SerializeField] Image nextWaveBackground;
    [SerializeField] Image tierOneTurretBackground;
    [SerializeField] Image tierOneTurretFrame;
    [SerializeField] Image tierOneExtractorBackground;
    [SerializeField] Image tierOneExtractorFrame;

    [SerializeField] TMP_Text budgetText;
    [SerializeField] TMP_Text incomeText;

    [SerializeField] Color unselectedColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Vector3 unavailableColor;
    [SerializeField] Vector3 availableColor;

    private GameObject selectedBuilding;

    private int selectedBuildingIndex;

    [SerializeField] float[] budgetCosts = new float[] {-1, 10, -1, -1, -1, -1, -1, -1, 10, -1 };

    public Dictionary<Vector2Int, GameObject> playerBuildings = new Dictionary<Vector2Int, GameObject>();
    public float budget = 100;
    public float income = 0;
    private Vector3 mouseOriginalPosition;
    private Vector3 cameraOriginalPosition;
    int wave = 0;
    public List<Enemy> currentEnemies = new List<Enemy>();
    public List<GameObject> checkpoints = new List<GameObject>();
    public bool betweenWaves = true;
    private TileManager tileManager;
    int maxEnemiesThisWave = 1;
    void Start()
    {
        tileManager = TileManager.Instance;
    }

    public void clickTest()
    {
        Debug.Log("Click Works");
    }

    public void NextWave()
    {
        if(betweenWaves)
        {
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
            for (int i = 0; i < 5 * Mathf.Pow(wave, 1.25f); i++)
            {
                enemySpawns[i % enemySpawns.Count].Add(Instantiate(baseEnemy, TileManager.Instance.Tilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[i % enemySpawns.Count].x, TileManager.Instance.potentialSpawnpoints[i % enemySpawns.Count].y)) + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f)), Quaternion.identity).GetComponent<Enemy>());
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
            TileManager.Instance.Next();
            TileManager.Instance.Next();
            }
        }
    }

    public void TierOneTurret()
    {
        if(budget >= budgetCosts[1] && betweenWaves)
        {
            if (selectedBuilding != null)
            {
                Destroy(selectedBuilding);
            }
            selectedBuildingIndex = 1;
            selectedBuilding = Instantiate(turretTierOne);
            Destroy(selectedBuilding.GetComponent<Turret>());
        }
    }

    public void TierOneExtractor()
    {
        if (budget >= budgetCosts[8] && betweenWaves)
        {
            if(selectedBuilding != null)
            {
                Destroy(selectedBuilding);
            }
            selectedBuildingIndex = 8;
            selectedBuilding = Instantiate(extractorTierOne);
            Destroy(selectedBuilding.GetComponent<ResourceExtractor>());
        }
    }

    private bool checkPlacement(Vector2Int location)
    {
        switch(selectedBuildingIndex)
        {
            case 1:
                return tileManager.Check(location) >= tileManager.traversableCutoff && tileManager.CheckResource(location) < tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
            case 8:
                return tileManager.CheckResource(location) >= tileManager.resourceCutoff && !playerBuildings.ContainsKey(location);
            default:
                return false;
        }
    }

    private void placeBuilding(Vector2Int hoveredTile)
    {
        switch(selectedBuildingIndex)
        {
            case 1:
                {
                    GameObject turret = Instantiate(turretTierOne, tileManager.Tilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    turret.GetComponent<Turret>().location = hoveredTile;
                    playerBuildings.Add(hoveredTile, turret);
                    return;
                }
            case 8:
                {
                    GameObject extractor = Instantiate(extractorTierOne, tileManager.Tilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    extractor.GetComponent<ResourceExtractor>().location = hoveredTile;
                    playerBuildings.Add(hoveredTile, extractor);
                    return;
                }
            default:
                return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        tierOneTurretFrame.color = selectedBuildingIndex == 1 ? selectedColor : unselectedColor;
        tierOneExtractorFrame.color = selectedBuildingIndex == 8 ? selectedColor : unselectedColor;

        tierOneTurretBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[1], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[1], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[1], 0, 1)));
        tierOneExtractorBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, Mathf.Clamp(budget / budgetCosts[8], 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, Mathf.Clamp(budget / budgetCosts[8], 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, Mathf.Clamp(budget / budgetCosts[8], 0, 1)));

        budgetText.text = $"Budget: {budget:F2}";
        incomeText.text = $"Income: {income:F2} / second";
        nextWaveBackground.color = new Color(Mathf.Lerp(unavailableColor.x, availableColor.x, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.y, availableColor.y, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)), Mathf.Lerp(unavailableColor.z, availableColor.z, 1 - Mathf.Clamp(currentEnemies.Count / (float)maxEnemiesThisWave, 0, 1)));

        if (Input.GetKeyDown(KeyCode.Escape) || (selectedBuildingIndex != -1 && budget < budgetCosts[selectedBuildingIndex]))
        {
            selectedBuildingIndex = -1;
            Destroy(selectedBuilding);
            selectedBuilding = null;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextWave();
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 25 * Time.deltaTime * Camera.orthographicSize;
        }
        Vector3 cameraModifier = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            cameraModifier.y += Camera.orthographicSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
        {
            cameraModifier.x -= Camera.orthographicSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            cameraModifier.y -= Camera.orthographicSize * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
        {
            cameraModifier.x += Camera.orthographicSize * Time.deltaTime;
        }
        Camera.transform.position += cameraModifier;

        Vector2Int hoveredTile = (Vector2Int)tileManager.Tilemap.WorldToCell(Camera.ScreenToWorldPoint(Input.mousePosition));
        if(Input.GetMouseButtonDown(0) && checkPlacement(hoveredTile) && Input.mousePosition.y > 200)
        {
            placeBuilding(hoveredTile);
            budget -= budgetCosts[selectedBuildingIndex];
        }
        if(selectedBuilding != null)
        {
            selectedBuilding.transform.position = tileManager.Tilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y));
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            TierOneTurret();
        }
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            TierOneExtractor();
        }
        if (!betweenWaves)
        {
            budget += income * Time.deltaTime;
        }
	}

    private void FixedUpdate()
    {
        
    }
}