using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //Other
    [SerializeField] Color activeColor = Color.white;

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
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);

        //Decrease health
        health -= damage;

        //If out of health
        if(health <= 0)
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
        //Modifies extraction rate and health based on difficulty modifiers
        extractionRate *= GameManager.Instance.playerIncome;
        baseHealth *= GameManager.Instance.playerPower;
        damageEffectiveness /= GameManager.Instance.playerPower;
        health = baseHealth;

        //Applies income
        GameManager.Instance.IncreaseIncome(extractionRate);

        //Figures out alignment config
        finishedAligning = maxAlignments == 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(activate)
        {
            activate = false;
            GameManager.Instance.IncreaseIncome(extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
            GameManager.Instance.ChangeEnergyCap(energyRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
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
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);
        health = Mathf.Min(baseHealth, health + healing);
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
                    if (active)
                    {
                        GameManager.Instance.IncreaseIncome(-extractionRate);
                    }
                    extractionRate *= upgradeEffects[type];
                    if (active)
                    {
                        GameManager.Instance.IncreaseIncome(extractionRate);
                    }
                    break;
                }
            //Increase energy production
            case 1:
                {
                    energyRate += upgradeEffects[type];
                    if (active)
                    {
                        GameManager.Instance.ChangeEnergyCap(upgradeEffects[type]);
                    }
                    else if(energyRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness) >= energyCost)
                    {
                        GameManager.Instance.energyDeficit += energyCost;
                        Enable();
                    }
                    break;
                }
            //Increase protection
            case 2:
                {
                    damageEffectiveness *= upgradeEffects[type] / GameManager.Instance.playerPower;
                    TakeDamage(0);
                    break;
                }
            //Increase health
            case 3:
                {
                    baseHealth *= upgradeEffects[type] * GameManager.Instance.playerPower;
                    health *= upgradeEffects[type] * GameManager.Instance.playerPower;
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
                    return $"{damageEffectiveness:F2} > {damageEffectiveness * upgradeEffects[type] / GameManager.Instance.playerPower:F2}";
                }
            //Increase health
            case 3:
                {
                    return $"{baseHealth:F2} > {baseHealth * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
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
            spriteRenderer.color = Color.black;
            GameManager.Instance.IncreaseIncome(-extractionRate * Mathf.Pow(health, damageEffectiveness) / Mathf.Pow(baseHealth, damageEffectiveness));
            GameManager.Instance.ChangeEnergyCap(-energyRate);
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
}
