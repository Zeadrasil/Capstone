using UnityEngine;

//PlayerBuilding is an abstract class that every single building the player can own is based off of
public abstract class PlayerBuilding : MonoBehaviour, IDamageable
{
    //Location data for referencing
    public Vector2Int location = Vector2Int.zero;

    //Basic description for upgradeables
    [SerializeField] protected string basicDescription;

    //Add damager
    public abstract void AddDamager(IDamager damager);

    //Send callers health
    public abstract float GetHealth();

    //Heal self
    public abstract void Heal(float healing);

    //Remove damager
    public abstract void RemoveDamager(IDamager damager);

    //Take damage to self
    public abstract float TakeDamage(float damage);
}
