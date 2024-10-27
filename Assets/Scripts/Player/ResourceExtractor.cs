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
    [SerializeField] int maxSpecializations;
    public int specializations;
    public float[] expenseModifiers = new float[] { 1, 1, 1, 1 };
    public float[] upgradeEffects = new float[] {1.25f, 1.25f, 0.9f, 1.25f };

    //Energy info
    public float[] energyCosts = new float[] { 0.1f, 0f, 0.1f, 0.1f };
    [SerializeField] SpriteRenderer spriteRenderer;
    private bool activate = false;

    //Damager list to avoid null references and improve reaction speed
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Take damage
    public override float TakeDamage(float damage)
    {
        //Remove currently given income
        GameManager.Instance.IncreaseIncome(-extractionRate * health * health / (baseHealth * baseHealth));
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);

        //Decrease health
        health -= damage;

        //If out of health
        if(health <= 0)
        {
            //Remove building
            GameManager.Instance.playerBuildings.Remove(location);

            //Tell all damagers to stop attacking this
            foreach(IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
            //Destroy self
            Destroy(transform.parent.gameObject);
        }
        else
        {
            //Otherwise update income based on reduced rate from reduced health
            GameManager.Instance.IncreaseIncome(extractionRate * health * health / (baseHealth * baseHealth));
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
        health = baseHealth;

        //Applies income
        GameManager.Instance.IncreaseIncome(extractionRate);
    }

    // Update is called once per frame
    void Update()
    {
        if(activate)
        {
            activate = false;
            GameManager.Instance.IncreaseIncome(extractionRate);
            GameManager.Instance.ChangeEnergyCap(energyRate);
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
        health = Mathf.Min(baseHealth, health + healing);
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.55f, 0);
    }

    //Upgrade given stat
    //TODO: Implement
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
                    GameManager.Instance.IncreaseIncome(-extractionRate);
                    extractionRate *= upgradeEffects[type];
                    GameManager.Instance.IncreaseIncome(extractionRate);
                    break;
                }
            //Increase energy production
            case 1:
                {
                    energyRate += upgradeEffects[type];
                    GameManager.Instance.ChangeEnergyCap(upgradeEffects[type]);
                    break;
                }
            //Increase protection
            case 2:
                {
                    damageEffectiveness *= upgradeEffects[type];
                    break;
                }
            //Increase health
            case 3:
                {
                    baseHealth *= upgradeEffects[type];
                    health *= upgradeEffects[type];
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    //Get cost to upgrade given stat
    //TODO: Implement
    public float GetUpgradeCost(int type)
    {
        return 2 * Mathf.Pow(1 + 0.25f * expenseModifiers[type], upgradeLevels[type]) * expenseModifiers[type];
    }

    //Get potential effects of given upgrade
    //TODO: Implement
    public string GetUpgradeEffects(int type)
    {
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
                    return $"{damageEffectiveness:F2} > {damageEffectiveness * upgradeEffects[type]:F2}";
                }
            //Increase health
            case 3:
                {
                    return $"{baseHealth:F2} > {baseHealth * upgradeEffects[type]:F2}";
                }
            //Default
            default:
                {
                    return "";
                }
        }
    }
    //Get the description and location of the building
    //TODO: Implement
    public string GetDescription()
    {
        return $"{basicDescription}\n({location.x}, {location.y})";
    }

    public override bool Sell()
    {
        //Remove from building list
        GameManager.Instance.playerBuildings.Remove(location);

        //Refund part of build cost
        GameManager.Instance.budget += cost * 0.5f * health / baseHealth;

        //Kill building
        Destroy(transform.parent.gameObject);

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
            GameManager.Instance.IncreaseIncome(-extractionRate);
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
            return energyCost;
        }
        return 0;
    }
}
