using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow;

//Turrets are used to kill enemies
public class Turret : PlayerBuilding, IDamager, IUpgradeable
{
    //Basic stats
    public float firerate = 3;
    public float damage = 10;
    float health = 20;
    [SerializeField] float baseHealth = 20;
    bool splash = false;
    float splashRange = 0.1f;
    public float range = 100;

    //Other
    bool killed = false;
    private Coroutine fireCoroutine = null;
    private Enemy target;

    //Damager list to avoid null references and improve reaction speed
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Upgrade data
    public int[] upgradeLevels = new int[] { 0, 0, 0, 0, 0 };
    public float[] expenseModifiers = new float[] { float.NegativeInfinity, 0.5f, 0.5f, 0.5f, 0.5f };
    public float[] upgradeEffects = new float[] { 1.2f, 1.2f, 1.2f, 1.2f, 1.2f };

    //Alignment data
    public int maxAlignments = 0;
    private int alignments = 0;
    private bool primaryMisalignmentChosen = false;
    private bool finishedAligning = false;

    //Energy info
    public float[] energyCosts = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    [SerializeField] SpriteRenderer spriteRenderer;

    //Other
    [SerializeField] Color activeColor = Color.white;
    [SerializeField] int baseUpgradeCost = 10;

    // Start is called before the first frame update
    void Start()
    {
        //Applies difficulty modifiers to stats
        firerate *= GameManager.Instance.playerPower;
        baseHealth *= GameManager.Instance.playerPower;
        health = baseHealth;
        damage *= GameManager.Instance.playerPower;
        range *= GameManager.Instance.playerPower;

        finishedAligning = maxAlignments == 0;
    }

    //Upgrade given stat
    public void Upgrade(int type)
    {
        //Subtracts upgrade cost from yourbudget
        GameManager.Instance.budget -= GetUpgradeCost(type);

        cost += GetUpgradeCost(type);

        //Marks as having done another upgrade of this type
        upgradeLevels[type]++;

        //Switch based on desired stat upgrade
        switch (type)
        {
            //Splash range
            case 0:
                {
                    splashRange *= upgradeEffects[0] * GameManager.Instance.playerPower;
                    break;
                }
            //Turret range
            case 1:
                {
                    range *= upgradeEffects[1] * GameManager.Instance.playerPower;
                    break;
                }
            //Damage
            case 2:
                {
                    damage *= upgradeEffects[2] * GameManager.Instance.playerPower;
                    break;
                }
            //Firerate
            case 3:
                {
                    firerate *= upgradeEffects[3] * GameManager.Instance.playerPower;
                    break;
                }
            //Building health
            case 4:
                {
                    baseHealth *= upgradeEffects[4] * GameManager.Instance.playerPower;
                    health += upgradeEffects[4] * GameManager.Instance.playerPower;
                    break;
                }
        }
        //Increase energy cost by proper amount for the upgrade
        energyCost += GetUpgradeEnergy(type);

        //If the building is not active update the energy deficit before notifying the manager of the energy usage increase
        if (!active)
        {
            GameManager.Instance.energyDeficit -= GetUpgradeEnergy(type);
        }
        GameManager.Instance.ChangeEnergyUsage(GetUpgradeEnergy(type));
    }

    //Gets the cost of upgrading a specific stat
    public float GetUpgradeCost(int type)
    {
        return baseUpgradeCost * Mathf.Pow(1 + expenseModifiers[type] * GameManager.Instance.playerCosts, upgradeLevels[type]) * (1 + expenseModifiers[type]) * GameManager.Instance.playerCosts;
    }

