using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "EnemyType")]
public class EnemyType : ScriptableObject
{
    public int id;
    public int health;
    public float speed;
    public int xpCarrying;

}
