using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{

    //Building array, null because prefabs are not yet passed in when this is created so it need to be done in Initialize()
    private GameObject[] buildings = null;

    //Other data
    public PlayerBase PlayerBase;


    //Stores data for building new buildings
    public GameObject selectedConstruction;
    public int selectedConstructionIndex;


    public Dictionary<Vector2Int, GameObject> playerBuildings = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, float> playerHealths = new Dictionary<Vector2Int, float>();
    public Dictionary<Vector2Int, float> playerExtractionData = new Dictionary<Vector2Int, float>();
    public Dictionary<Vector2Int, float> playerDamageData = new Dictionary<Vector2Int, float>();
    public Dictionary<Vector2Int, float> playerRepairData = new Dictionary<Vector2Int, float>();

    //Building prefabs
    public GameObject turretTierOne;
    public GameObject turretTierTwo;
    public GameObject turretTierThree;
    public GameObject repairTierOne;
    public GameObject repairTierTwo;
    public GameObject wallTierOne;
    public GameObject wallTierTwo;
    public GameObject extractorTierOne;
    public GameObject extractorTierTwo;
    public GameObject extractorTierThree;


    public GameObject selectedBuilding;
    private int selectedUpgrade;

    public void MidInit()
    {
        playerBuildings.Clear();
        selectedConstructionIndex = -1;
        selectedConstruction = null;
    }

    public void InitializeLoad(GameData data)
    {
        playerBuildings.Clear();
        selectedConstructionIndex = -1;
        selectedConstruction = null;

        BuildingData[] buildingDataArray = data.buildings;

        //Go through all of the loaded buildings in order to place them on the map
        foreach (BuildingData buildingData in buildingDataArray)
        {
            //Create the gameObject and get the building from it
            GameObject go = Instantiate(buildings[buildingData.type + buildingData.maxAlignments], TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(buildingData.location.x, buildingData.location.y)), Quaternion.identity);
            PlayerBuilding pb = go.GetComponentInChildren<PlayerBuilding>();

            //Add to the very end of enabling queue
            PlayerBuilding holder = EconomyManager.Instance.mostRecentEnergyDecrease;
            while (holder.nextChanged != null)
            {
                holder = holder.nextChanged;
            }
            holder.nextChanged = pb;
            pb.previousChanged = holder;

            //Load saved data
            pb.LoadData(buildingData);

            //Add to the tracked building dictionary
            playerBuildings.Add(buildingData.location, go);
        }
    }

    public void Initialize()
    {
        //Sets the buildings array since the data has now been passed in
        buildings = new GameObject[] { turretTierOne, turretTierTwo, turretTierThree, repairTierOne, repairTierTwo, wallTierOne, wallTierTwo, extractorTierOne, extractorTierTwo, extractorTierThree };

        playerBuildings.Clear();
        playerBuildings.Add(new Vector2Int(0, 0), PlayerBase.gameObject);

        selectedBuilding = null;
        selectedUpgrade = -1;
    }

    public void Build(int type)
    {

        //Ensure that stored data is the same as the new type
        selectedConstructionIndex = type;

        //Ensure that upgrades get cleared
        selectedBuilding = null;
        standardUpgradeEvents();

        //Clear construction data
        standardConstructionEvents();
        //Do not try to instantiate a new wave or no selection
        if (type != -1 && type != 10 && EconomyManager.Instance.CanAffordBuilding(type) && buildings[type] != null && GameManager.Instance.betweenWaves)
        {
            //Create a building that will track your cursor to show what it will look like when you place it
            selectedConstruction = Instantiate(buildings[type]);

            //Ensures that there is no functionality to the building tracker
            Destroy(selectedConstruction.GetComponentInChildren<PlayerBuilding>());
        }
        else if (type != 10 && type != -1)
        {
            Build(-1);
        }
    }

    //Clear out construction data
    private void standardConstructionEvents()
    {
        //If a building is already being previewed, destroy it
        if (selectedConstruction != null)
        {
            Destroy(selectedConstruction);
        }
        //Set the building to null in order to ensure that there are no accidental references
        selectedBuilding = null;
    }

    //Updates the upgrade windows
    private void standardUpgradeEvents()
    {
        //If no building is selected to upgrade, close the windows
        if (selectedBuilding == null)
        {
            GameManager.Instance.OpenWindow(-1, null);
        }
        else
        {
            bool buildingExists = false;

            //Identifies what type of building you selected
            Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
            if (turret != null)
            {
                GameManager.Instance.OpenWindow(0, turret);
                buildingExists = true;
            }
            else
            {
                RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
                if (repair != null)
                {
                    GameManager.Instance.OpenWindow(1, repair);
                    buildingExists = true;
                }
                else
                {
                    Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                    if (wall != null)
                    {
                        GameManager.Instance.OpenWindow(2, wall);
                        buildingExists = true;
                    }
                    else
                    {
                        ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                        if (extractor != null)
                        {
                            GameManager.Instance.OpenWindow(3, extractor);
                            buildingExists = true;
                        }
                    }
                }
            }
            if(buildingExists)
            {
                GameManager.Instance.OpenWindow(4, null);
            }
        }
    }


    //Upgrade a building
    public void UpgradeBuilding(int type, int upgrade)
    {
        //Ensures that selection is up to date
        selectedUpgrade = upgrade;
        GameManager.Instance.UpdateSelection(type, selectedBuilding);

        //Gets the building to upgrade
        IUpgradeable subject = selectedBuilding.GetComponentInChildren<IUpgradeable>();

        //Ensure that alignment is finished processing
        if (subject.IsAligned())
        {
            //Ensures tht you can afford the upgrade
            if (EconomyManager.Instance.budget >= subject.GetUpgradeCost(upgrade))
            {
                //Upgrade the building with the desired upgrade
                subject.Upgrade(upgrade);

                //Update the panel to show the new data
                GameManager.Instance.UpdateUpgradeCost(type, upgrade);
            }
        }
        //If it is not, then this is going to be where it gets aligned
        else
        {
            //Identify how many upgrades you will need to update after alignment
            int maxTypes = 0;
            switch (type)
            {
                case 0:
                    {
                        maxTypes = 5;
                        break;
                    }
                case 1:
                    {
                        maxTypes = 3;
                        break;
                    }
                case 2:
                    {
                        maxTypes = 2;
                        break;
                    }
                case 3:
                    {
                        maxTypes = 4;
                        break;
                    }
                default:
                    {
                        maxTypes = 0;
                        break;
                    }

            }
            //Align the building
            subject.Align(upgrade);

            //Update upgrades
            for (int i = 0; i < maxTypes; i++)
            {
                GameManager.Instance.UpdateUpgradeCost(type, i);
            }
        }
    }


    //Find if a spot is a valid place to put a given building
    private bool checkPlacement(Vector2Int location)
    {
        //Use a switch depending on type of building
        switch (selectedConstructionIndex)
        {
            //Turrets
            case 0:
            case 1:
            case 2:
                {
                    return TileManager.Instance.Check(location) >= TileManager.Instance.traversableCutoff && TileManager.Instance.CheckResource(location) < TileManager.Instance.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //Repair Stations
            case 3:
            case 4:
                {
                    return TileManager.Instance.CheckResource(location) < TileManager.Instance.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //Walls
            case 5:
            case 6:
                {
                    return TileManager.Instance.Check(location) >= TileManager.Instance.traversableCutoff && TileManager.Instance.CheckResource(location) < TileManager.Instance.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //Resource Extractors
            case 7:
            case 8:
            case 9:
                {
                    return TileManager.Instance.CheckResource(location) >= TileManager.Instance.resourceCutoff && !playerBuildings.ContainsKey(location);
                }
            //If none of these, it is not a valid building so you cannot place it
            default:
                return false;
        }
    }

    //Build a building
    private void placeBuilding(Vector2Int hoveredTile)
    {
        //Stores the created building
        GameObject go;

        //Creates the selected building
        switch (selectedConstructionIndex)
        {
            //Tier One Turret
            case 0:
                {
                    go = Instantiate(turretTierOne, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Turret
            case 1:
                {
                    go = Instantiate(turretTierTwo, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Three Turret
            case 2:
                {
                    go = Instantiate(turretTierThree, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier One Repair Station
            case 3:
                {
                    go = Instantiate(repairTierOne, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Repair Station
            case 4:
                {
                    go = Instantiate(repairTierTwo, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier One Wall
            case 5:
                {
                    go = Instantiate(wallTierOne, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Wall
            case 6:
                {
                    go = Instantiate(wallTierTwo, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier One Resource Extractor
            case 7:
                {
                    go = Instantiate(extractorTierOne, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Two Resource Extractor
            case 8:
                {
                    go = Instantiate(extractorTierTwo, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Tier Three Resource Extractor
            case 9:
                {
                    go = Instantiate(extractorTierThree, TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y)), Quaternion.identity);
                    break;
                }
            //Not a valid building, so just skip the rest
            default:
                return;
        }
        //Sets the building information
        PlayerBuilding pb = go.GetComponentInChildren<PlayerBuilding>();
        pb.location = hoveredTile;
        EconomyManager.Instance.PlaceBuilding(ref pb);

        //Add to the tracked building dictionary
        playerBuildings.Add(hoveredTile, go);
        MusicManager.Instance.PlayClick();
    }

    public void CancelConstruction()
    {
        //Ensure that all construction data is cleared
        selectedConstructionIndex = -1;
        Destroy(selectedConstruction);
        selectedConstruction = null;
    }

    public void CancelUpgrade()
    {
        //Clear all of the selected building data
        selectedBuilding = null;
        standardUpgradeEvents();
    }

    public void ChangeConstructionSelection(int direction)
    {
        //Increase selected index
        selectedConstructionIndex += direction;

        //Loop back around to no selection
        if (selectedConstructionIndex == 11)
        {
            selectedConstructionIndex = -1;
        }
        //Loop back around to new wave
        else if (selectedConstructionIndex == -2)
        {
            //Next Wave stuff here later
            selectedConstructionIndex = 10;
        }
        //Update selected construction data
        Build(selectedConstructionIndex);
    }

    public void ChangeUpgradeSelection(int direction)
    {
        //Sets the number of different upgrades based on the type of building
        int cap = 0;

        //Turret has 5 upgrades, subtract one due to zero based index
        Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
        if (turret != null)
        {
            cap = 5;
        }
        else
        {
            //Repair Station has 3 upgrade, subtract one due to zero based index
            RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
            if (repair != null)
            {
                cap = 3;
            }
            else
            {
                //Wall has 2 upgrades, subtract one due to zreo based index
                Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                if (wall != null)
                {
                    cap = 2;
                }
                else
                {
                    //Resource Extractor has 4 upgrades, subtract one due to zero based index
                    ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                    if (extractor != null)
                    {
                        cap = 4;
                    }
                }
            }
        }
        //Increase the index of the selected upgrade
        selectedUpgrade += direction;

        //Wrap around based on the cap
        if (selectedUpgrade == cap)
        {
            selectedUpgrade = 0;
        }
        else if (selectedUpgrade == -1)
        {
            selectedUpgrade = cap - 1;
        }
        //Update the shown data to reflect new selection
        GameManager.Instance.UpdateSelection(selectedUpgrade, selectedBuilding);
    }

    public void Upgrade()
    {
        //Identify building type that you are trying to upgrade
        Turret turret = selectedBuilding.GetComponentInChildren<Turret>();
        if (turret != null)
        {
            //Upgrade turret with selected upgrade
            UpgradeBuilding(0, selectedUpgrade);
        }
        else
        {
            RepairStation repair = selectedBuilding.GetComponentInChildren<RepairStation>();
            if (repair != null)
            {
                //upgrade repair station with selected upgrade
                UpgradeBuilding(1, selectedUpgrade);
            }
            else
            {
                Wall wall = selectedBuilding.GetComponentInChildren<Wall>();
                if (wall != null)
                {
                    //Upgrade wall with selected upgrade
                    UpgradeBuilding(2, selectedUpgrade);
                }
                else
                {
                    ResourceExtractor extractor = selectedBuilding.GetComponentInChildren<ResourceExtractor>();
                    if (extractor != null)
                    {
                        //Upgrade Extractor with selected upgrade
                        UpgradeBuilding(3, selectedUpgrade);
                    }
                }
            }
        }
    }

    public void SelectBuilding(GameObject building)
    {
        //Updates upgrade data
        selectedUpgrade = 0;
        selectedBuilding = building;
        standardUpgradeEvents();
        GameManager.Instance.UpdateSelection(selectedUpgrade, selectedBuilding);
    }

    public void AttemptPlacement(Vector2Int hoveredTile)
    {
        if (checkPlacement(hoveredTile))
        {
            placeBuilding(hoveredTile);
        }
    }

    public void TriggerFollow(Vector2Int hoveredTile)
    {
        //Makes the building preview follow the mouse if it exists
        if (selectedConstruction != null)
        {
            //Snaps the position to be centered on the tilemap's hexagonal grid
            selectedConstruction.transform.position = TileManager.Instance.TraversableTilemap.CellToWorld(new Vector3Int(hoveredTile.x, hoveredTile.y));
        }
    }


    //Function to remove a building
    public void Sell()
    {
        //Do not attempt to remove a building if you do not have a building selected
        if (selectedBuilding != null)
        {
            //Checks to see if you can remove the building
            PlayerBuilding building = selectedBuilding.GetComponentInChildren<PlayerBuilding>();
            if (building)
            {
                if (building.Sell())
                {
                    //Clears building selection
                    selectedBuilding = null;
                    standardUpgradeEvents();
                }
            }
        }
    }

    //Events for building removal
    public void RemoveBuilding(PlayerBuilding building)
    {
        //Ensures that the previous changed building does not lose its next link
        if (building.previousChanged != null)
        {
            building.previousChanged.nextChanged = building.nextChanged;
        }
        //Ensures that the next changed building does not lose its previous link
        if (building.nextChanged != null)
        {
            building.nextChanged.previousChanged = building.previousChanged;
        }
        //Updates energy data
        if (building.active)
        {
            //If it is an active building remove the energy cost from it
            EconomyManager.Instance.ChangeEnergyUsage(-building.energyCost);

            //Ensure that all economy data from extractors is cleared
            if (building.GetBuildingType() >= 7)
            {
                ResourceExtractor extractor = building.gameObject.GetComponentInChildren<ResourceExtractor>();
                EconomyManager.Instance.IncreaseIncome(-extractor.extractionRate);
                EconomyManager.Instance.ChangeEnergyCap(-extractor.energyRate);
            }
        }
        //Remove from building tracker
        playerBuildings.Remove(building.location);

        EconomyManager.Instance.RemoveBuilding(building);
    }

    public void GetSaveData(ref GameData data)
    {
        data.buildings = new BuildingData[playerBuildings.Count - 1];
        GameObject[] buildingArray = playerBuildings.Values.ToArray();
        //Buildings
        for (int i = 0; i < data.buildings.Length; i++)
        {
            data.buildings[i] = buildingArray[i + 1].GetComponentInChildren<PlayerBuilding>().GetAsData();
        }
    }

    public void Deactivate()
    {
        Destroy(playerBuildings[new Vector2Int(0, 0)]);

        //Clear data
        playerBuildings.Clear();
        playerHealths.Clear();
        playerExtractionData.Clear();
        playerDamageData.Clear();
        playerRepairData.Clear();
    }
}
