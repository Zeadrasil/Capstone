using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBuilding : MonoBehaviour, IDamageable
{
    public Vector2Int location = Vector2Int.zero;
    [SerializeField] protected string basicDescription;

    public abstract void AddDamager(IDamager damager);

    public abstract float GetHealth();

    public abstract void Heal(float healing);

    public abstract void RemoveDamager(IDamager damager);

    public abstract float TakeDamage(float damage);

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
