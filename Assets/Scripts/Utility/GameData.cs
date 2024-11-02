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
    public float enemyDifficulty, playerPower, playerIncome, playerCosts;
    public BuildingData[] buildings;
}
