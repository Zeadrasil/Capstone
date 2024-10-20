using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceExtractor : PlayerBuilding
{
    float health = 10;
    float baseHealth = 10;
    float extractionRate = 0.1f;
    private List<IDamager> currentDamagers = new List<IDamager>();
    public override float TakeDamage(float damage)
    {
        GameManager.Instance.IncreaseIncome(-extractionRate * health * health / (baseHealth * baseHealth));
        health -= damage;
        if(health <= 0)
        {
            GameManager.Instance.playerBuildings.Remove(location);
            foreach(IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.IncreaseIncome(extractionRate * health * health / (baseHealth * baseHealth));
        }
        return health;
    }

    // Start is called before the first frame update
    void Start()
    {
        extractionRate *= GameManager.Instance.playerIncome;
        baseHealth *= GameManager.Instance.playerPower;
        health = baseHealth;
        GameManager.Instance.IncreaseIncome(extractionRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float GetHealth()
    {
        return health;
    }

    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    public override void Heal(float healing)
    {
        health = Mathf.Min(baseHealth, health + healing);
    }

    public void Upgrade(int type)
    {
        throw new System.NotImplementedException();
    }

    public float GetUpgradeCost(int type)
    {
        throw new System.NotImplementedException();
    }

    public string GetUpgradeEffects(int type)
    {
        throw new System.NotImplementedException();
    }

    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }
}
