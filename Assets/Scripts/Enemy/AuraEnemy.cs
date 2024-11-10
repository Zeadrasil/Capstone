using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Derived Enemy class that has an aura with some effect

public class AuraEnemy : Enemy
{
    //Number of aura instances
    public int auraCount;

    //How far away an enemy can be and still get the boosts
    private float auraRange;

    //Stores all of the aura instances
    int[] activeAuras = { 0, 0, 0, 0, 0 };

    //Auras update 2.5x per second
    int auraApplicationCooldown = 0;
    int baseAuraApplicationCooldown = 19;

    //Stores affected enemies so that enemies that leave the aura lose the benefits
    List<Enemy> affectedEnemies = new List<Enemy>();

    //Start is called before the first frame update
    protected override void Start()
    {
        //Call standard enemy start events
        base.Start();

        //Can have multiple instances of the same aura
        while(auraCount > 0)
        {
            //Determines which type of aura this instance is
            int at = BasicUtils.WrappedRandomRange(0, activeAuras.Length);
            activeAuras[at]++;

            //Spent over 5 hours improving enemy spawn performance only to realize that a lack of altering the aura count is what was causing it to freeze
            auraCount--;
        }
    }

    //Called exactly 50 times per second
    protected override void FixedUpdate()
    {
        //Call base enemy update events
        base.FixedUpdate();

        //Limit aura application rate to increase performance and balance
        if(auraApplicationCooldown == 0)
        {
            //Store the new enemies that will have the auras applied to them
            List<Enemy> newEnemies = new List<Enemy>();

            //Get all colliders within aura range
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, auraRange);

            //Find all of the colliders attached to enemies
            foreach(Collider2D hit in hits)
            {
                Enemy enemy = hit.gameObject.GetComponentInChildren<Enemy>();
                //Add to new enemy list if it is an enemy
                if(enemy != null)
                {
                    newEnemies.Add(enemy);
                }
            }
            //Go through all of the current enemies and remove all of the aura effects
            foreach(Enemy enemy in affectedEnemies)
            {
                //Remove the effects of all aura types
                for (int i = 0; i < activeAuras.Length; i++)
                {
                    enemy.RemoveAura(i, activeAuras[i]);
                }
            }
            //Go through all of the new enemies and add the aura effects
            foreach(Enemy enemy in newEnemies)
            {
                //Add the effects of all aura types
                for (int i = 0; i < activeAuras.Length; i++)
                {
                    enemy.ApplyAura(i, activeAuras[i]);
                }
            }
            //Reset application cooldown
            auraApplicationCooldown = baseAuraApplicationCooldown;

            //Swap out the old enemies for the new
            affectedEnemies = newEnemies;
        }
        else
        {
            //Reduce cooldown to progress towards next aura update
            auraApplicationCooldown--;
        }
    }

    //Override type return to prevent issues later with carrier if that ever gets made
    protected override int GetEnemyType()
    {
        return 1;
    }
}
