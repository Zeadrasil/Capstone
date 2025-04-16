using UnityEngine;

public class EconomyManager : Singleton<EconomyManager>
{

    //Cost data for new buildings
    public float[] budgetCosts;
    public float[] energyCosts;

    //Economy data
    public float budget;
    private float income;
    public float energy { get; private set; }
    public float usedEnergy { get; private set; }
    public float energyDeficit;

    public float playerIncome = 1;
    public float playerCosts = 1;
    public float energyProduction = 1;
    public float energyConsumption = 1;

    public PlayerBuilding mostRecentEnergyDecrease;

    public void Initialize(float playerEconomy)
    {
        playerIncome = playerEconomy;
        playerCosts = 1 / playerEconomy;
        energyConsumption = 1 / playerEconomy;
        energyProduction = playerEconomy;
    }
    public void Initialize(float playerIncome, float playerCosts, float energyConsumption, float energyProduction)
    {
        this.playerIncome = playerIncome;
        this.playerCosts = playerCosts;
        this.energyConsumption = energyConsumption;
        this.energyProduction = energyProduction;
    }


    public void MidInit()
    {
        usedEnergy = 0;
        income = 0;
        energyDeficit = 0;
        energy = 10;
        budget = 100;

        //Sets the player base as the first item that is affecting energy
        mostRecentEnergyDecrease = BuildingManager.Instance.PlayerBase;

        //Modifies starting values by the difficulty modifiers
        budget *= playerIncome;
        energy *= energyProduction;

        budgetCosts = new float[] { 10, 15, 25, 10, 15, 5, 7.5f, 10, 15, 25 };
        energyCosts = new float[] { 1, 1, 1, 1, 1, 0, 0, 1, 1, 1 };

        //Modifies building costs by difficulty modifier
        for (int i = 0; i < budgetCosts.Length; i++)
        {
            budgetCosts[i] *= playerCosts;
            energyCosts[i] *= energyConsumption;
        }
    }

    public void InitializeLoad(GameData data)
    {
        playerCosts = data.playerCosts;
        playerIncome = data.playerIncome;
        energyProduction = data.energyProduction;
        energyConsumption = data.energyConsumption;

        budget = data.budget;
        energy = 10;
        usedEnergy = 0;
        income = 0;
        energyDeficit = 0;

        budgetCosts = data.budgetCosts;
        energyCosts = data.energyCosts;
        mostRecentEnergyDecrease = BuildingManager.Instance.PlayerBase;
    }


    //Function to increase income in order to improve performance of constant checks
    public void IncreaseIncome(float increase)
    {
        //Increase/decrease income
        income += increase;
        //Update income text to reflect the updated income
        GameManager.Instance.incomeText.text = $"Income: {income:F2} / second";
    }



    //Incrase or decrease max energy
    public void ChangeEnergyCap(float difference)
    {
        energy += difference;
        updateEnergy();
    }

    //Increase or decrease energy usage
    public void ChangeEnergyUsage(float difference)
    {
        usedEnergy += difference;
        updateEnergy();
    }

    //Update all of the energy details
    private void updateEnergy()
    {
        //Update UI
        GameManager.Instance.energyText.text = $"Energy Usage: {usedEnergy:F2} / {energy:F2}";

        //If you are now in an energy deficit not already accounted for
        if (energy - usedEnergy < energyDeficit)
        {
            //Disables one building if possible and runs through checks again
            if (mostRecentEnergyDecrease != null)
            {
                energyDeficit += mostRecentEnergyDecrease.Disable();
                mostRecentEnergyDecrease = mostRecentEnergyDecrease.previousChanged;
                updateEnergy();
            }
        }
        //Otherwise if you have enough energy to reenable a building
        else if (mostRecentEnergyDecrease.nextChanged != null && energy - (usedEnergy + mostRecentEnergyDecrease.nextChanged.energyCost) >= energyDeficit /*- (mostRecentEnergyDecrease.nextChanged.gameObject.TryGetComponent(out ResourceExtractor ext) ? ext.energyRate : 0)*/)
        {
            //Reenable a building and run through checks again
            energyDeficit += mostRecentEnergyDecrease.nextChanged.Enable();
            mostRecentEnergyDecrease = mostRecentEnergyDecrease.nextChanged;
            updateEnergy();
        }
        //Should only run on the very first building as it checks to see if there is no previous or next, which should only happen then
        //Enables the building if it runs
        else if (mostRecentEnergyDecrease.nextChanged == null && mostRecentEnergyDecrease.previousChanged == null && energy - usedEnergy >= mostRecentEnergyDecrease.energyCost && energyDeficit < 0)
        {
            energyDeficit += mostRecentEnergyDecrease.Enable();
            updateEnergy();
        }
    }

