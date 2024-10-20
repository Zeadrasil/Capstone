
//IDamager is used to avoid accidental null references and improve attacker reaction time
public interface IDamager
{
    //Entire purpose is here, allow others to tell you to stop bullying them
    public void cancelAttack();
}
