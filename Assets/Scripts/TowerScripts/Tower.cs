using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] public TowerTypeBase towerData;

    //[SerializeField] public List<Upgradable> upgradables = new List<Upgradable>();

    float range;
    float collisionRange;
    [SerializeField] GameObject rangeVisual;
    [SerializeField] GameObject collisionVisual;

    public int strength { get; protected set; }


    //Dictionary<EnemyScriptManager, GameObject> checkingEnemies = new Dictionary<EnemyScriptManager, GameObject>();

    protected EnemyManager lockedEnemy = null;

    [SerializeField] protected GameObject mesh;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TowerHover towerHover;

    public Dictionary<Upgradable, int> costIndexes = new Dictionary<Upgradable, int>();

    protected float attackSpeed = 0;
    protected float attackTimer = 0;


    protected bool attacking;

    [HideInInspector] public bool ignoreOnClick = true;

    bool stopAttaking; 

    private void Awake()
    {
        towerHover.OnMouseOverChanged += ChangeHoverState;
        towerHover.OnMouseOverClicked += InvokeUpgradeShop;
        GameManager.Instance.OnGameRestart += StartGame;
        GameManager.Instance.OnPauseGame += StopAttacking;
        GameManager.Instance.OnPauseGame += SetIgnoreOnClick;
        SetTowerData();
    }
    private void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        costIndexes.Clear();
        stopAttaking = false;
        towerHover.enabled = true;
        SetCostInxes();
    }

    void SetCostInxes()
    {
        foreach (var upgrade in towerData.upgradables)
        {
            costIndexes.Add(upgrade, 0);
        }
    }
    public IEnumerator ExtraWaitForReclickable()
    {
        yield return new WaitForSeconds(0.1f);
        ignoreOnClick = false;
    }

    public virtual void SetTowerData() // USE THIS ONLY ONCE WHEN SPAWNING 
    {
        if (towerData.upgradables.Count < 1) return;

        strength = (int)towerData.GetUpgradable(Upgradable.UpgradeType.strength).upgradeLevels[0].value;
        range = towerData.GetUpgradable(Upgradable.UpgradeType.range).upgradeLevels[0].value;
        attackSpeed = towerData.GetUpgradable(Upgradable.UpgradeType.attackSpeed).upgradeLevels[0].value;
        collisionRange = towerData.collisionRange;

        rangeVisual.transform.localScale = new Vector3(range * 2, 0.1f, range * 2);
        stopAttaking = false;
  
    }

    void InvokeUpgradeShop()
    {
        if (!ignoreOnClick) UiUpgradeShopManager.Instance.Initiate(this);
    }
    void SetIgnoreOnClick()
    {
        ignoreOnClick = true; 
    }


    void ChangeHoverState(bool state)
    {
        if (state) meshRenderer.material.color = Color.gray;
        else 
        {
            meshRenderer.material.color = Color.white;
            ignoreOnClick = false;
        }

    }

    public virtual void Upgrade(int upgradeType, float vlaue)
    {
        if (upgradeType == 0) strength = (int)vlaue;
        if (upgradeType == 1)
        {
            range = vlaue;
            rangeVisual.transform.localScale = new Vector3(range * 2, 0.1f, range * 2);
        }
        if (upgradeType == 2) attackSpeed = vlaue;
    }



    public virtual void Attack() { attacking = true; }

    protected void DealDamage(EnemyManager enemy, int damageDealt)
    {
        enemy.TakeDamage(damageDealt);
    }


    public virtual void Attacking() { }



    private void FixedUpdate()
    {
        if (stopAttaking) return; 
        if (attacking) Attacking(); 
    }

    private void Update()
    {
        if (stopAttaking) return;
        if (!attacking)SettingLockedEnemy();
        if (lockedEnemy && !attacking) Attack();  
        if (lockedEnemy) Aim(lockedEnemy);

    }

    void Aim(EnemyManager target)
    {
        transform.forward = target.transform.position - transform.position;
    }

    void SettingLockedEnemy()
    {
        EnemyManager closestEnemy = GetClosestEnemy();
        if (closestEnemy != lockedEnemy)
        {
            //if (lockedEnemy) lockedEnemy.SetCheckingColor();
            lockedEnemy = closestEnemy;
            //if (lockedEnemy) lockedEnemy.SetTargetingColor();
        }
    }

     protected bool IsEnemyInRange(EnemyManager enemy)
    {
        float dist = Vector3.Distance(enemy.gameObject.transform.position, transform.position);
        if (dist <= range) return true;
        return false;
    }

    EnemyManager GetClosestEnemy()
    {
        Dictionary<EnemyManager, GameObject> checkingEnemies = ObjectPooler.Instance.GetAllActiveEnemies();
        if (checkingEnemies.Count < 1) return null;
        float closest = float.MaxValue;
        EnemyManager closestEnemy = null;
        foreach (var item in checkingEnemies)
        {
            if (!item.Value.activeInHierarchy || item.Key.isDead)
            {
                continue;
            }
                float dist = Vector3.Distance(item.Value.transform.position, transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestEnemy = item.Key;
            }
        }
        if (!closestEnemy) return null;
        if (!IsEnemyInRange(closestEnemy))
        {
            return null;
        }
        return closestEnemy;
    }

    void StopAttacking()
    {
        stopAttaking = true;
    }

    private void OnDestroy()
    {
        towerHover.OnMouseOverChanged -= ChangeHoverState;
        towerHover.OnMouseOverClicked -= InvokeUpgradeShop;
    }
}
