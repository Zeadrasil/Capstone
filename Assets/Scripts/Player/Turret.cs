using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Turrets are used to kill enemies
public class Turret : PlayerBuilding, IDamager, IUpgradeable
{
    //Basic stats
    float firerate = 3;
    float damage = 10;
    float health = 20;
    float baseHealth = 20;
    bool splash = false;
    float splashRange = 0.1f;
    [SerializeField] float range = 100;

    //Other
    bool killed = false;
    private Coroutine fireCoroutine = null;
    private Enemy target;

    //Damager list to avoid null references and improve reaction speed
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Upgrade data
    public int[] upgradeLevels = new int[] { 0, 0, 0, 0, 0 };
    [SerializeField] int maxSpecializations;
    public int specializations;
    public float[] expenseModifiers = new float[] { -1, 1, 1, 1, 1 };
    public float[] upgradeEffects = new float[] { 1.25f, 1.25f, 1.25f, 1.25f, 1.25f };

    // Start is called before the first frame update
    void Start()
    {
        //Applies difficulty modifiers to stats
        firerate *= GameManager.Instance.playerPower;
        baseHealth *= GameManager.Instance.playerPower;
        health = baseHealth;
        damage *= GameManager.Instance.playerPower;
    }

    //Upgrade given stat
    public void Upgrade(int type)
    {
        //Subtracts upgrade cost from yourbudget
        GameManager.Instance.budget -= GetUpgradeCost(type);

        //Marks as having done another upgrade of this type
        upgradeLevels[type]++;

        //Switch based on desired stat upgrade
        switch (type)
        {
            //Splash range
            case 0:
                {
                    splashRange *= upgradeEffects[0];
                    break;
                }
            //Turret range
            case 1:
                {
                    range *= upgradeEffects[1];
                    break;
                }
            //Damage
            case 2:
                {
                    damage *= upgradeEffects[2];
                    break;
                }
            //Firerate
            case 3:
                {
                    firerate *= upgradeEffects[3];
                    break;
                }
            //Building health
            case 4:
                {
                    baseHealth *= upgradeEffects[4];
                    health += upgradeEffects[4];
                    break;
                }
        }
    }

    //Gets the cost of upgrading a specific stat
    public float GetUpgradeCost(int type)
    {
        return 2 * Mathf.Pow(1 + 0.25f * expenseModifiers[type], upgradeLevels[type]) * expenseModifiers[type];
    }

    //Gets the potential effects of upgrading a specific stat
    public string GetUpgradeEffects(int type)
    {
        //Switch based on desired upgrade
        switch(type)
        {
            //Splash range
            case 0:
                {
                    return $"{splashRange:F2} > {splashRange * upgradeEffects[type]:F2}";
                }
            //Turret range
            case 1:
                {
                    return $"{range:F2} > {range * upgradeEffects[type]:F2}";
                }
            //Damage
            case 2:
                {
                    return $"{damage:F2} > {damage * upgradeEffects[type]:F2}";
                }
            //Firerate
            case 3:
                {
                    return $"{firerate:F2} > {firerate * upgradeEffects[type]:F2}";
                }
            //Building health
            case 4:
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
    public string GetDescription()
    {
        return $"{basicDescription}\n({location.x}, {location.y})";
    }

    // Update is called once per frame
    void Update()
    {
        //If no target find one
        if(target == null)
        {
            target = findTargets();


            //If a target was found
            if (target != null)
            {
                //Add self as a damager to the target
                target.AddDamager(this);

                //If no firing loop, start one
                if (fireCoroutine == null)
                {
                    fireCoroutine = StartCoroutine(fireLoop());
                }
            }
        }
    }

    //Take damage and die if low
    public override float TakeDamage(float damage)
    {
        health -= damage;
        //If out of health
        if (health <= 0)
        {
            //Mark dead, using indirect system as legacy because idr if I fixed the bug this solved in another way as well
            killed = true;

            //Remove from buildings
            GameManager.Instance.playerBuildings.Remove(location);

            //If there is a target remove self from target's damagers
            if(target != null)
            {
                target.RemoveDamager(this);
                target = null;
            }
            //Tell all damagers to find another target
            foreach(IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
        }
        //Return health for utility
        return health;
    }

    //Called every frame after all Update()s have finished
    public void LateUpdate()
    {
        //End self if killed
        if(killed)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    //Looping task to shoot at enemies
    IEnumerator fireLoop()
    {
        //While there is a target
        while (target != null)
        {
            //Ensure that target is actually close enough to hit
            if(Vector2.Distance(target.transform.position, transform.position) > range)
            {
                target.RemoveDamager(this);

                //Find new target if it is too far
                target = findTargets();

                //Damage new target if it exists
                if(target != null)
                {
                    target.TakeDamage(damage);
                }
                else
                {
                    //Stop attacking if it does not exist
                    StopCoroutine(fireCoroutine);
                }
            }
            else
            {
                //If it is attack it
                target.TakeDamage(damage);
            }
            //Wait until  time to fire again
            yield return new WaitForSeconds(10 / firerate);
        }
        //Reset fire coroutine
        fireCoroutine = null;
    }

    //Find a new target to attack
    private Enemy findTargets()
    {
        //Get all colliders within range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        //Get a priority queue to ensure that you fire at the closest enemy
        Utils.PriorityQueue<Enemy, float> targets = new Utils.PriorityQueue<Enemy, float>();

        //Go through every potential enemy
        foreach (Collider2D hit in hits)
        {
            //If it is an enemy, add it to the queue with the distance between the enemy and the turret as the priority
            if(hit.gameObject.TryGetComponent(out Enemy enemy))
            {
                targets.Enqueue(enemy, Mathf.Abs((hit.gameObject.transform.position - transform.position).magnitude));
            }
        }
        //If there is at least one potential target, return the closest target
        if (targets.Count > 0)
        {
            return targets.Dequeue();
        }
        //Otherwise return null
        return null;

    }

    //Send health to caller for use
    public override float GetHealth()
    {
        return health;
    }

    //Cancel attack to improve reaction and avoid null references
    public void cancelAttack()
    {
        target = null;
    }

    //Add damager to list of damagers
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
        health = Mathf.Min(healing + health, baseHealth);
    }
}
