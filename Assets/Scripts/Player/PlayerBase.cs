using System.Collections.Generic;
using UnityEngine;

//PlayerBase is your home base, if you lose it you lose the game
public class PlayerBase : PlayerBuilding
{
    //Basic data
    float health = 100;
    float baseHealth = 100;

    //Damagers to avoid null references and improve reaction time
    private List<IDamager> currentDamagers = new List<IDamager>();

    [SerializeField] AudioSource audioSource;

    //Add damager to damager list
    public override void AddDamager(IDamager damager)
    {
        currentDamagers.Add(damager);
    }

    private void Start()
    {
        baseHealth *= GameManager.Instance.playerHealth;
        audioSource.volume = (MusicManager.Instance.masterVolume / 100) * (MusicManager.Instance.sfxVolume / 500);
        health = baseHealth;
    }

    //Cannot disable your base
    public override float Disable()
    {
        return 0;
    }

    //Cannot enable your base
    public override float Enable()
    {
        return 0;
    }

    //Send health to callers for them to use
    public override float GetHealth()
    {
        return health;
    }

    //Heal self by given amount, capped to base health
    public override void Heal(float healing)
    {
        health = Mathf.Min(healing + health, baseHealth);
    }

    //Remove damager from damager list
    public override void RemoveDamager(IDamager damager)
    {
        currentDamagers.Remove(damager);
    }

    public override bool Sell()
    {
        return false;
    }

    //Take damage to self and kill if out
    public override float TakeDamage(float damage)
    {
        audioSource.PlayOneShot(audioSource.clip);
        health -= damage;
        //healthBar.transform.localScale = new Vector3(health / baseHealth, 0.1f, 1);
        //healthBar.transform.localPosition = new Vector3((-1 + health / baseHealth) * -0.5f, -0.4f, 0);
        //If sufficiently low
        if (health <= 0)
        {
            //Run through kill events
            Remove();
        }
        //Return health for utility use
        return health;
    }

    //Only called on kill here for now
    protected override void Remove()
    {
        //Go through all damagers and tell them to find something else to kill
        foreach (IDamager damager in currentDamagers)
        {
            damager.cancelAttack();
        }
        //Destroy self
        //Destroy(gameObject, GameManager.Instance.playerBuildings.Count * 0.2f + 1);
        //Self gets destroyed in the deactivate function

        //Stop music
        MusicManager.Instance.StopBattle();

        //Go back to main menu
        MenuManager.Instance.Return();
    }

    //Attempt to get building data, should not be possible to call this function on this building
    public override BuildingData GetAsData()
    {
        throw new System.NotImplementedException();
    }

    //Attempt to load building data, should not be possible to call this function on this building
    public override void LoadData(BuildingData data)
    {
        throw new System.NotImplementedException();
    }

    //Get building type for convenient way to determine what it is
    public override int GetBuildingType()
    {
        return -1;
    }
}
