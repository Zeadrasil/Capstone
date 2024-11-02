using System.Collections.Generic;
using System.Data;
using UnityEngine;

//RepairStations allow buildings to be repaired, though a repair station can only repair other buildings and repair stations and not itself
public class RepairStation : PlayerBuilding, IDamageable, IUpgradeable
{
    //List of damagers to avoid null references and improve reaction speed
    private List<IDamager> currentDamagers = new List<IDamager>();

    //Basic stats
    private float health;
    int cooldown = 10;
    int baseCooldown = 10;
    [SerializeField] float baseHealth;
    public float healing;
    public float range;

    //Upgrade details
    [SerializeField] float[] expenseModifiers = new float[] { 0.5f, 0.5f, 0.5f };
    [SerializeField] float[] upgradeEffects = new float[] { 1.2f, 1.2f, 1.2f };
    [SerializeField] int[] upgradeLevels = new int[] { 0, 0, 0 };
    [SerializeField] int baseUpgradeCost = 10;

    //Alignment data
    public int maxAlignments = 0;
    private int alignments = 0;
    private bool finishedAligning = false;

    //Energy data
    public float[] energyCosts = new float[] { 0.1f, 0.1f, 0.1f };
    [SerializeField] SpriteRenderer spriteRenderer;

    //Other
    [SerializeField] Color activeColor = Color.white;

    //Add damager to list
    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    //Get the description and location of the building
    public string GetDescription()
    {
        return $"{basicDescription}\n({location.x}, {location.y})";
    }

    //Send health to caller for use
    public override float GetHealth()
    {
        return health;
    }

    //Gets the cost of a specific upgrade type
    public float GetUpgradeCost(int type)
    {
        //Applies various modifiers to the expense to get the cost
        return baseUpgradeCost * Mathf.Pow(1 + expenseModifiers[type] * GameManager.Instance.playerCosts, upgradeLevels[type]) * (1 + expenseModifiers[type]) * GameManager.Instance.playerCosts;
    }

    //Gets a string representing the potential changes of an upgrade
    public string GetUpgradeEffects(int type)
    {
        //Change displayed string if it is still aligning
        if(!finishedAligning)
        {
            if(alignments == 0)
            {
                return "Select as Alignment";
            }
            if (expenseModifiers[type] != 0.3f)
            {
                return "Select as Primary Misalignment";
            }
            return "N/A";
        }
        //Switch based on which upgrade you are checking the effects of
        switch (type)
        {
            //Range
            case 0:
                {
                    return $"{range:F2} > {range * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
                }
            //Healing power
            case 1:
                {
                    return $"{healing:F2} > {healing * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
                }
            //Building health
            case 2:
                {
                    return $"{baseHealth:F2} > {baseHealth * upgradeEffects[type] * GameManager.Instance.playerPower:F2}";
                }
            default:
                {
                    return "";
                }
        }
    }

