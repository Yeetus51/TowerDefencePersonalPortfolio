using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTypeBase : ScriptableObject
{
    public string towerName;
    public Sprite thumbnail;  
    public int level;
    public int cost;


    public float collisionRange;

    public List<Upgradable> upgradables = new List<Upgradable>();

    public Upgradable GetUpgradable(Upgradable.UpgradeType type)
    {
        foreach (var item in upgradables)
        {
            if (item.upgradeType == type) return item; 
        }
        return upgradables[0]; 
    }



}
