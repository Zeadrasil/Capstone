using System.Collections.Generic;
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
    [SerializeField] float healing;
    [SerializeField] float range;

    //Upgrade details
    [SerializeField] float[] expenseModifiers = new float[] { 1.25f, 1.25f, 1.25f };
    [SerializeField] float[] upgradeEffects = new float[] { 1.25f, 1.25f, 1.25f };
    [SerializeField] int[] upgradeLevels = new int[] { 0, 0, 0 };

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
        return 2 * Mathf.Pow(1 + 0.25f * expenseModifiers[type], upgradeLevels[type]) * expenseModifiers[type];
    }

    //Gets a string representing the potential changes of an upgrade
    public string GetUpgradeEffects(int type)
    {
        //Switch based on which upgrade you are checking the effects of
        switch (type)
        {
            //Range
            case 0:
                {
                    return $"{range:F2} > {range * upgradeEffects[type]:F2}";
                }
            //Healing power
            case 1:
                {
                    return $"{healing:F2} > {healing * upgradeEffects[type]:F2}";
                }
            //Building health
            case 2:
                {
                    return $"{baseHealth:F2} > {baseHealth * upgradeEffects[type]:F2}";
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
            //Remove from building list
            GameManager.Instance.playerBuildings.Remove(location);

            //Tell all damagers to find something else
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
                    range *= upgradeEffects[0];
                    break;
                }
            //Healing power
            case 1:
                {
                    healing *= upgradeEffects[1];
                    break;
                }
            //Building Health
            case 2:
                {
                    baseHealth *= upgradeEffects[2];
                    health += upgradeEffects[2];
                    break;
                }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        health = baseHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Every so often heal nearby buildings
        if(cooldown == 0)
        {
            //Gets all colliders nearby
            Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), range);
            
            //Goes through all of them
            foreach(Collider2D hit in hits)
            {
                //Checks to see if it is a building that is not itself
                if(hit.gameObject.TryGetComponent(out PlayerBuilding building) && building.gameObject != gameObject)
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
}
