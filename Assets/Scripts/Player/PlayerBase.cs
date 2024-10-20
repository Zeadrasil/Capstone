using System.Collections.Generic;
using UnityEngine;

//PlayerBase is your home base, if you lose it you lose the game
public class PlayerBase : PlayerBuilding
{
    //Basic data
    float health = 100;
    float baseHealth = 100;

    //Damagers to avoid null references and improve reaction time
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Add damager to damager list
    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    //Send health to callers for them to use
    public override float GetHealth()
    {
        return health;
    }

    //Heal self by given amount, capped to base health
    public override void Heal(float healing)
    {
        health = Mathf.Min(healing + health, baseHealth);
    }

    //Remove damager from damager list
    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    //Take damage to self and kill if out
    public override float TakeDamage(float damage)
    {
        health -= damage;
        //If sufficiently low
        if(health <= 0)
        {
            //Go through all damagers and tell them to find something else to kill
            foreach(IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
            //Destroy self
            Destroy(gameObject);
        }
        //Return health for utility use
        return health;
    }

}
