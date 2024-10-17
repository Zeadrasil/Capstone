using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerValueHolder : MonoBehaviour
{
    [SerializeField] TMP_Text nextWaveLabel;
    [SerializeField] GameObject turretTierOne;
    [SerializeField] GameObject extractorTierOne;
    [SerializeField] GameObject baseEnemy;

    [SerializeField] Image nextWaveBackground;
    [SerializeField] Image tierOneTurretBackground;
    [SerializeField] Image tierOneTurretFrame;
    [SerializeField] Image tierOneExtractorBackground;
    [SerializeField] Image tierOneExtractorFrame;

    [SerializeField] TMP_Text budgetText;
    [SerializeField] TMP_Text incomeText;

    [SerializeField] Color unselectedColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Vector3 unavailableColor;
    [SerializeField] Vector3 availableColor;

    [SerializeField] TMP_Text tierOneTurretLabel;
    [SerializeField] TMP_Text tierOneExtractorLabel;
    [SerializeField] Camera camera;
    [SerializeField] GameObject enemyCheckpointPrefab;
    [SerializeField] PlayerBase playerBase;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.nextWaveLabel = nextWaveLabel;
        GameManager.Instance.turretTierOne = turretTierOne;
        GameManager.Instance.extractorTierOne = extractorTierOne;
        GameManager.Instance.baseEnemy = baseEnemy;
        GameManager.Instance.nextWaveBackground = nextWaveBackground;
        GameManager.Instance.tierOneTurretBackground = tierOneTurretBackground;
        GameManager.Instance.tierOneTurretFrame = tierOneTurretFrame;
        GameManager.Instance.tierOneExtractorBackground = tierOneExtractorBackground;
        GameManager.Instance.tierOneExtractorFrame = tierOneExtractorFrame;
        GameManager.Instance.budgetText = budgetText;
        GameManager.Instance.incomeText = incomeText;
        GameManager.Instance.unselectedColor = unselectedColor;
        GameManager.Instance.selectedColor = selectedColor;
        GameManager.Instance.unavailableColor = unavailableColor;
        GameManager.Instance.availableColor = availableColor;
        GameManager.Instance.tierOneTurretLabel = tierOneTurretLabel;
        GameManager.Instance.tierOneExtractorLabel = tierOneExtractorLabel;
        GameManager.Instance.Camera = camera;
        GameManager.Instance.EnemyCheckpointPrefab = enemyCheckpointPrefab;
        GameManager.Instance.PlayerBase = playerBase;
    }

}
