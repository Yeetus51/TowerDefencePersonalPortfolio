using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAttackTower : Tower
{
    float attackPeriod;
    float attackAnimationIntensity;


    bool givenDamage;

    public override void SetTowerData()
    {
        base.SetTowerData();
        TowerTypeZombie temp = (TowerTypeZombie)towerData;
        attackPeriod = temp.attackPeriod;
        attackAnimationIntensity = temp.AnimationIntensity;
    }

    public override void Attacking()
    {
        if (attackTimer > attackPeriod)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer - attackPeriod < 1/attackSpeed) return;
            attacking = false;
            givenDamage = false;
            attackTimer = 0;
            return;
        }
        float i = (1 / attackPeriod) * Mathf.PI;
        float angle = Mathf.Sin(attackTimer * i) * attackAnimationIntensity;
        bool halfPoint = false;
        if (attackTimer > attackPeriod / 2)
        {
            halfPoint = true;
            if (!givenDamage) DealDamage(lockedEnemy, strength);
            givenDamage = true;
        }
        mesh.transform.Rotate(Vector3.right, halfPoint ? -angle : angle);

        attackTimer += Time.deltaTime;
    }
}
