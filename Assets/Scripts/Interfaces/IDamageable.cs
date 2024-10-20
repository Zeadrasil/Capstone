using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float TakeDamage(float damage);
    public void Heal(float healing);
    public float GetHealth();
    public void AddDamager(IDamager damager);
    public void RemoveDamager(IDamager damager);
}
