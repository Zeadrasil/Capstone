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
    [SerializeField] float[] expenseModifiers = new float[] { 1.25f, 1.25f };
    [SerializeField] float[] upgradeEffects = new float[] { 1.25f, 1.25f };
    [SerializeField] int[] upgradeLevels = new int[] { 0, 0 };

    //Add damager to damager list
    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    //Get description and location of the building
    public string GetDescription()
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
        return 2 * Mathf.Pow(1 + 0.25f * expenseModifiers[type], upgradeLevels[type]) * expenseModifiers[type];
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
                    return $"{baseHealth:F2} > {health * upgradeEffects[type]:F2}";
                }
            //Healing effectiveness
            case 1:
                {
                    return $"{healingEffectiveness:F2} > {healingEffectiveness * upgradeEffects[type]:F2}";
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
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.65f, 0);
    }

    //Remove damager from damager list
    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    //Sell building
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

    //Take damage to self and kill if out of health
    public override float TakeDamage(float damage)
    {
        health -= damage;
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.65f, 0);
        //If out of health
        if (health <= 0)
        {
            //Remove from buildings
            GameManager.Instance.playerBuildings.Remove(location);

            //Tell all damagers t find something else to attack
            foreach (IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
            //Destroy self
            Destroy(transform.parent.gameObject);
        }
        //Return health for utility
        return health;
    }

    //Upgrade specific stat
    public void Upgrade(int type)
    {
        //Take upgrade cost out of budget
        GameManager.Instance.budget -= GetUpgradeCost(type);

        cost += GetUpgradeCost(type);

        //Mark stat as having been upgraded
        upgradeLevels[type]++;

        //Switch based on desired stat upgrade
        switch (type)
        {
            //Health
            case 0:
                {
                    baseHealth *= upgradeEffects[0];
                    health += upgradeEffects[0];
                    break;
                }
            //Healing effectiveness
            case 1:
                {
                    healingEffectiveness *= upgradeEffects[1];
                    break;
                }
        }
    }

    private void Start()
    {
        health = baseHealth;
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
}
