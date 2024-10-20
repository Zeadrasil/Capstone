using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : PlayerBuilding
{
    float health = 100;
    float baseHealth = 100;
    private List<IDamager> currentDamagers = new List<IDamager>();

    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    public override float GetHealth()
    {
        return health;
    }

    public override void Heal(float healing)
    {
        health = Mathf.Min(healing + health, baseHealth);
    }

    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    public override float TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            foreach(IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
            Destroy(gameObject);
        }
        return health;
    }

}
