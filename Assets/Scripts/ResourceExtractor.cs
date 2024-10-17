using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceExtractor : MonoBehaviour, IDamageable
{
    float health = 10;
    float baseHealth = 10;
    float extractionRate = 0.1f;
    public Vector2Int location = Vector2Int.zero;
    public float takeDamage(float damage)
    {
        GameManager.Instance.income -= extractionRate * health * health / (baseHealth * baseHealth);
        health -= damage;
        if(health <= 0)
        {
            GameManager.Instance.playerBuildings.Remove(location);
            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.income += extractionRate * health * health / (baseHealth * baseHealth);
        }
        return health;
    }

    // Start is called before the first frame update
    void Start()
    {
        extractionRate *= GameManager.Instance.playerIncome;
        baseHealth *= GameManager.Instance.playerPower;
        health = baseHealth;
        GameManager.Instance.income += extractionRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getHealth()
    {
        return health;
    }
}
