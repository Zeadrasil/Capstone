using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStation : PlayerBuilding, IDamageable, IUpgradeable
{
    private List<IDamager> currentDamagers = new List<IDamager>();
    private float health;
    int cooldown = 10;
    int baseCooldown = 10;
    [SerializeField] float baseHealth;
    [SerializeField] float healing;
    [SerializeField] float range;
    [SerializeField] float[] expenseModifiers = new float[] { 1.25f, 1.25f, 1.25f };
    [SerializeField] float[] upgradeEffects = new float[] { 1.25f, 1.25f, 1.25f };
    [SerializeField] int[] upgradeLevels = new int[] { 0, 0, 0 };
    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    public string GetDescription()
    {
        return $"{basicDescription}\n({location.x}, {location.y})";
    }

    public override float GetHealth()
    {
        return health;
    }

    public float GetUpgradeCost(int type)
    {
        return 2 * Mathf.Pow(1 + 0.25f * expenseModifiers[type], upgradeLevels[type]) * expenseModifiers[type];
    }

    public string GetUpgradeEffects(int type)
    {
        switch (type)
        {
            case 0:
                {
                    return $"{range:F2} > {range * upgradeEffects[type]:F2}";
                }
            case 1:
                {
                    return $"{healing:F2} > {healing * upgradeEffects[type]:F2}";
                }
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

    public override void Heal(float healing)
    {
        health = Mathf.Min(health + healing, baseHealth);
    }

    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    public override float TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.Instance.playerBuildings.Remove(location);
            foreach (IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
            Destroy(gameObject);
        }
        return health;
    }

    public void Upgrade(int type)
    {
        GameManager.Instance.budget -= GetUpgradeCost(type);
        upgradeLevels[type]++;
        switch (type)
        {
            case 0:
                {
                    range *= upgradeEffects[0];
                    break;
                }
            case 1:
                {
                    healing *= upgradeEffects[1];
                    break;
                }
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
        if(cooldown == 0)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), range);
            foreach(Collider2D hit in hits)
            {
                if(hit.gameObject.TryGetComponent(out PlayerBuilding building))
                {
                    building.Heal(healing * 0.2f);
                }
            }
            cooldown = baseCooldown;
        }
        else
        {
            cooldown--;
        }
    }
}
