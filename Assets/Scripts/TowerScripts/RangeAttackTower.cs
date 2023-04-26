using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackTower : Tower
{
    float projectileSpeed;
    int projectileDamage; 
    [SerializeField] GameObject shootingPoint; 

    public override void Attack()
    {
        attacking = true;
        ShootProjectile(0);
    }

    protected Projectile ShootProjectile(int type)
    {
        Projectile shotArrow = ObjectPooler.Instance.GetPooledProjectile(type);
        shotArrow.damagePotential = projectileDamage;
        shotArrow.ogPostion = transform.position;
        shotArrow.transform.position = shootingPoint.transform.position;
        Vector3 targetPotion = lockedEnemy.transform.position;
        targetPotion += Vector3.up * lockedEnemy.collider.size.y/2; 
        shotArrow.transform.forward = (targetPotion - shootingPoint.transform.position).normalized;
        shotArrow.speed = projectileSpeed;
        shotArrow.gameObject.SetActive(true);
        return shotArrow;
    }

    public override void Upgrade(int upgradeType, float vlaue)
    {
        base.Upgrade(upgradeType, vlaue);
        if (upgradeType == (int)Upgradable.UpgradeType.projectileSpeed) projectileSpeed = vlaue;
        if (upgradeType == (int)Upgradable.UpgradeType.projectileDamage) projectileDamage = (int)vlaue;
    }

    public override void Attacking()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer < 1/attackSpeed) return;
        attacking = false;
        attackTimer = 0;
    }

    public override void SetTowerData()
    {
        base.SetTowerData();

        projectileSpeed = towerData.GetUpgradable(Upgradable.UpgradeType.projectileSpeed).upgradeLevels[0].value; 
        projectileDamage = (int)towerData.GetUpgradable(Upgradable.UpgradeType.projectileDamage).upgradeLevels[0].value;
    }


}
