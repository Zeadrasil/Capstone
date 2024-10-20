using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeable
{
    public void Upgrade(int type);
    public float GetUpgradeCost(int type);
    public string GetUpgradeEffects(int type);
    public string GetDescription();
}
