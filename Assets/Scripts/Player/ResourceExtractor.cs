using System.Collections.Generic;
using UnityEngine;

//ResourceExtractors are used to generate resources for the player
public class ResourceExtractor : PlayerBuilding
{
    //Basic stats
    float health = 10;
    float baseHealth = 10;
    float extractionRate = 0.1f;

    //Damager list to avoid null references and improve reaction speed
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Take damage
    public override float TakeDamage(float damage)
    {
        //Remove currently given income
        GameManager.Instance.IncreaseIncome(-extractionRate * health * health / (baseHealth * baseHealth));

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
            Destroy(gameObject);
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
    }

    //Upgrade given stat
    //TODO: Implement
    public void Upgrade(int type)
    {
        throw new System.NotImplementedException();
    }

    //Get cost to upgrade given stat
    //TODO: Implement
    public float GetUpgradeCost(int type)
    {
        throw new System.NotImplementedException();
    }

    //Get potential effects of given upgrade
    //TODO: Implement
    public string GetUpgradeEffects(int type)
    {
        throw new System.NotImplementedException();
    }
    //Get the description and location of the building
    //TODO: Implement
    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }
}