    //Heal self, capped to base health
    public override void Heal(float healing)
    {
        health = Mathf.Min(health + healing, baseHealth);
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.4f, 0);
    }

    //Remove damager from damager list
    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    //Take damage and kill if low
    public override float TakeDamage(float damage)
    {
        health -= damage;
        healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * 0.5f, -0.4f, 0);
        //If health is at or below 0
        if (health <= 0)
        {
            //Do remove events
            Remove();
        }
        //Return health for utility
        return health;
    }

    //Apply an upgrade to a given stat
    public void Upgrade(int type)
    {
        //Subtract the cost of the upgrade from player budget
        GameManager.Instance.budget -= GetUpgradeCost(type);

        cost += GetUpgradeCost(type);

        //Mark level has having increased
        upgradeLevels[type]++;

        //Switch based on desired upgrade
        switch (type)
        {
            //Range
            case 0:
                {
                    range *= upgradeEffects[0] * GameManager.Instance.playerPower;
                    break;
                }
            //Healing power
            case 1:
                {
                    healing *= upgradeEffects[1] * GameManager.Instance.playerPower;
                    break;
                }
            //Building Health
            case 2:
                {
                    baseHealth *= upgradeEffects[2] * GameManager.Instance.playerPower;
                    health += upgradeEffects[2] * GameManager.Instance.playerPower;
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

    // Start is called before the first frame update
    void Start()
    {
        baseHealth *= GameManager.Instance.playerPower;
        healing *= GameManager.Instance.playerPower;
        range *= GameManager.Instance.playerPower;
        health = baseHealth;

        //If you are unable to specialize, you are finished specializing
        finishedAligning = maxAlignments == 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.Instance.betweenWaves && active)
        {
            //Every so often heal nearby buildings
            if (cooldown == 0)
            {
                //Gets all colliders nearby
                Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), range);

                //Goes through all of them
                foreach (Collider2D hit in hits)
                {
                    //Checks to see if it is a building that is not itself
                    if (hit.gameObject.TryGetComponent(out PlayerBuilding building) && building.gameObject != gameObject)
                    {
                        //Heals it if so
                        building.Heal(healing * 0.2f);
                    }
                }
                //Reset cooldown
                cooldown = baseCooldown;
            }
            else
            {
                //Decrease time on cooldown
                cooldown--;
            }
        }
    }

    //Sell building
    public override bool Sell()
    {
        //Refund part of build cost
        GameManager.Instance.budget += cost * 0.5f * health / baseHealth;

        //Do removal events
        Remove();

        //Ensures that it is known that building was successfully sold
        return true;
    }

    //Get energy required to upgrade stat
    public float GetUpgradeEnergy(int type)
    {
        return energyCosts[type] * GameManager.Instance.playerCosts;
    }

    //Disable repair station to save energy
    public override float Disable()
    {
        active = false;
        spriteRenderer.color = Color.black;
        return -energyCost;
    }

    //Enable repair station after getting enough energy
    public override float Enable()
    {
        active = true;
        spriteRenderer.color = activeColor;
        return energyCost;
    }

    //Events that are always done when the building is removed
    protected override void Remove()
    {
        //Call GameManager removal
        GameManager.Instance.RemoveBuilding(this);

        //Tell all damagers to find something else
        foreach (IDamager damager in currentDamagers)
        {
            damager.cancelAttack();
        }
        //Destroy self
        Destroy(transform.parent.gameObject);
    }

    //Handles alignment
    public void Align(int type)
    {
        //Cannot select alignment twice
        if (expenseModifiers[type] == 0.5f)
        {
            //If you are selecting a misalignment type
            if (maxAlignments == alignments)
            {
                //Type passed in is primary misalignment
                expenseModifiers[type] = 2;

                //Goes through all of the possibilities for misalignment in order to find the one that still has the default alignment
                for (int i = 0; i < expenseModifiers.Length; i++)
                {
                    if (expenseModifiers[i] == 0.5f)
                    {
                        //Sets as secondary misalignment
                        expenseModifiers[i] = 1.25f;
                        finishedAligning = true;
                        return;
                    }
                }
            }
            else
            {
                //Select primary alignment
                alignments++;
                expenseModifiers[type] = 0.3f;
            }
        }
    }

    //Call to get whether this has finished the alignment process
    public bool IsAligned()
    {
        return finishedAligning;
    }

    //Creates a BuildingData object with the identifying information of this repair station
    public override BuildingData GetAsData()
    {
        BuildingData data = new BuildingData();

        //Sets building type
        data.type = 3;

        //Sets repair station data
        data.healing = healing;
        data.baseCooldown = baseCooldown;
        data.cooldown = cooldown;

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
        data.finishedAligning = finishedAligning;

        return data;
    }

    //Loads data from a BuildingData object into the repair station
    //TODO - Implement
    public override void LoadData(BuildingData data)
    {
        throw new System.NotImplementedException();
    }
}
