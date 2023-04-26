using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubWaveSO
{
    public float startTime;
    [System.Serializable]
    public enum EnemyID
    {
        chicken = 0,
        pig = 1,
        sheep = 2,
        cow = 3,
        wolf = 4,
        villiger = 5,
        ironGolem = 6
    };
    [SerializeField]
    public EnemyID enemyId = EnemyID.chicken;
    public float frequency;
    public float endTime;

}
