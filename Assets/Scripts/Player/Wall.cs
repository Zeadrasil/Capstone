using System.Collections.Generic;
using UnityEngine;

//Walls literally just stand in the way of enemies, and they are very good at that, especially with how easy they are to repair
public class Wall : PlayerBuilding, IUpgradeable
{
    //Damager list to avoid null references and improve reaction speed
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Basic stats
    private float health;
    [SerializeField] float baseHealth;
    [SerializeField] float healingEffectiveness = 1;

    //Upgrade data
    [SerializeField] float[] expenseModifiers = new float[] { 0.5f, 0.5f };
    [SerializeField] float[] upgradeEffects = new float[] { 1.2f, 1.2f };
    [SerializeField] int[] upgradeLevels = new int[] { 0, 0 };
    [SerializeField] int baseUpgradeCost = 10;

    //Alignment data
    public int maxAlignments = 0;
    private bool finishedAligning = false;

    //Sprite data
    //Health
    [SerializeField] SpriteRenderer healthArmorA;
    [SerializeField] SpriteRenderer healthArmorB;
    [SerializeField] SpriteRenderer healthArmorC;
    [SerializeField] SpriteRenderer healthArmorD;
    [SerializeField] SpriteRenderer healthArmorE;
    [SerializeField] SpriteRenderer healthArmorF;

    //Healing
    [SerializeField] SpriteRenderer healingCenter;

    [SerializeField] AudioSource upgradeSource;

    //Add damager to damager list
    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    //Get description and location of the building
    public override string GetDescription()
    {
        return $"{basicDescription}\n({location.x}, {location.y})";
    }

    //Send health to caller for use
    public override float GetHealth()
    {
        return health;
    }

    //Get the cost of a specific stat upgrade
    public float GetUpgradeCost(int type)
    {
        return baseUpgradeCost * Mathf.Pow(1 + expenseModifiers[type] * GameManager.Instance.playerCosts, upgradeLevels[type]) * (1 + expenseModifiers[type]) * GameManager.Instance.playerCosts;
    }

    //Get the potential effects of upgrading a specific stat
    public string GetUpgradeEffects(int type)
    {
        //Switch based on desired upgrade type
        switch (type)
        {
            //Health
            case 0:
                {
                    return finishedAligning ? $"{baseHealth:F2} > {health * upgradeEffects[type] * GameManager.Instance.playerHealth:F2}" : "Select as Alignment";
                }
            //Healing effectiveness
            case 1:
                {
                    return finishedAligning ? $"{healingEffectiveness:F2} > {healingEffectiveness * upgradeEffects[type] * GameManager.Instance.playerStrength:F2}" : "Select as Alignment";
                }
            //Default
            default:
                {
                    return "";
                }
        }
    }

    //Heal self, modified by healing effectiveness and capped by base health
    public override void Heal(float healing)
    {
        health = Mathf.Min(health + healing * healingEffectiveness, baseHealth);

        //Update health bar
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.65f, 0);

