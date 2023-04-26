using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgradable
{
/*    public void Reset()
    {
        costIndex = 0; 
    }*/

    [SerializeField]
    public enum UpgradeType
    {
        strength = 0,
        range = 1,
        attackSpeed = 2,
        projectileDamage = 3,
        effectDuration = 4,
        effectRange = 5,
        projectileSpeed = 6
    };

    public UpgradeType upgradeType = UpgradeType.strength;
    public string upgradeName; 
    public List<UpgradeCostPair> upgradeLevels = new List<UpgradeCostPair>(); 

    public UpgradeCostPair GetData(Tower tower,int offset = 0)
    {
        return upgradeLevels[tower.costIndexes[this] + offset]; 
    }
    public UpgradeCostPair GetLastDataPoint()
    {
        return upgradeLevels[upgradeLevels.Count-1];
    }

}
[System.Serializable]
public class UpgradeCostPair
{
    public float value;
    public int cost;
    public int downgradePay;
}

