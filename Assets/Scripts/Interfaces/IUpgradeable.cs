
//IUpgradeable is used for playerbuildings that can be upgraded
public interface IUpgradeable
{
    //Upgrade a given stat
    public void Upgrade(int type);

    //Get the cost required for upgrading a given stat
    public float GetUpgradeCost(int type);

    //Get the string showing the effects of upgrading a given stat
    public string GetUpgradeEffects(int type);

    //Gets the description string for the upgrade window
    public string GetDescription();

    //Gets the energy required for upgrading a specific stat
    public float GetUpgradeEnergy(int type);

    //Handles alignment, which is choosing what a building is good at and bad at
    public void Align(int type);

    //Helper method so that the GameManager can ask if it has finished aligning itself
    public bool IsAligned();
}