        //Update data for enemies
        BuildingManager.Instance.playerHealths[location] = health;
    }

    //Remove damager from damager list
    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    //Sell building
    public override bool Sell()
    {
        //Refund part of build cost
        GameManager.Instance.budget += cost * 0.5f * health / baseHealth;

        //Call remove events
        Remove();

        //Ensures that it is known that building was successfully sold
        return true;
    }

    //Take damage to self and kill if out of health
    public override float TakeDamage(float damage)
    {
        health -= damage;

        //Update healthbar
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.65f, 0);
        
        //Update data for enemies
        BuildingManager.Instance.playerHealths[location] = health;
        
        //If out of health
        if (health <= 0)
        {
            Remove();
        }
        //Return health for utility
        return health;
    }

    //Upgrade specific stat
    public void Upgrade(int type)
    {
        //Take upgrade cost out of budget
        GameManager.Instance.budget -= GetUpgradeCost(type);

        upgradeSource.PlayOneShot(upgradeSource.clip);

        cost += GetUpgradeCost(type);

        //Mark stat as having been upgraded
        upgradeLevels[type]++;

        //Switch based on desired stat upgrade
        switch (type)
        {
            //Health
            case 0:
                {
                    //Update stats
                    baseHealth *= upgradeEffects[0] * GameManager.Instance.playerHealth;
                    health *= upgradeEffects[0] * GameManager.Instance.playerHealth;

                    //Update data for enemies
                    BuildingManager.Instance.playerHealths[location] = health;
                    break;
                }
            //Healing effectiveness
            case 1:
                {
                    //Update stats
                    healingEffectiveness *= upgradeEffects[1] * GameManager.Instance.playerStrength;
                    break;
                }
        }
    }

    //Start is called before the first frame update
    private void Start()
    {
        //Apply default sprite data
        healthArmorA.enabled = false;
        healthArmorB.enabled = false;
        healthArmorC.enabled = false;
        healthArmorD.enabled = false;
        healthArmorE.enabled = false;
        healthArmorF.enabled = false;
        healingCenter.enabled = false;
        if (needsDifficultyModifiers)
        {
            //Apply difficulty modifiers
            baseHealth *= GameManager.Instance.playerHealth;
            health = baseHealth;
            healingEffectiveness *= GameManager.Instance.playerStrength;

            //Alignment handling
            finishedAligning = maxAlignments == 0;
        }
        //This means it was loaded, so apply sprite data if relevant
        else
        {
            //If aligned with health, update sprites to match
            if (expenseModifiers[0] == 0.3f)
            {
                healthArmorA.enabled = true;
                healthArmorB.enabled = true;
                healthArmorC.enabled = true;
                healthArmorD.enabled = true;
                healthArmorE.enabled = true;
                healthArmorF.enabled = true;
            }
            //If aligned with healing effectiveness, update sprites to match
            if(expenseModifiers[1] == 0.3f)
            {
                healingCenter.enabled = true;
            }
        }
        //Update data for enemies
        BuildingManager.Instance.playerHealths.Add(location, health);

        //Update upgrade sfx volume
        upgradeSource.volume = (MusicManager.Instance.masterVolume / 100) * (MusicManager.Instance.sfxVolume / 100);
    }

    //Get energy required to upgrade stat
    public float GetUpgradeEnergy(int type)
    {
        return 0;
    }

    //Walls cannot be disabled
    public override float Disable()
    {
        return 0;
    }

    //Walls cannot be enabled
    public override float Enable()
    {
        return 0;
    }

    //Events that are always done when the building is removed
    protected override void Remove()
    {
        //Remove building
        BuildingManager.Instance.RemoveBuilding(this);

        //Update data for enemies
        BuildingManager.Instance.playerHealths.Remove(location);

        //Tell all damagers to find something else to attack
        foreach (IDamager damager in currentDamagers)
        {
            damager.cancelAttack();
        }
        //Destroy self
        Destroy(transform.parent.gameObject);
    }

    //Handles alignment
    public void Align(int type)
    {
        //Only has three different alignment options, which is none, health, or healing, so only needs to happen once
        finishedAligning = true;

        //Set the chosen alignment as primary
        expenseModifiers[type] = 0.3f;

        //If aligning to health, update sprites to match
        if(type == 0)
        {
            healthArmorA.enabled = true;
            healthArmorB.enabled = true;
            healthArmorC.enabled = true;
            healthArmorD.enabled = true;
            healthArmorE.enabled = true;
            healthArmorF.enabled = true;
        }
        //If aligning to healing effectiveness, update sprites to match
        else
        {
            healingCenter.enabled = true;
        }

        //Automatically pick the other as primary misalignment
        expenseModifiers[(type + 1) % expenseModifiers.Length] = 1f;
    }

    //Call to get whether this has finished the alignment process
    public bool IsAligned()
    {
        return finishedAligning;
    }

    //Creates a BuildingData object with the identifying information of this wall
    public override BuildingData GetAsData()
    {
        BuildingData data = new BuildingData();

        //Sets building type
        data.type = 5;

        //Sets wall data
        data.healingEffectiveness = healingEffectiveness;

        //Sets generic data
        data.health = health;
        data.baseHealth = baseHealth;
        data.cost = cost;
        data.location = location;

        //Sets upgrade data
        data.expenseModifiers = expenseModifiers;
        data.upgradeLevels = upgradeLevels;

        //Sets alignment data
        data.maxAlignments = maxAlignments;
        data.finishedAligning = finishedAligning;

        return data;
    }

    //Loads data from a BuildingData object into the wall
    public override void LoadData(BuildingData data)
    {
        //Wall data
        healingEffectiveness = data.healingEffectiveness;

        //Generic data
        health = data.health;
        baseHealth = data.baseHealth;
        cost = data.cost;
        location = data.location;
        needsDifficultyModifiers = false;

        //Upgrade data
        expenseModifiers = data.expenseModifiers;
        upgradeLevels = data.upgradeLevels;

        //Alignment data
        maxAlignments = data.maxAlignments;
        finishedAligning = data.finishedAligning;

        //Energy management
        GameManager.Instance.ChangeEnergyUsage(energyCost);
    }

    //Return type for use in identify what to do with this
    public override int GetBuildingType()
    {
        return 5 + maxAlignments;
    }

    public override int GetConstructionType()
    {
        return 2;
    }
}
