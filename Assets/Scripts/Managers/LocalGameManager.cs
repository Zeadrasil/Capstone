using TMPro;
using UnityEngine;
using UnityEngine.UI;

//LocalGameManager allows data to be stored in a scene and then passed into the GameManager
//It also is used as a relay for buttons.
public class LocalGameManager : MonoBehaviour
{
    [SerializeField] TMP_Text nextWaveLabel;

    //Prefabs
    [SerializeField] GameObject turretTierOne;
    [SerializeField] GameObject repairTierOne;
    [SerializeField] GameObject wallTierOne;
    [SerializeField] GameObject extractorTierOne;
    [SerializeField] GameObject baseEnemy;
    [SerializeField] GameObject enemyCheckpointPrefab;

    //Construction panel data
    [SerializeField] Image nextWaveBackground;
    [SerializeField] Image nextWaveFrame;
    [SerializeField] Image tierOneTurretBackground;
    [SerializeField] Image tierOneTurretFrame;
    [SerializeField] Image tierTwoTurretBackground;
    [SerializeField] Image tierTwoTurretFrame;
    [SerializeField] Image tierThreeTurretBackground;
    [SerializeField] Image tierThreeTurretFrame;
    [SerializeField] Image tierOneRepairBackground;
    [SerializeField] Image tierOneRepairFrame;
    [SerializeField] Image tierTwoRepairBackground;
    [SerializeField] Image tierTwoRepairFrame;
    [SerializeField] Image tierOneWallBackground;
    [SerializeField] Image tierOneWallFrame;
    [SerializeField] Image tierTwoWallBackground;
    [SerializeField] Image tierTwoWallFrame;
    [SerializeField] Image tierOneExtractorBackground;
    [SerializeField] Image tierOneExtractorFrame;
    [SerializeField] Image tierTwoExtractorBackground;
    [SerializeField] Image tierTwoExtractorFrame;
    [SerializeField] Image tierThreeExtractorBackground;
    [SerializeField] Image tierThreeExtractorFrame;

    //Economy data
    [SerializeField] TMP_Text budgetText;
    [SerializeField] TMP_Text incomeText;

    //Color data
    [SerializeField] Color unselectedColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Vector3 unavailableColor;
    [SerializeField] Vector3 availableColor;

    //Hotkey labels
    [SerializeField] TMP_Text tierOneTurretLabel;
    [SerializeField] TMP_Text tierTwoTurretLabel;
    [SerializeField] TMP_Text tierThreeTurretLabel;
    [SerializeField] TMP_Text tierOneRepairLabel;
    [SerializeField] TMP_Text tierTwoRepairLabel;
    [SerializeField] TMP_Text tierOneWallLabel;
    [SerializeField] TMP_Text tierTwoWallLabel;
    [SerializeField] TMP_Text tierOneExtractorLabel;
    [SerializeField] TMP_Text tierTwoExtractorLabel;
    [SerializeField] TMP_Text tierThreeExtractorLabel;

    //Other data
    [SerializeField] Camera camera;
    [SerializeField] PlayerBase playerBase;

    //Turret upgrade data
    [SerializeField] Canvas TurretUpgradeWindow;

    [SerializeField] TMP_Text turretSplashUpgradeText;
    [SerializeField] Image turretSplashFrame;
    [SerializeField] Image turretSplashBackground;

    [SerializeField] TMP_Text turretRangeUpgradeText;
    [SerializeField] Image turretRangeFrame;
    [SerializeField] Image turretRangeBackground;

    [SerializeField] TMP_Text turretDamageUpgradeText;
    [SerializeField] Image turretDamageFrame;
    [SerializeField] Image turretDamageBackground;

    [SerializeField] TMP_Text turretFirerateUpgradeText;
    [SerializeField] Image turretFirerateFrame;
    [SerializeField] Image turretFirerateBackground;

    [SerializeField] TMP_Text turretHealthUpgradeText;
    [SerializeField] Image turretHealthFrame;
    [SerializeField] Image turretHealthBackground;

    [SerializeField] TMP_Text turretDescriptionText;

    //Repair station upgrade data
    [SerializeField] Canvas RepairUpgradeWindow;

    [SerializeField] TMP_Text repairRangeUpgradeText;
    [SerializeField] Image repairRangeFrame;
    [SerializeField] Image repairRangeBackground;

