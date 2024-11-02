using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    //Type of building
    public int type;

    //Turret specific data
    public float firerate, damage, splashRange;
    public bool splash;

    //Repair station specific data
    public int cooldown, baseCooldown;
    public float healing;

    //Wall specific data
    public float healingEffectiveness;

    //Resource extractor specific data
    public float extractionRate, energyRate, damageEffectiveness;

    //Data used by multiple building types
    public float range, health, baseHealth, energyCost, cost;
    public Vector2Int location;

    //Upgrade data
    public float[] expenseModifiers;
    public int[] upgradeLevels;

    //Alignment data
    public int maxAlignments, alignments;
    public bool primaryMisalignmentChosen, finishedAligning;
}
