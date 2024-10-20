using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : PlayerBuilding, IUpgradeable
{
    private List<IDamager> currentDamagers = new List<IDamager>();
    private float health;
    [SerializeField] float baseHealth;
    [SerializeField] float healingEffectiveness = 1;
    [SerializeField] float[] expenseModifiers = new float[] { 1.25f, 1.25f };
    [SerializeField] float[] upgradeEffects = new float[] { 1.25f, 1.25f };
    [SerializeField] int[] upgradeLevels = new int[] { 0, 0 };
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
                    return $"{baseHealth:F2} > {health * upgradeEffects[type]:F2}";
                }
            case 1:
                {
                    return $"{healingEffectiveness:F2} > {healingEffectiveness * upgradeEffects[type]:F2}";
                }
            default:
                {
                    return "";
                }
        }
    }

    public override void Heal(float healing)
    {
        health = Mathf.Min(health + healing * healingEffectiveness, baseHealth);
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
                    baseHealth *= upgradeEffects[0];
                    health += upgradeEffects[0];
                    break;
                }
            case 1:
                {
                    healingEffectiveness *= upgradeEffects[1];
                    break;
                }
        }
    }

    private void Start()
    {
        health = baseHealth;
    }
}
