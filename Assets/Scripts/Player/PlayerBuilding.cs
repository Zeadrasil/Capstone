using UnityEngine;

//PlayerBuilding is an abstract class that every single building the player can own is based off of
public abstract class PlayerBuilding : MonoBehaviour, IDamageable
{
    //Location data for referencing
    public Vector2Int location = Vector2Int.zero;
    [SerializeField] protected GameObject healthBar;
    protected float cost;

    //Energy data
    public float energyCost = 1;
    public bool active = true;
    public PlayerBuilding previousChanged;
    public PlayerBuilding nextChanged;

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

    //Sell in order to refund money and energy
    public abstract bool Sell();

    //Temporarily disable in order to allow for more energy usage, should return a negative number indicating the difference in energy usage
    public abstract float Disable();

    //Reeanable once energy allows, should return a positive number indicating the difference in energy usage
    public abstract float Enable();

    //Remove the building for any reason, destroyed, sold, etc
    protected abstract void Remove();

    //Get the unique data required to recreate a copy using the prefabs
    public abstract BuildingData GetAsData();

    //Take the data from a save file to recreate a copy of the saved building using the prefabs
    public abstract void LoadData(BuildingData data);

}
