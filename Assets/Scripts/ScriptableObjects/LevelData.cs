using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Levels/Level")]
public class LevelData : ScriptableObject
{
    public int startingMoney;
    public int startingLives; 
}