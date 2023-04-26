using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public FollowPath followPath;


    public delegate void DamageTakenHandler(int damage);
    public event DamageTakenHandler OnDamageTaken;

    [SerializeField] protected Renderer renderer;

    bool takingDamage;
    float damageTakingTimer = 0; 
    float damangeColorPeriod = 0.1f;

    [HideInInspector] public int id { get; protected set; }
    [HideInInspector] public int health;
    [HideInInspector] public float speed;
    [HideInInspector] public int xpCarrying;

    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isSlowed;
    [HideInInspector] public float slowedTime;
    [HideInInspector] public float slowedTimer = 0;

    public void TakeDamage(int damageTaken)
    {
        takingDamage = true;
        renderer.material.color = Color.red;

        health -= damageTaken;
        OnDamageTaken.Invoke(health);
        if (health <= 0) EnemyDeath();
    }

    void EnemyDeath()
    {
        isDead = true;
        renderer.material.color = Color.white;
        ObjectPooler.Instance.RemoveFromActiveEnemies((EnemyManager)this);
        GameManager.Instance.AddMoney(xpCarrying);
        UiManager.Instance.SpawnTextOnScreen(this.transform.position, xpCarrying);
        if (UiUpgradeShopManager.Instance.GetShopActive()) UiUpgradeShopManager.Instance.CheckPayableUpgrades();
    }
    public void FixedUpdate()
    {
        if (takingDamage) DamageTaking();
    }

    protected void SlowDownTimer()
    {
        slowedTimer += Time.deltaTime; 
    }


    protected void EnemyReachedBase()
    {
        ObjectPooler.Instance.RemoveFromActiveEnemies((EnemyManager)this);
    }

    void DamageTaking()
    {
        damageTakingTimer += Time.deltaTime;
        if (damageTakingTimer < damangeColorPeriod) return;
        damageTakingTimer = 0;
        takingDamage = false;
        renderer.material.color = Color.white;
    }
}
