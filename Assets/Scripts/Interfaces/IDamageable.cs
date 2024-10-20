
//IDamageable is used for all things that can take damage
public interface IDamageable
{
    //Take damage, self explanatory
    public float TakeDamage(float damage);

    //Heal self, only reason it is not the same as Take Damage is to make it so that you don't need to check for kill on healing and overheal on damaging
    public void Heal(float healing);

    //Get health so that others can know
    public float GetHealth();

    //Add damager to ensure that you can change targets at a decent rate
    public void AddDamager(IDamager damager);

    //Remove damager to ensure that you do not try to tell a nonexistent damager to stop attacking you
    public void RemoveDamager(IDamager damager);
}