    public void TickIncome()
    {
        budget += income * Time.deltaTime;
    }

    public void GetSaveData(ref GameData data)
    {

        data.playerIncome = playerIncome;
        data.playerCosts = playerCosts;
        data.energyProduction = energyProduction;
        data.energyConsumption = energyConsumption;

        data.budget = budget;
        data.budgetCosts = budgetCosts;
        data.energyCosts = energyCosts;
    }

    public bool CanAffordBuilding(int type)
    {
        return budget >= budgetCosts[type];
    }

    public void PlaceBuilding(ref PlayerBuilding building)
    {
        building.cost = budgetCosts[BuildingManager.Instance.selectedConstructionIndex];

        //Add to the very end of enabling queue
        PlayerBuilding holder = mostRecentEnergyDecrease;
        while (holder.nextChanged != null)
        {
            holder = holder.nextChanged;
        }
        holder.nextChanged = building;
        building.previousChanged = holder;

        //Energy management
        building.energyCost = energyCosts[building.GetBuildingType()];
        energyCosts[building.GetBuildingType()] += building.GetBuildingType() == 5 || building.GetBuildingType() == 6 ? 0 : energyConsumption * 0.5f;
        energyDeficit += building.Disable();
        ChangeEnergyUsage(building.energyCost);

        budget -= budgetCosts[BuildingManager.Instance.selectedConstructionIndex];

        //Increases price of new building of that tier in order to encourage using a variety of tiers and upgrading things
        budgetCosts[BuildingManager.Instance.selectedConstructionIndex] *= 1.2f;
    }

    public void RemoveBuilding(PlayerBuilding building)
    {
        int type = building.GetBuildingType();
        //Reduce construction energy cost due to the building being destroyed
        energyCosts[type] -= building.GetBuildingType() == 5 || building.GetBuildingType() == 6 ? 0 : energyConsumption * 0.5f;

        //Reduce energy costs of connected buildings after it is gone to ensure that they take the appropriate amount
        while (building.nextChanged != null)
        {
            building = building.nextChanged;
            if (type == building.GetBuildingType())
            {
                building.energyCost -= energyConsumption * 0.5f;
                ChangeEnergyUsage(-energyConsumption * 0.5f);
            }
        }
        //Ensures that the reference to the most recent change is still valid
        if (building.location == mostRecentEnergyDecrease.location)
        {
            mostRecentEnergyDecrease = mostRecentEnergyDecrease.previousChanged;
        }
    }

    public void Deactivate()
    {
        //Clear out the building references to reduce memory leaks
        while (mostRecentEnergyDecrease.nextChanged != null)
        {
            mostRecentEnergyDecrease = mostRecentEnergyDecrease.nextChanged;
        }
        while (mostRecentEnergyDecrease.previousChanged != null)
        {
            Destroy(mostRecentEnergyDecrease.transform.parent.gameObject);
            mostRecentEnergyDecrease = mostRecentEnergyDecrease.previousChanged;
        }
        try
        {
            Destroy(mostRecentEnergyDecrease.transform.parent.gameObject);
        }
        catch
        {
            try
            {
                Destroy(mostRecentEnergyDecrease.gameObject);
            }
            catch
            {

            }
        }
    }
}
