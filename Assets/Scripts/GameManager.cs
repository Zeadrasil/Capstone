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
    [SerializeField] GameObject baseEnemy;
    public Dictionary<Vector2Int, GameObject> playerBuildings = new Dictionary<Vector2Int, GameObject>();
    public float budget = 100;
    private Vector3 mouseOriginalPosition;
    private Vector3 cameraOriginalPosition;
    int wave = 0;
    public List<Enemy> currentEnemies = new List<Enemy>();
    public List<GameObject> checkpoints = new List<GameObject>();
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
        //if(Input.GetMouseButtonDown(1))
        //{
        //    mouseOriginalPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        //    cameraOriginalPosition = Camera.transform.position;
        //}
        //if(Input.GetMouseButton(1))
        //{
        //    Camera.transform.position = cameraOriginalPosition + (mouseOriginalPosition - Camera.ScreenToWorldPoint(Input.mousePosition));
        //}

        Vector2Int hoveredTile = (Vector2Int)TileManager.Instance.Tilemap.WorldToCell(Camera.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(TileManager.Instance.Adjacencies.ContainsKey(hoveredTile) && !playerBuildings.ContainsKey(hoveredTile) && budget >= 10)
            {
                GameObject turret = Instantiate(turretTierOne, TileManager.Instance.Tilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                turret.GetComponent<Turret>().location = hoveredTile;
                playerBuildings.Add(hoveredTile, turret);
                budget -= 10;
            }
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            wave++;
            EnemyCheckpoint.positions.Clear();
            if(currentEnemies.Count == 0)
            {
                foreach(GameObject checkpoint in checkpoints)
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
                    enemySpawns[i % enemySpawns.Count].Add(Instantiate(baseEnemy, TileManager.Instance.Tilemap.CellToWorld(new Vector3Int(TileManager.Instance.potentialSpawnpoints[i % enemySpawns.Count].x, TileManager.Instance.potentialSpawnpoints[i % enemySpawns.Count].y)) + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)), Quaternion.identity).GetComponent<Enemy>());
                }
                for(int i = 0; i < enemySpawns.Count; i++)
                {
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
                        for (int j = 0; j < enemies.Count; j++)
                        {
                            if (enemySpawns[j % enemySpawns.Count][0].currentGuide != null)
                            {
                                enemies[j].activatePath(enemySpawns[j % enemySpawns.Count][0].currentGuide);
                                currentEnemies.Add(enemies[j]);
                            }
                            enemies[j].transform.position = enemySpawns[j % enemySpawns.Count][0].transform.position;
                            enemySpawns[j % enemySpawns.Count].Add(enemies[j]);
                        }
                    }
                }
                TileManager.Instance.Next();
            }
        }

	}

    private void FixedUpdate()
    {
        
    }
}