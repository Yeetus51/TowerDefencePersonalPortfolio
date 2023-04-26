using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeTower : Tower
{
    [SerializeField] float blinkingDuration;
    [SerializeField] float whiteDuration;
    [SerializeField] float redDuration;

    [SerializeField] Renderer renderer;


    public override void SetTowerData()
    {
        base.SetTowerData();
        TowerTypeCreeper creeperTowerData = (TowerTypeCreeper)towerData;
        blinkingDuration = creeperTowerData.blinckingDuration;
        whiteDuration = creeperTowerData.whiteDuration;
        redDuration = creeperTowerData.redDuration;

    }
    public override void Attacking()
    {
        attackTimer += Time.deltaTime;


        float whiteColor = -Mathf.Cos(Mathf.Pow(attackTimer, 2) * 20) * 1 + 1; 

        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", new Color(whiteColor, whiteColor, whiteColor));
        if(attackTimer > blinkingDuration) renderer.material.SetColor("_EmissionColor", new Color(1, 1, 1));
        if (attackTimer > whiteDuration + blinkingDuration) renderer.material.SetColor("_EmissionColor", new Color(1, 0, 0));
        if (attackTimer > whiteDuration + blinkingDuration + redDuration) renderer.material.SetColor("_EmissionColor", new Color(0, 0, 0));



        if (attackTimer < 1/attackSpeed + blinkingDuration + whiteDuration + redDuration) return;
        DamageInRange();
        attacking = false;
        attackTimer = 0;
    }

    void DamageInRange()
    {
        List<EnemyManager> toDamage = new List<EnemyManager>(); 
        foreach (var item in ObjectPooler.Instance.GetAllActiveEnemies())
        {
            if (IsEnemyInRange(item.Key))
            {
                toDamage.Add(item.Key);
            }
        }
        foreach (var item in toDamage)
        {
            DealDamage(item, strength);
        }
    }
}
