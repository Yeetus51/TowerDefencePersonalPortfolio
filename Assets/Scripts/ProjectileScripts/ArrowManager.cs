using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        foreach (var item in ObjectPooler.Instance.GetAllActiveEnemies())
        {
            if(item.Value == other.gameObject)
            {
                item.Key.TakeDamage(damagePotential);
                gameObject.SetActive(false);
                return;
            }
        }
    }
}
