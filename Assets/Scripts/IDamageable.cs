using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float takeDamage(float damage);
    public float getHealth();
}