    [SerializeField] TMP_Text repairHealingUpgradeText;
    [SerializeField] Image repairHealingFrame;
    [SerializeField] Image repairHealingBackground;

    [SerializeField] TMP_Text repairHealthUpgradeText;
    [SerializeField] Image repairHealthFrame;
    [SerializeField] Image repairHealthBackground;

    [SerializeField] TMP_Text repairDescriptionText;

    //Wall upgrade data
    [SerializeField] Canvas WallUpgradeWindow;

    [SerializeField] TMP_Text wallHealthUpgradeText;
    [SerializeField] Image wallHealthFrame;
    [SerializeField] Image wallHealthBackground;

    [SerializeField] TMP_Text wallHealingUpgradeText;
    [SerializeField] Image wallHealingFrame;
    [SerializeField] Image wallHealingBackground;

    [SerializeField] TMP_Text wallDescriptionText;

    // Passes data to the GameManager instance
    void Start()
    {
        GameManager.Instance.nextWaveLabel = nextWaveLabel;

        //Pass prefabs
        GameManager.Instance.turretTierOne = turretTierOne;
        GameManager.Instance.repairTierOne = repairTierOne;
        GameManager.Instance.wallTierOne = wallTierOne;
        GameManager.Instance.extractorTierOne = extractorTierOne;
        GameManager.Instance.baseEnemy = baseEnemy;
        GameManager.Instance.EnemyCheckpointPrefab = enemyCheckpointPrefab;

        //Pass construction panel data
        GameManager.Instance.nextWaveBackground = nextWaveBackground;
        GameManager.Instance.nextWaveFrame = nextWaveFrame;
        GameManager.Instance.tierOneTurretBackground = tierOneTurretBackground;
        GameManager.Instance.tierOneTurretFrame = tierOneTurretFrame;
        GameManager.Instance.tierTwoTurretBackground = tierTwoTurretBackground;
        GameManager.Instance.tierTwoTurretFrame = tierTwoTurretFrame;
        GameManager.Instance.tierThreeTurretBackground = tierThreeTurretBackground;
        GameManager.Instance.tierThreeTurretFrame = tierThreeTurretFrame;
        GameManager.Instance.tierOneRepairBackground = tierOneRepairBackground;
        GameManager.Instance.tierOneRepairFrame = tierOneRepairFrame;
        GameManager.Instance.tierTwoRepairBackground = tierTwoRepairBackground;
        GameManager.Instance.tierTwoRepairFrame = tierTwoRepairFrame;
        GameManager.Instance.tierOneWallBackground = tierOneWallBackground;
        GameManager.Instance.tierOneWallFrame = tierOneWallFrame;
        GameManager.Instance.tierTwoWallBackground = tierTwoWallBackground;
        GameManager.Instance.tierTwoWallFrame = tierTwoWallFrame;
        GameManager.Instance.tierOneExtractorBackground = tierOneExtractorBackground;
        GameManager.Instance.tierOneExtractorFrame = tierOneExtractorFrame;
        GameManager.Instance.tierTwoExtractorBackground = tierTwoExtractorBackground;
        GameManager.Instance.tierTwoExtractorFrame = tierTwoExtractorFrame;
        GameManager.Instance.tierThreeExtractorBackground = tierThreeExtractorBackground;
        GameManager.Instance.tierThreeExtractorFrame = tierThreeExtractorFrame;

        //Pass economy data
        GameManager.Instance.budgetText = budgetText;
        GameManager.Instance.incomeText = incomeText;

        //Pass color data
        GameManager.Instance.unselectedColor = unselectedColor;
        GameManager.Instance.selectedColor = selectedColor;
        GameManager.Instance.unavailableColor = unavailableColor;
        GameManager.Instance.availableColor = availableColor;

        //Pass hotkey labels
        GameManager.Instance.tierOneTurretLabel = tierOneTurretLabel;
        GameManager.Instance.tierTwoTurretLabel = tierTwoTurretLabel;
        GameManager.Instance.tierThreeTurretLabel = tierThreeTurretLabel;
        GameManager.Instance.tierOneRepairLabel = tierOneRepairLabel;
        GameManager.Instance.tierTwoRepairLabel = tierTwoRepairLabel;
        GameManager.Instance.tierOneWallLabel = tierOneWallLabel;
        GameManager.Instance.tierTwoWallLabel = tierTwoWallLabel;
        GameManager.Instance.tierOneExtractorLabel = tierOneExtractorLabel;
        GameManager.Instance.tierTwoExtractorLabel = tierTwoExtractorLabel;
        GameManager.Instance.tierThreeExtractorLabel = tierThreeExtractorLabel;

        //Pass other data
        GameManager.Instance.Camera = camera;
        GameManager.Instance.PlayerBase = playerBase;

        //Pass turret upgrade data
        GameManager.Instance.TurretUpgradeWindow = TurretUpgradeWindow;
        GameManager.Instance.turretSplashUpgradeText = turretSplashUpgradeText;
        GameManager.Instance.turretSplashFrame = turretSplashFrame;
        GameManager.Instance.turretSplashBackground = turretSplashBackground;
        GameManager.Instance.turretRangeUpgradeText = turretRangeUpgradeText;
        GameManager.Instance.turretRangeFrame = turretRangeFrame;
        GameManager.Instance.turretRangeBackground = turretRangeBackground;
        GameManager.Instance.turretDamageUpgradeText = turretDamageUpgradeText;
        GameManager.Instance.turretDamageFrame = turretDamageFrame;
        GameManager.Instance.turretDamageBackground = turretDamageBackground;
        GameManager.Instance.turretFirerateUpgradeText = turretFirerateUpgradeText;
        GameManager.Instance.turretFirerateFrame = turretFirerateFrame;
        GameManager.Instance.turretFirerateBackground = turretFirerateBackground;
        GameManager.Instance.turretHealthUpgradeText = turretHealthUpgradeText;
        GameManager.Instance.turretHealthFrame = turretHealthFrame;
        GameManager.Instance.turretHealthBackground = turretHealthBackground;
        GameManager.Instance.turretDescriptionText = turretDescriptionText;

        //Pass repair station upgrade data
        GameManager.Instance.RepairUpgradeWindow = RepairUpgradeWindow;
        GameManager.Instance.repairRangeUpgradeText = repairRangeUpgradeText;
        GameManager.Instance.repairRangeFrame = repairRangeFrame;
        GameManager.Instance.repairRangeBackground = repairRangeBackground;
        GameManager.Instance.repairHealingUpgradeText = repairHealingUpgradeText;
        GameManager.Instance.repairHealingFrame = repairHealingFrame;
        GameManager.Instance.repairHealingBackground = repairHealingBackground;
        GameManager.Instance.repairHealthUpgradeText = repairHealthUpgradeText;
        GameManager.Instance.repairHealthFrame = repairHealthFrame;
        GameManager.Instance.repairHealthBackground = repairHealthBackground;
        GameManager.Instance.repairDescriptionText = repairDescriptionText;

        //Pass wall upgrade data
        GameManager.Instance.WallUpgradeWindow = WallUpgradeWindow;
        GameManager.Instance.wallHealthUpgradeText = wallHealthUpgradeText;
        GameManager.Instance.wallHealthFrame = wallHealthFrame;
        GameManager.Instance.wallHealthBackground = wallHealthBackground;
        GameManager.Instance.wallHealingUpgradeText = wallHealingUpgradeText;
        GameManager.Instance.wallHealingFrame = wallHealingFrame;
        GameManager.Instance.wallHealingBackground = wallHealingBackground;
        GameManager.Instance.wallDescriptionText = wallDescriptionText;
    }
    
    //Relay for building selection using buttons
    public void Build(int building)
    {
        GameManager.Instance.Build(building);
    }
    //Relay for turret upgrades using buttons
    public void UpgradeTurret(int upgrade)
    {
        GameManager.Instance.UpgradeTurret(upgrade);
    }
    //Relay for repair station upgrades using buttons
    public void UpgradeRepairStation(int upgrade)
    {
        GameManager.Instance.UpgradeRepairStation(upgrade);
    }
    //Relay for wall upgrades using buttons
    public void UpgradeWall(int upgrade)
    {
        GameManager.Instance.UpgradeWall(upgrade);
    }
    //Relay for spawning a new wave using buttons
    public void NextWave()
    {
        GameManager.Instance.NextWave();
    }

    public void Sell()
    {
        GameManager.Instance.Sell();
    }
}
