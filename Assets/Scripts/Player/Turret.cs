using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Turret : PlayerBuilding, IDamager, IUpgradeable
{
    float firerate = 3;
    float damage = 10;
    float health = 20;
    float baseHealth = 20;
    bool splash = false;
    float splashRange = 0.1f;
    bool killed = false;
    [SerializeField] float range = 100;
    private List<IDamager> currentDamagers = new List<IDamager>();
    private Enemy target;
    private Coroutine fireCoroutine = null;
    public int[] upgradeLevels = new int[] { 0, 0, 0, 0, 0 };
    [SerializeField] int maxSpecializations;
    public int specializations;
    public float[] expenseModifiers = new float[] { -1, 1, 1, 1, 1 };
    public float[] upgradeEffects = new float[] { 1.25f, 1.25f, 1.25f, 1.25f, 1.25f };
    // Start is called before the first frame update
    void Start()
    {
        firerate *= GameManager.Instance.playerPower;
        baseHealth *= GameManager.Instance.playerPower;
        health = baseHealth;
        damage *= GameManager.Instance.playerPower;
    }
    public void Upgrade(int type)
    {
        GameManager.Instance.budget -= GetUpgradeCost(type);
        upgradeLevels[type]++;
        switch (type)
        {
            case 0:
                {
                    splashRange *= upgradeEffects[0];
                    break;
                }
            case 1:
                {
                    range *= upgradeEffects[1];
                    break;
                }
            case 2:
                {
                    damage *= upgradeEffects[2];
                    break;
                }
            case 3:
                {
                    firerate *= upgradeEffects[3];
                    break;
                }
            case 4:
                {
                    baseHealth *= upgradeEffects[4];
                    health += upgradeEffects[4];
                    break;
                }
        }
    }

    public float GetUpgradeCost(int type)
    {
        return 2 * Mathf.Pow(1 + 0.25f * expenseModifiers[type], upgradeLevels[type]) * expenseModifiers[type];
    }
    public string GetUpgradeEffects(int type)
    {
        switch(type)
        {
            case 0:
                {
                    return $"{splashRange:F2} > {splashRange * upgradeEffects[type]:F2}";
                }
            case 1:
                {
                    return $"{range:F2} > {range * upgradeEffects[type]:F2}";
                }
            case 2:
                {
                    return $"{damage:F2} > {damage * upgradeEffects[type]:F2}";
                }
            case 3:
                {
                    return $"{firerate:F2} > {firerate * upgradeEffects[type]:F2}";
                }
            case 4:
                {
                    return $"{baseHealth:F2} > {baseHealth * upgradeEffects[type]:F2}";
                }
            default:
                {
                    return "";
                }
        }
    }
    public string GetDescription()
    {
        return $"{basicDescription}\n({location.x}, {location.y})";
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            target = findTargets();
            if(target != null && fireCoroutine == null)
            {
                target.AddDamager(this);
                fireCoroutine = StartCoroutine(fireLoop());
            }
        }
    }

    public override float TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            killed = true;
            GameManager.Instance.playerBuildings.Remove(location);
            if(target != null)
            {
                target.RemoveDamager(this);
                target = null;
            }
            foreach(IDamager damager in currentDamagers)
            {
                damager.cancelAttack();
            }
        }
        return health;
    }

    public void LateUpdate()
    {
        if(killed)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator fireLoop()
    {
        while (target != null)
        {
            if(Vector2.Distance(target.transform.position, transform.position) > range)
            {
                target.RemoveDamager(this);
                target = null;
            }
            else
            {
                target.TakeDamage(damage);
            }
            yield return new WaitForSeconds(10 / firerate);
        }
        fireCoroutine = null;
    }

    private Enemy findTargets()
    {
        Collider2D[] hits =Physics2D.OverlapCircleAll(this.transform.position, range);
        Utils.PriorityQueue<Enemy, float> targets = new Utils.PriorityQueue<Enemy, float>();
        foreach (Collider2D hit in hits)
        {
            if(hit.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                targets.Enqueue(enemy, Mathf.Abs((hit.gameObject.transform.position - transform.position).magnitude));
            }
        }
        if (targets.Count > 0)
        {
            return targets.Dequeue();
        }
        
        return null;

    }

    public override float GetHealth()
    {
        return health;
    }

    public void cancelAttack()
    {
        target = null;
    }

    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    public override void Heal(float healing)
    {
        health = Mathf.Min(healing + health, baseHealth);
    }
}
