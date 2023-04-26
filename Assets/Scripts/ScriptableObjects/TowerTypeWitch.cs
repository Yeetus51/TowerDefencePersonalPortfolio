using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Witch", menuName = "TowerType/Witch", order = 2)]
public class TowerTypeWitch : TowerTypeBase
{
    public float splashRange;
    public float slownessEffectMultiplier;
    public float slownessEffectDuration;
    [HideInInspector]public float projectileSpeed;

}
