using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowPath) , typeof(Collider))]
public class EnemyManager : Enemy
{
    [SerializeField] EnemyType EnemyData; 
    public FollowPath followPathScript;

    [HideInInspector] public BoxCollider collider;


    [SerializeField] HealthBar healthBar; 

    Color baseColor;

    bool stopMovement; 

    private void Awake()
    {
        baseColor = renderer.material.color;

        GameManager.Instance.OnGameRestart += StartGame;

        GameManager.Instance.OnPauseGame += StopMovement; 

        SetEnemyData();
        StartGame();
    }
    // Start is called before the first frame update
    void StartGame()
    {
        stopMovement = false; 
    }

    void SetEnemyData()
    {
        id = EnemyData.id;
        health = EnemyData.health;
        speed = EnemyData.speed;
        xpCarrying = EnemyData.xpCarrying;
        followPath = followPathScript;
        healthBar.maxValue = EnemyData.health;
        healthBar.ResetBar();
        isDead = false;
        stopMovement = false;
    }

    public void ResetEnemy()
    {
        SetEnemyData();
        followPathScript.Restart();
    }

    public void SetCheckingColor()
    {
        renderer.material.color = Color.green; 
    }
    public void SetTargetingColor()
    {
        renderer.material.color = Color.red;
    }
    private void Update()
    {
        if (stopMovement) return; 
        if (followPath.endReached) EnemyReachedBase();

        if (slowedTimer > slowedTime)
        {
            slowedTimer = 0;
            isSlowed = false;
            speed = EnemyData.speed;
        }

        if (isSlowed) SlowDownTimer();
        float deltaTime = Time.deltaTime;
        if(deltaTime < 0.1f)followPath.Translate(speed * deltaTime * 100);
    }


    public void ResetColor()
    {
        renderer.material.color = baseColor;
    }

    void StopMovement()
    {
        stopMovement = true;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameRestart -= StartGame;

        GameManager.Instance.OnPauseGame -= StopMovement;
    }
}
