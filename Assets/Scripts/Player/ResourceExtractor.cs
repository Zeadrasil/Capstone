using System.Collections.Generic;
using UnityEngine;

//ResourceExtractors are used to generate resources for the player
public class ResourceExtractor : PlayerBuilding, IUpgradeable
{
    //Basic stats
    float health = 10;
    [SerializeField] float baseHealth = 10;
    public float extractionRate = 0.1f;
    public float energyRate = 0;
    public float damageEffectiveness = 10;

    //Upgrade data
    public int[] upgradeLevels = new int[] { 0, 0, 0, 0 };
    public float[] expenseModifiers = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
    public float[] upgradeEffects = new float[] {1.2f, 0.2f, 0.9f, 1.2f };
    [SerializeField] int baseUpgradeCost = 0;

    //Energy info
    public float[] energyCosts = new float[] { 0.1f, 0f, 0.1f, 0.1f };
    [SerializeField] SpriteRenderer spriteRenderer;
    private bool activate = false;

    //Alignment data
    public int maxAlignments = 0;
    private int alignments = 0;
    private bool primaryMisalignmentChosen = false;
    private bool finishedAligning = false;

    //Sprite data
    [SerializeField] Color activeColor = Color.white;

    //Center data
    [SerializeField] SpriteRenderer energyCenter;
    [SerializeField] SpriteRenderer extractionCenter;

    //Health
    [SerializeField] SpriteRenderer healthArmorA;
    [SerializeField] SpriteRenderer healthArmorB;
    [SerializeField] SpriteRenderer healthArmorC;
    [SerializeField] SpriteRenderer healthArmorD;
    [SerializeField] SpriteRenderer healthArmorE;
    [SerializeField] SpriteRenderer healthArmorF;

    //Protection
    [SerializeField] SpriteRenderer protectionBarA;
    [SerializeField] SpriteRenderer protectionBarB;
    [SerializeField] SpriteRenderer protectionBarC;