    //Gets the potential effects of upgrading a specific stat
    public string GetUpgradeEffects(int type)
    {
        //Specify alignment informaiton if not done aligning
        if (!finishedAligning)
        {
            //If 2 alignments and 2 major misalignments
            if (maxAlignments == 2)
            {
                //If you have not selected this alignment already, mark it as a possibility
                if (expenseModifiers[type] == 0.5f || expenseModifiers[type] == -1)
                {
                    return "Select as Alignment";
                }
                //Otherwise say you cannot choose it again
                return "N/A";
            }
            //If you have not selected an alignment, mark all as possibilities
            if (alignments == 0)
            {
                return "Select as Alignment";
            }
            //Mark as a possible misalignment
            if (expenseModifiers[type] == 0.5f)
            {
                return "Select as Misalignment";
            }
            //Only remaining possibilities are that you chose it for something or it is splash, so mention that it is not an option
            return "N/A";
        }
        //Switch based on desired upgrade
        switch (type)
        {
            //Splash range
            case 0:
                {
                    return $"{splashRange:F2} > {splashRange * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
                }
            //Turret range
            case 1:
                {
                    return $"{range:F2} > {range * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
                }
            //Damage
            case 2:
                {
                    return $"{damage:F2} > {damage * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
                }
            //Firerate
            case 3:
                {
                    return $"{firerate:F2} > {firerate * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
                }
            //Building health
            case 4:
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

    // Update is called once per frame
    void Update()
    {
        //If no target find one
        if(active && target == null)
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
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.4f, 0);
        //If out of health
        if (health <= 0)
        {
            //Call removal events
            Remove();
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
            Destroy(transform.parent.gameObject);
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
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.4f, 0);
    }

    //Sell building
    public override bool Sell()
    {
        //Call remove events
        Remove();

        //Refund part of build cost
        GameManager.Instance.budget += cost * 0.5f * health / baseHealth;

        //Ensures that it is known that building was successfully sold
        return true;
    }

    //Get energy required to upgrade stat
    public float GetUpgradeEnergy(int type)
    {
        return energyCosts[type];
    }

    //Disable turret to save energy
    public override float Disable()
    {
        StopAllCoroutines();
        active = false;
        spriteRenderer.color = Color.black;
        return -energyCost;
    }

    //Enable turret once you get enough energy
    public override float Enable()
    {
        target = null;
        active = true;
        spriteRenderer.color = activeColor;
        return energyCost;
    }

    //Events that are always done when the building is removed
    protected override void Remove()
    {
        //Mark dead, using indirect system as legacy because idr if I fixed the bug this solved in another way as well
        killed = true;

        //Remove building
        GameManager.Instance.RemoveBuilding(this);

        //If there is a target remove self from target's damagers
        if (target != null)
        {
            target.RemoveDamager(this);
            target = null;
        }
        //Tell all damagers to find another target
        foreach (IDamager damager in currentDamagers)
        {
            damager.cancelAttack();
        }
    }

    //Handles alignment
    public void Align(int type)
    {
        //Ensure that you are aligning to a new alignment
        if (expenseModifiers[type] == 0.5f || expenseModifiers[type] < 0)
        {
            //If setting an alignment
            if (alignments < maxAlignments)
            {
                //Increase alignment count and set the alignment
                alignments++;
                expenseModifiers[type] = 0.3f;

                //If this is a second alignment (max)
                if (alignments == 2)
                {
                    //Sets the remaining alignments to extremely misaligned
                    for (int i = 0; i < expenseModifiers.Length; i++)
                    {
                        if (expenseModifiers[i] == 0.5f)
                        {
                            expenseModifiers[i] = 3f;
                        }
                    }
                    //Marks alignment as done
                    finishedAligning = true;
                }
            }
            //If you are not setting an alignment you are choosing a misalignment, excluding splash
            else if(expenseModifiers[type] > 0)
            {
                //If you have already picked a primary misalignment, mark as secondary misalignment and finish alignment
                if (primaryMisalignmentChosen)
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

    //Creates a BuildingData object with the identifying information of this turret
    public override BuildingData GetAsData()
    {
        BuildingData data = new BuildingData();

        //Sets building type
        data.type = 5;

        //Sets turret data
        data.damage = damage;
        data.firerate = firerate;
        data.splashRange = splashRange;
        data.splash = splash;

        //Sets generic data
        data.health = health;
        data.baseHealth = baseHealth;
        data.cost = cost;
        data.location = location;
        data.range = range;

        //Sets upgrade data
        data.expenseModifiers = expenseModifiers;
        data.upgradeLevels = upgradeLevels;

        //Sets alignment data
        data.maxAlignments = maxAlignments;
        data.alignments = alignments;
        data.primaryMisalignmentChosen = primaryMisalignmentChosen;
        data.finishedAligning = finishedAligning;

        return data;
    }

    //Loads data from a BuildingData object into the turret
    //TODO - Implement
    public override void LoadData(BuildingData data)
    {
        throw new System.NotImplementedException();
    }
}
