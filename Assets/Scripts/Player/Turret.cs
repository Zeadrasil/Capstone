using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float range = 5;

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
    [SerializeField] int baseUpgradeCost = 10;
    [SerializeField] GameObject rotationCenter;

    //Sprite data
    [SerializeField] Color activeColor = Color.white;
    
    //Barrels
    [SerializeField] SpriteRenderer defaultBarrel;
    [SerializeField] SpriteRenderer firerateBarrelA;
    [SerializeField] SpriteRenderer firerateBarrelB;

    //Hubs
    [SerializeField] SpriteRenderer defaultHub;
    [SerializeField] SpriteRenderer splashHub;
    [SerializeField] SpriteRenderer rangeBase;

    //Tips
    [SerializeField] SpriteRenderer damageTip;

    //Armor
    [SerializeField] SpriteRenderer healthArmorA;
    [SerializeField] SpriteRenderer healthArmorB;
    [SerializeField] SpriteRenderer healthArmorC;
    [SerializeField] SpriteRenderer healthArmorD;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource upgradeSource;

    // Start is called before the first frame update
    void Start()
    {
        //Set default sprite data
        defaultBarrel.enabled = true;
        firerateBarrelA.enabled = false;
        firerateBarrelB.enabled = false;
        defaultHub.enabled = true;
        splashHub.enabled = false;
        rangeBase.enabled = false;
        damageTip.enabled = false;
        healthArmorA.enabled = false;
        healthArmorB.enabled = false;
        healthArmorC.enabled = false;
        healthArmorD.enabled = false;

        //Skip applying difficulty modifiers if already applied due to loading
        if (needsDifficultyModifiers)
        {
            //Applies difficulty modifiers to stats
            firerate *= GameManager.Instance.playerStrength;
            baseHealth *= GameManager.Instance.playerHealth;
            health = baseHealth;
            damage *= GameManager.Instance.playerStrength;
            range *= GameManager.Instance.playerStrength;

            //Alignment handling
            finishedAligning = maxAlignments == alignments;
        }
        //This means it was loaded, so apply sprite data if relevant
        else
        {
            //If aligned with splash, update sprites to match
            if (expenseModifiers[0] == 0.3f)
            {
                splashHub.enabled = true;
                defaultHub.enabled = false;
            }
            //If aligned with range, update sprites to match
            if (expenseModifiers[1] == 0.3f)
            {
                rangeBase.enabled = true;
            }
            //If aligned with damage, update sprites to match
            if (expenseModifiers[2] == 0.3f)
            {
                damageTip.enabled = true;
            }
            //If aligned with firerate, update sprites to match
            if (expenseModifiers[3] == 0.3f)
            {
                firerateBarrelA.enabled = true;
                firerateBarrelB.enabled = true;
                defaultBarrel.enabled = false;
            }
            //If aligned with health, update sprites to match
            if(expenseModifiers[4] == 0.3f)
            {
                healthArmorA.enabled = true;
                healthArmorB.enabled = true;
                healthArmorC.enabled = true;
                healthArmorD.enabled = true;
            }
        }
        //Update data for enemies
        GameManager.Instance.playerHealths.TryAdd(location, health);
        GameManager.Instance.playerDamageData.TryAdd(location, range * damage * firerate * (splash ? Mathf.Pow(splashRange * 10, 2) * 5 : 1));

        //Update volume for upgrade sfx
        upgradeSource.volume = (MusicManager.Instance.masterVolume / 100) * (MusicManager.Instance.sfxVolume / 100);
    }

    //Upgrade given stat
    public void Upgrade(int type)
    {
        //Subtracts upgrade cost from your budget
        GameManager.Instance.budget -= GetUpgradeCost(type);

        upgradeSource.PlayOneShot(upgradeSource.clip);

        cost += GetUpgradeCost(type);

        //Marks as having done another upgrade of this type
        upgradeLevels[type]++;

        //Switch based on desired stat upgrade
        switch (type)
        {
            //Splash range
            case 0:
                {
                    //Update stats
                    splashRange *= upgradeEffects[0] * GameManager.Instance.playerStrength;

                    //Update data for enemies
                    GameManager.Instance.playerDamageData[location] = range * damage * firerate * (splash ? Mathf.Pow(splashRange * 10, 2) * 5 : 1);
                    break;
                }
            //Turret range
            case 1:
                {
                    //Update stats
                    range *= upgradeEffects[1] * GameManager.Instance.playerStrength;

                    //Update data for enemies
                    GameManager.Instance.playerDamageData[location] = range * damage * firerate * (splash ? Mathf.Pow(splashRange * 10, 2) * 5 : 1);
                    break;
                }
            //Damage
            case 2:
                {
                    //Update stats
                    damage *= upgradeEffects[2] * GameManager.Instance.playerStrength;

                    //Update data for enemies
                    GameManager.Instance.playerDamageData[location] = range * damage * firerate * (splash ? Mathf.Pow(splashRange * 10, 2) * 5 : 1);
                    break;
                }
            //Firerate
            case 3:
                {
                    //Update stats
                    firerate *= upgradeEffects[3] * GameManager.Instance.playerStrength;

                    //Update data for enemies
                    GameManager.Instance.playerDamageData[location] = range * damage * firerate * (splash ? Mathf.Pow(splashRange * 10, 2) * 5 : 1);
                    break;
                }
            //Building health
            case 4:
                {
                    //Update stats
                    baseHealth *= upgradeEffects[4] * GameManager.Instance.playerHealth;
                    health += upgradeEffects[4] * GameManager.Instance.playerHealth;

                    //Update data for enemies
                    GameManager.Instance.playerHealths[location] = health;
                    break;
                }
        }
        //Increase energy cost by proper amount for the upgrade
        energyCost += GetUpgradeEnergy(type) * GameManager.Instance.energyConsumption;

        //If the building is not active update the energy deficit before notifying the manager of the energy usage increase
        if (!active)
        {
            GameManager.Instance.energyDeficit -= GetUpgradeEnergy(type) * GameManager.Instance.energyConsumption;
        }
        GameManager.Instance.ChangeEnergyUsage(GetUpgradeEnergy(type) * GameManager.Instance.energyConsumption);
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
                    return $"{splashRange:F2} > {splashRange * upgradeEffects[type] * GameManager.Instance.playerStrength:F2}";
                }
            //Turret range
            case 1:
                {
                    return $"{range:F2} > {range * upgradeEffects[type] * GameManager.Instance.playerStrength:F2}";
                }
            //Damage
            case 2:
                {
                    return $"{damage:F2} > {damage * upgradeEffects[type] * GameManager.Instance.playerStrength:F2}";
                }
            //Firerate
            case 3:
                {
                    return $"{firerate:F2} > {firerate * upgradeEffects[type] * GameManager.Instance.playerStrength:F2}";
                }
            //Building health
            case 4:
                {
                    return $"{baseHealth:F2} > {baseHealth * upgradeEffects[type] * GameManager.Instance.playerHealth:F2}";
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
        if(target != null)
        {
            Vector2 relative = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);
            rotationCenter.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(relative.y, relative.x), new Vector3(0, 0, 1));
        }
    }

    //Take damage and die if low
    public override float TakeDamage(float damage)
    {
        health -= damage;

        //Update health bar
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.4f, 0);

        //Update data for enemies
        GameManager.Instance.playerHealths[location] = health;

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
                    audioSource.volume = (MusicManager.Instance.masterVolume / 100) * (MusicManager.Instance.sfxVolume / 100) * (5 / GameManager.Instance.Camera.orthographicSize);
                    audioSource.PlayOneShot(audioSource.clip); Vector2 relative = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);
                    rotationCenter.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(relative.y, relative.x), new Vector3(0, 0, 1));

                    //Fire Effects
                    BasicUtils.DrawLine(damageTip.transform.position, target.transform.position, new Color(0, 1, 1, 0.75f), 0.5f, 0.1f, true);

                    //If it is splash hit everything within range
                    if (splash)
                    {
                        //Find enemies within range
                        Collider2D[] hits = Physics2D.OverlapCircleAll(target.transform.position, splashRange, LayerMask.GetMask(new string[] { "Enemies" }));
                        foreach(Collider2D hit in hits)
                        {
                            //Confirm that it is an enemy so that it can be accessed
                            Enemy enemy = hit.gameObject.GetComponentInChildren<Enemy>();
                            if (enemy != null)
                            {
                                //Damage the enemy
                                enemy.TakeDamage(damage);
                            }
                        }
                    }
                    //If it is not a splash turret, just be normal
                    else
                    {
                        target.TakeDamage(damage);
                    }
                }
                else
                {
                    //Stop attacking if it does not exist
                    StopCoroutine(fireCoroutine);
                    fireCoroutine = null;
                }
            }
            else
            {
                Vector2 relative = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);
                rotationCenter.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(relative.y, relative.x), new Vector3(0, 0, 1));

                //Fire Effects
                audioSource.volume = (MusicManager.Instance.masterVolume / 100) * (MusicManager.Instance.sfxVolume / 100) * Mathf.Min(05f * damage / GameManager.Instance.Camera.orthographicSize, 1);
                audioSource.PlayOneShot(audioSource.clip);
                BasicUtils.DrawLine(damageTip.transform.position, target.transform.position, new Color(0, 1, 1, 0.75f), 0.5f, 0.1f, true);

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

        //Update health bar
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.4f, 0);

        //Update data for enemies
        GameManager.Instance.playerHealths[location] = health;
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

        //Update data for enemies
        GameManager.Instance.playerHealths.Remove(location);
        GameManager.Instance.playerDamageData.Remove(location);

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
                
                //If it is aligned to splash, make it splashable
                if(type == 0)
                {
                    splash = true;
                    splashRange = 0.1f;
                }

                //Update sprites to show proper sprites for alignment
                switch(type)
                {
                    //Activate splash sprites
                    case 0:
                        {
                            splashHub.enabled = true;
                            defaultHub.enabled = false;
                            break;
                        }
                        //Activate range sprite
                    case 1:
                        {
                            rangeBase.enabled = true;
                            break;
                        }
                        //Activate damage sprite
                    case 2:
                        {
                            damageTip.enabled = true;
                            break;
                        }
                        //Activate firerate sprites
                    case 3:
                        {
                            firerateBarrelA.enabled = true;
                            firerateBarrelB.enabled = true;
                            defaultBarrel.enabled = false;
                            break;
                        }
                        //Activate health sprites
                    case 4:
                        {
                            healthArmorA.enabled = true;
                            healthArmorB.enabled = true;
                            healthArmorC.enabled = true;
                            healthArmorD.enabled = true;
                            break;
                        }
                }


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
        data.type = 0;

        //Sets turret data
        data.damage = damage;
        data.firerate = firerate;
        data.splashRange = splashRange;
        data.splash = splash;

        //Sets generic data
        data.health = health;
        data.baseHealth = baseHealth;
        data.energyCost = energyCost;
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
    public override void LoadData(BuildingData data)
    {
        //Turret data
        damage = data.damage;
        firerate = data.firerate;
        splashRange = data.splashRange;
        splash = data.splash;

        //Generic data
        health = data.health;
        baseHealth = data.baseHealth;
        energyCost = data.energyCost;
        cost = data.cost;
        location = data.location;
        range = data.range;
        needsDifficultyModifiers = false;

        //Upgrade data
        expenseModifiers = data.expenseModifiers;
        upgradeLevels = data.upgradeLevels;

        //Alignment data
        maxAlignments = data.maxAlignments;
        alignments = data.alignments;
        primaryMisalignmentChosen = data.primaryMisalignmentChosen;
        finishedAligning = data.finishedAligning;

        //Energy management
        //Disable();
        GameManager.Instance.energyDeficit += Disable();
        GameManager.Instance.ChangeEnergyUsage(energyCost);
    }

    //Return type for use in identify what to do with this
    public override int GetBuildingType()
    {
        return maxAlignments;
    }
}
