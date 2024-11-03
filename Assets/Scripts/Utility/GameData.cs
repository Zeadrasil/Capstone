using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds game data for saving and loading
[System.Serializable]
public class GameData
{
    //Seed data
    public uint seedA, seedB, seedC, seedD, seedE, seedF;
    public int simplifiedSeed;

    //Economy info
    public float[] budgetCosts;
    public float budget;

    //Wave info
    public int wave;

    //Difficulty settings
    public float enemyQuantity = 1, enemyStrength = 1, playerStrength = 1, playerHealth = 1, playerIncome = 1, playerCosts = 1, energyProduction = 1, energyConsumption = 1;
    public BuildingData[] buildings;

    //Other info
    public uint generatedNumbers;
    public Vector2Int[] swappedTiles;
}
