using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    public class LocalBuildingManager : MonoBehaviour
    {
        //Building prefabs
        [SerializeField] GameObject turretTierOne;
        [SerializeField] GameObject turretTierTwo;
        [SerializeField] GameObject turretTierThree;
        [SerializeField] GameObject repairTierOne;
        [SerializeField] GameObject repairTierTwo;
        [SerializeField] GameObject wallTierOne;
        [SerializeField] GameObject wallTierTwo;
        [SerializeField] GameObject extractorTierOne;
        [SerializeField] GameObject extractorTierTwo;
        [SerializeField] GameObject extractorTierThree;


        [SerializeField] PlayerBase playerBase;

    private void Start()
    {

        //Pass building prefabs
        BuildingManager.Instance.turretTierOne = turretTierOne;
        BuildingManager.Instance.turretTierTwo = turretTierTwo;
        BuildingManager.Instance.turretTierThree = turretTierThree;
        BuildingManager.Instance.repairTierOne = repairTierOne;
        BuildingManager.Instance.repairTierTwo = repairTierTwo;
        BuildingManager.Instance.wallTierOne = wallTierOne;
        BuildingManager.Instance.wallTierTwo = wallTierTwo;
        BuildingManager.Instance.extractorTierOne = extractorTierOne;
        BuildingManager.Instance.extractorTierTwo = extractorTierTwo;
        BuildingManager.Instance.extractorTierThree = extractorTierThree;

        BuildingManager.Instance.PlayerBase = playerBase;
    }

    //Relay for turret upgrades using buttons
    public void UpgradeTurret(int upgrade)
    {
        BuildingManager.Instance.UpgradeBuilding(0, upgrade);
    }
    //Relay for repair station upgrades using buttons
    public void UpgradeRepairStation(int upgrade)
    {
        BuildingManager.Instance.UpgradeBuilding(1, upgrade);
    }
    //Relay for wall upgrades using buttons
    public void UpgradeWall(int upgrade)
    {
        BuildingManager.Instance.UpgradeBuilding(2, upgrade);
    }
    //Relay for extractor upgrades using buttons
    public void UpgradeExtractor(int upgrade)
    {
        BuildingManager.Instance.UpgradeBuilding(3, upgrade);
    }
    //Relay for selling buildings using buttons
    public void Sell()
    {
        BuildingManager.Instance.Sell();
    }
}