    //Damager list to avoid null references and improve reaction speed
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Take damage
    public override float TakeDamage(float damage)
    {
        //Remove currently given income
        if (active)
        {
            GameManager.Instance.IncreaseIncome(-extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
        }

        //Decrease health
        health -= damage;

        //Update data for enemies
        GameManager.Instance.playerHealths[location] = health;

        //Update healthbar
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);

        //If out of health
        if (health <= 0)
        {
            //Call removal events
            Remove();
        }
        else if(active)
        {
            //Otherwise update income based on reduced rate from reduced health
            GameManager.Instance.IncreaseIncome(extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
        }
        //Return health for utility
        return health;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set default sprite data
        energyCenter.enabled = false;
        extractionCenter.enabled = false;
        healthArmorA.enabled = false;
        healthArmorB.enabled = false;
        healthArmorC.enabled = false;
        healthArmorD.enabled = false;
        healthArmorE.enabled = false;
        healthArmorF.enabled = false;
        protectionBarA.enabled = false;
        protectionBarB.enabled = false;
        protectionBarC.enabled = false;

        //Skip applying difficulty modifiers if they have alread been applied due to loading
        if (needsDifficultyModifiers)
        {
            //Modifies extraction rate and health based on difficulty modifiers
            extractionRate *= GameManager.Instance.playerIncome;
            baseHealth *= GameManager.Instance.playerHealth;
            damageEffectiveness /= GameManager.Instance.playerHealth;
            health = baseHealth;

            //Applies income
            GameManager.Instance.IncreaseIncome(extractionRate);

            //Figures out alignment config
            finishedAligning = maxAlignments == alignments;
        }
        //This means it was loaded, so apply sprite data if relevant
        else
        {
            //If aligned with extraction rate, update sprites to match
            if (expenseModifiers[0] == 0.3f)
            {
                extractionCenter.enabled = true;
            }
            //If aligned with energy rate, update sprites to match
            if(expenseModifiers[1] == 0.3f)
            {
                energyCenter.enabled = true;
            }
            //If aligned with protection, update sprites to match
            if (expenseModifiers[2] == 0.3f)
            {
                protectionBarA.enabled = true;
                protectionBarB.enabled = true;
                protectionBarC.enabled = true;
            }
            //If aligned with health update sprites to match
            if (expenseModifiers[3] == 0.3f)
            {
                healthArmorA.enabled = true;
                healthArmorB.enabled = true;
                healthArmorC.enabled = true;
                healthArmorD.enabled = true;
                healthArmorE.enabled = true;
                healthArmorF.enabled = true;
            }

            //Also apply resource production
            GameManager.Instance.IncreaseIncome(extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
            GameManager.Instance.ChangeEnergyCap(energyRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
        }
        //Update data for enemies
        GameManager.Instance.playerHealths.Add(location, health);
        GameManager.Instance.playerExtractionData.Add(location, (extractionRate * 40 + energyRate * 100) * damageEffectiveness);
    }

    // Update is called once per frame
    void Update()
    {
        //If activation status is being changed, necessary here to prevent infinite update loops
        if(activate)
        {
            //Changes what is done depending on whether it is being turned on or off
            activate = false;
            if (active)
            {
                //Turn it on
                GameManager.Instance.IncreaseIncome(extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
                GameManager.Instance.ChangeEnergyCap(energyRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
            }
            //Turn it off
            else
            {
                GameManager.Instance.ChangeEnergyCap(-energyRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
            }
        }
    }

    //Send health to callers for use
    public override float GetHealth()
    {
        return health;
    }

    //Add damager to damager list
    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    //Remove damager from damager list
    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    //Heal self, capped to base health
    public override void Heal(float healing)
    {
        if (active)
        {
            GameManager.Instance.IncreaseIncome(-extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
        }
        health = Mathf.Min(baseHealth, health + healing);

        //Update healthbar
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);

        //Update data for enemies
        GameManager.Instance.playerHealths[location] = health;
        if (active)
        {
            GameManager.Instance.IncreaseIncome(extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
        }
    }

    //Upgrade given stat
    public void Upgrade(int type)
    {
        //Increase noted cost
        cost += GetUpgradeCost(type);

        //Remove budget
        GameManager.Instance.budget -= GetUpgradeCost(type);

        //Mark upgraded
        upgradeLevels[type]++;

        //Upgrade depending on type
        switch (type)
        {
            //Increase extraction rate
            case 0:
                {
                    //Update stats
                    if (active)
                    {
                        GameManager.Instance.IncreaseIncome(-extractionRate);
                    }
                    extractionRate *= upgradeEffects[type];
                    if (active)
                    {
                        GameManager.Instance.IncreaseIncome(extractionRate);
                    }

                    //Update data for enemies
                    GameManager.Instance.playerExtractionData[location] = (extractionRate * 40 + energyRate * 100) * damageEffectiveness;
                    break;
                }
            //Increase energy production
            case 1:
                {
                    //Update stats
                    energyRate += upgradeEffects[type];
                    if (active)
                    {
                        GameManager.Instance.ChangeEnergyCap(upgradeEffects[type]);
                    }
                    //Check if this upgrade makes it worth activating this
                    else if(energyRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness) >= GameManager.Instance.usedEnergy - GameManager.Instance.energy + GameManager.Instance.energyDeficit)
                    {
                        GameManager.Instance.energyDeficit += energyCost;
                        GameManager.Instance.mostRecentEnergyDecrease = GameManager.Instance.mostRecentEnergyDecrease.nextChanged;
                        Enable();
                    }

                    //Update data for enemies
                    GameManager.Instance.playerExtractionData[location] = (extractionRate * 40 + energyRate * 100) * damageEffectiveness;
                    break;
                }
            //Increase protection
            case 2:
                {
                    //TODO: fix issue with improper data manipulation
                    //Update stats
                    damageEffectiveness *= upgradeEffects[type] / GameManager.Instance.playerHealth;
                    TakeDamage(0);

                    //Update data for enemies
                    GameManager.Instance.playerExtractionData[location] = (extractionRate * 40 + energyRate * 100) * damageEffectiveness;
                    break;
                }
            //Increase health
            case 3:
                {
                    //Update stats
                    baseHealth *= upgradeEffects[type] * GameManager.Instance.playerHealth;
                    health *= upgradeEffects[type] * GameManager.Instance.playerHealth;

                    //Update data for enemies
                    GameManager.Instance.playerHealths[location] = health;
                    break;
                }
            default:
                {
                    break;
                }
        }
        //Skip energy updates if upgrading energy production
        if (type != 1)
        {
            //Increase energy cost by proper amount for the upgrade
            energyCost += GetUpgradeEnergy(type);

            //If the building is not active update the energy deficit before notifying the manager of the energy usage increase
            if (!active)
            {
                GameManager.Instance.energyDeficit -= GetUpgradeEnergy(type);
            }
            GameManager.Instance.ChangeEnergyUsage(GetUpgradeEnergy(type));
        }
    }

    //Get cost to upgrade given stat
    public float GetUpgradeCost(int type)
    {
        return baseUpgradeCost * Mathf.Pow(1 + expenseModifiers[type] * GameManager.Instance.playerCosts, upgradeLevels[type]) * (1 + expenseModifiers[type]) * GameManager.Instance.playerCosts;
    }

    //Get potential effects of given upgrade
    public string GetUpgradeEffects(int type)
    {
        //Specify alignment informaiton if not done aligning
        if(!finishedAligning)
        {
            //If 2 alignments and 2 major misalignments
            if(maxAlignments == 2)
            {
                //If you have not selected this alignment already, mark it as a possibility
                if(expenseModifiers[type] == 0.5f)
                {
                    return "Select as Alignment";
                }
                //Otherwise say you cannot choose it again
                return "N/A";
            }
            //If you have not selected an alignment, mark all as possibilities
            if(alignments == 0)
            {
                return "Select as Alignment";
            }
            //Mark as a possible misalignment
            if (expenseModifiers[type] == 0.5f)
            {
                return "Select as Misalignment";
            }
            //Only remaining possibility is that you chose it for something, so mention that it is not an option
            return "N/A";
        }

        //Switch based on desired upgrade type
        switch (type)
        {
            //Increase extraction rate
            case 0:
                {
                    return $"{extractionRate:F2} > {extractionRate * upgradeEffects[type]:F2}";
                }
            //Increase energy production
            case 1:
                {
                    return $"{energyRate:F2} > {energyRate + upgradeEffects[type]:F2}";
                }
            //Increase damage protection
            case 2:
                {
                    return $"{damageEffectiveness:F2} > {damageEffectiveness * upgradeEffects[type] / GameManager.Instance.playerHealth:F2}";
                }
            //Increase health
            case 3:
                {
                    return $"{baseHealth:F2} > {baseHealth * upgradeEffects[type] * GameManager.Instance.playerHealth:F2}";
                }
            //Default
            default:
                {
                    return "";
                }
        }
    }
    //Get the description and location of the building
    public string GetDescription()
    {
        return $"{basicDescription}\n({location.x}, {location.y})";
    }

    //Remove building while giving a partial refund
    public override bool Sell()
    {
        //Refund part of build cost
        GameManager.Instance.budget += cost * 0.5f * health / baseHealth;

        //Call removal events
        Remove();

        //Ensures that it is known that building was successfully sold
        return true;
    }

    //Gets energy required for sleected upgrade (not useable yet)
    public float GetUpgradeEnergy(int type)
    {
        return energyCosts[type];
    }

    //Disables in order to save energy usage
    public override float Disable()
    {
        //Only disable if doing so would be a net energy increase
        if(energyRate < energyCost)
        {
            active = false;
            activate = true;
            spriteRenderer.color = Color.black;
            GameManager.Instance.IncreaseIncome(-extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
            return -energyCost;
        }
        return 0;

    }

    //Reenable once energy is available
    public override float Enable()
    {
        //Ensures that it only reenables if it was disabled
        if(!active)
        {
            active = true;
            activate = true;
            spriteRenderer.color = activeColor;
            return energyCost;
        }
        return 0;
    }

    //Events that are lways done when the building is removed
    protected override void Remove()
    {
        //Remove building
        GameManager.Instance.RemoveBuilding(this);

        //Update data for enemies
        GameManager.Instance.playerHealths.Remove(location);
        GameManager.Instance.playerExtractionData.Remove(location);

        //Tell all damagers to stop attacking this
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
        //Ensure that you are aligning to a new alignment
        if (expenseModifiers[type] == 0.5f)
        {
            //If setting an alignment
            if(alignments < maxAlignments)
            {
                //Increase alignment count and set the alignment
                alignments++;
                expenseModifiers[type] = 0.3f;

                //Update sprites to show proper sprites for alignment
                switch(type)
                {
                    //Activate extraction sprite
                    case 0:
                        {
                            extractionCenter.enabled = true;
                            break;
                        }
                    //Activate energy sprite
                    case 1:
                        {
                            energyCenter.enabled = true;
                            break;
                        }
                    //Activate protection sprites
                    case 2:
                        {
                            protectionBarA.enabled = true;
                            protectionBarB.enabled = true;
                            protectionBarC.enabled = true;
                            break;
                        }
                    //Activate health sprites
                    case 3:
                        {
                            healthArmorA.enabled = true;
                            healthArmorB.enabled = true;
                            healthArmorC.enabled = true;
                            healthArmorD.enabled = true;
                            healthArmorE.enabled = true;
                            healthArmorF.enabled = true;
                            break;
                        }
                }

                //If this is a second alignment (max)
                if(alignments == 2)
                {
                    //Sets the remaining alignments to extremely misaligned
                    for(int i = 0; i < expenseModifiers.Length; i++)
                    {
                        if(expenseModifiers[i] == 0.5f)
                        {
                            expenseModifiers[i] = 3f;
                        }
                    }
                    //Marks alignment as done
                    finishedAligning = true;
                }
            }
            //If you are not setting an alignment you are choosing a misalignment
            else
            {
                //If you have already picked a primary misalignment, mark as secondary misalignment and finish alignment
                if(primaryMisalignmentChosen)
                {
                    expenseModifiers[type] = 1.25f;
                    finishedAligning = true;
                }
                //Otherwise set as primary alignment and mark that you have chosen it
                else
                {
                    expenseModifiers[type] = 2f;
                    primaryMisalignmentChosen = true;
                }
            }
        }
    }

    //Allows ohers to check if it is done aligning itself
    public bool IsAligned()
    {
        return finishedAligning;
    }

    //Creates a BuildingData object with the identifying information of this extractor
    public override BuildingData GetAsData()
    {
        BuildingData data = new BuildingData();

        //Sets building type
        data.type = 7;

        //Sets resource extractor data
        data.extractionRate = extractionRate;
        data.energyRate = energyRate;
        data.damageEffectiveness = damageEffectiveness;

        //Sets generic data
        data.health = health;
        data.baseHealth = baseHealth;
        data.energyCost = energyCost;
        data.cost = cost;
        data.location = location;

        //Sets upgrade data
        data.expenseModifiers = expenseModifiers;
        data.upgradeLevels = upgradeLevels;

        //Sets alignment data
        data.maxAlignments = maxAlignments;
        data.alignments = alignments;
        data.finishedAligning = finishedAligning;

        return data;
    }

    //Loads data from a BuildingData object into the extractor
    public override void LoadData(BuildingData data)
    {
        //Extractor date
        extractionRate = data.extractionRate;
        energyRate = data.energyRate;
        damageEffectiveness = data.damageEffectiveness;

        //Generic data
        health = data.health;
        baseHealth = data.baseHealth;
        energyCost = data.energyCost;
        cost = data.cost;
        location = data.location;
        needsDifficultyModifiers = false;

        //Upgrade data
        expenseModifiers = data.expenseModifiers;
        upgradeLevels = data.upgradeLevels;

        //Alignment data
        maxAlignments = data.maxAlignments;
        alignments = data.alignments;
        finishedAligning = data.finishedAligning;

        //Energy management
        GameManager.Instance.energyDeficit += Disable();
        GameManager.Instance.ChangeEnergyUsage(energyCost);
        GameManager.Instance.ChangeEnergyCap(energyRate);
        if (energyRate >= energyCost)
        {
            Enable();
        }
    }

    //Return type for use in identify what to do with this
    public override int GetBuildingType()
    {
        return 7 + maxAlignments;
    }
}
