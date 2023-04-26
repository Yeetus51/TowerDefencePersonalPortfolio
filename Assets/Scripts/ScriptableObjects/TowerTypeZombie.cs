using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Zombie", menuName = "TowerType/Zombie", order = 0)]
public class TowerTypeZombie : TowerTypeBase
{
    public float attackPeriod; // NEEDS TO BE AN ODD decemal !!! otherwise bad ! use 0.1 , 0.3 or 0.5
    public float AnimationIntensity; 
}
