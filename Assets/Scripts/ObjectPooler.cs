using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [SerializeField] WayPointPath toFollowPath; 
    [SerializeField] List<EnemyManager> enemyPrefabs;
    [SerializeField] List<Projectile> projectilesPrefabs;
    [SerializeField] List<Tower> towersPrefabs;
    [SerializeField] UiMoneyDroppedElement uiMoneyDroppedPrefab; 



    public enum EnemyID {
        chicken = 0,
        pig = 1,
        sheep = 2,
        cow = 3, 
        wolf = 4, 
        villiger = 5, 
        ironGolem = 6
    };

    List<Dictionary<EnemyManager, GameObject>> enemyPools = new List<Dictionary<EnemyManager, GameObject>>();

    Dictionary<EnemyManager, GameObject> activeEnemies = new Dictionary<EnemyManager, GameObject>();

    Dictionary<EnemyManager, GameObject> chickenPool = new Dictionary<EnemyManager, GameObject>();
    Dictionary<EnemyManager, GameObject> pigPool = new Dictionary<EnemyManager, GameObject>();
    Dictionary<EnemyManager, GameObject> sheepPool = new Dictionary<EnemyManager, GameObject>();
    Dictionary<EnemyManager, GameObject> cowPool = new Dictionary<EnemyManager, GameObject>();
    Dictionary<EnemyManager, GameObject> wolfPool = new Dictionary<EnemyManager, GameObject>();
    Dictionary<EnemyManager, GameObject> villigerPool = new Dictionary<EnemyManager, GameObject>();
    Dictionary<EnemyManager, GameObject> ironGolemPool = new Dictionary<EnemyManager, GameObject>();

    List<Dictionary<Projectile, GameObject>> projectilePool = new List<Dictionary<Projectile, GameObject>>(); 
    Dictionary<Projectile, GameObject> arrowPool = new Dictionary<Projectile, GameObject>();
    Dictionary<Projectile, GameObject> potionPool = new Dictionary<Projectile, GameObject>();

    List<Dictionary<Tower, GameObject>> towerPools = new List<Dictionary<Tower, GameObject>>();

    Dictionary<Tower, GameObject> zombieTowerPool = new Dictionary<Tower, GameObject>();
    Dictionary<Tower, GameObject> skeletonTowerPool = new Dictionary<Tower, GameObject>();
    Dictionary<Tower, GameObject> witchTowerPool = new Dictionary<Tower, GameObject>();
    Dictionary<Tower, GameObject> creeperTowerPool = new Dictionary<Tower, GameObject>();

    Dictionary<UiMoneyDroppedElement, GameObject> droppedMoneyPool = new Dictionary<UiMoneyDroppedElement, GameObject>(); 


    public static ObjectPooler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        GameManager.Instance.OnGameRestart += DeactivateAllActiveObjects; 
    }

    private void Start()
    {
        AddPools();
        InitializePools(); 
    }
    void AddPools()
    {
        enemyPools.Add(chickenPool);
        enemyPools.Add(pigPool);
        enemyPools.Add(sheepPool);
        enemyPools.Add(cowPool);
        enemyPools.Add(wolfPool);
        enemyPools.Add(villigerPool);
        enemyPools.Add(ironGolemPool);

        projectilePool.Add(arrowPool);
        projectilePool.Add(potionPool);

        towerPools.Add(zombieTowerPool);
        towerPools.Add(skeletonTowerPool);
        towerPools.Add(witchTowerPool);
        towerPools.Add(creeperTowerPool);
    }

    void InitializePools()
    {
        for (int i = 0; i < enemyPools.Count; i++)
        {
            FindDependencies(enemyPrefabs[i]);
            for (int j = 0; j < 20; j++)
            {
                InstansiateNewEnemy(i);
            }
        }
        for (int i = 0; i < projectilesPrefabs.Count; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                InstansiateNewProjectile(i);
            }
        }

        for (int i = 0; i < towerPools.Count; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                InstansiateNewTower(i);
            }
        }

        for (int j = 0; j < 20; j++)
        {
            InstansiateNewMoneyDropped();
        }

    }



    void FindDependencies(EnemyManager item)
    {
        EnemyManager enemyScriptManager = item.GetComponent<EnemyManager>();
        FollowPath followScript = enemyScriptManager.followPathScript = item.GetComponent<FollowPath>();
        followScript.designatedPath = toFollowPath;
        enemyScriptManager.collider = item.GetComponent<BoxCollider>();
    }

    public Dictionary<EnemyManager, GameObject> GetAllActiveEnemies()
    {
        return activeEnemies;
    }

    public void RemoveFromActiveEnemies(EnemyManager item)
    {
        activeEnemies.Remove(item);
        item.gameObject.SetActive(false);
    }
    private void AddToActiveEnemies(EnemyManager item)
    {
        activeEnemies.Add(item, item.gameObject);
    }


    public EnemyManager GetPooledEnemy(int type) // USE THIS WHEN SPAWING // DO NOT USE THIS IF YOU DONT INTEND TO ACTIVATE THE ENEMY
    {
        foreach (var enemy in enemyPools[type])
        {
            if (!enemy.Value.activeInHierarchy)
            {
                AddToActiveEnemies(enemy.Key); 
                return enemy.Key;
            }
        }
        EnemyManager newEnemy =  InstansiateNewEnemy(type);
        AddToActiveEnemies(newEnemy);
        return newEnemy; 
    }


    public Projectile GetPooledProjectile(int type)
    {
        foreach (var arrow in projectilePool[type])
        {
            if (!arrow.Value.activeInHierarchy) return arrow.Key;
        }
        Projectile newArrow = InstansiateNewProjectile(type);
        return newArrow;
    }
    public Projectile GetPooledArrow() 
    {
        foreach (var arrow in arrowPool)
        {
            if (!arrow.Value.activeInHierarchy) return arrow.Key;
        }
        Projectile newArrow = InstansiateNewArrow();
        return newArrow;
    }


    public Projectile GetPooledPotion()
    {
        foreach (var arrow in arrowPool)
        {
            if (!arrow.Value.activeInHierarchy) return arrow.Key;
        }
        Projectile newArrow = InstansiateNewPotion();
        return newArrow;
    }

    public Tower GetPooledTower(int type)
    {
        foreach (var tower in towerPools[type])
        {
            if (!tower.Value.activeInHierarchy) return tower.Key;
        }
        Tower newArrow = InstansiateNewTower(type);
        return newArrow;
    }
    public UiMoneyDroppedElement GetPooledUiMoneyDropped()
    {
        foreach (var moneyDropped in droppedMoneyPool)
        {
            if (!moneyDropped.Value.activeInHierarchy) return moneyDropped.Key;
        }
        UiMoneyDroppedElement newMoneyDropped = InstansiateNewMoneyDropped();
        return newMoneyDropped;
    }

    public Dictionary<Tower,GameObject> GetAllActivePooledTowers()
    {
        Dictionary<Tower, GameObject> allTowers = new Dictionary<Tower, GameObject>();
        foreach (var pool in towerPools)
        {
            foreach (var tower in pool)
            {
                if(tower.Value.activeInHierarchy) allTowers.Add(tower.Key, tower.Value);
            }
        }
        return allTowers; 
    }

    public Dictionary<EnemyManager, GameObject> GetSpecificPooledEnemies(int type)
    {
        return enemyPools[type];
    }

    public Dictionary<EnemyManager, GameObject> GetAllPooledEnemies()
    {
        Dictionary<EnemyManager, GameObject> allEnemies = new Dictionary<EnemyManager, GameObject>();
        foreach (var pool in enemyPools)
        {
            foreach (var Item in pool)
            {
                if (Item.Value.activeInHierarchy)
                {
                    allEnemies.Add(Item.Key, Item.Value);
                }   
            }
        }
        return allEnemies;
    }

    Projectile InstansiateNewProjectile(int type)
    {
        Projectile newProjectile = Instantiate(projectilesPrefabs[type]);
        newProjectile.gameObject.SetActive(false);
        projectilePool[type].Add(newProjectile, newProjectile.gameObject);
        return newProjectile;
    }
    Projectile InstansiateNewArrow()
    {
        Projectile newArrow = Instantiate(projectilesPrefabs[0]);
        newArrow.gameObject.SetActive(false);
        arrowPool.Add(newArrow, newArrow.gameObject);
        return newArrow;
    }
    Projectile InstansiateNewPotion()
    {
        Projectile newPotion = Instantiate(projectilesPrefabs[1]);
        newPotion.gameObject.SetActive(false);
        potionPool.Add(newPotion, newPotion.gameObject);
        return newPotion;
    }

    EnemyManager InstansiateNewEnemy(int type) 
    {
        EnemyManager newEnemy = Instantiate(enemyPrefabs[type]);
        newEnemy.gameObject.SetActive(false);
        enemyPools[type].Add(newEnemy, newEnemy.gameObject);

        return newEnemy;
    }

    Tower InstansiateNewTower(int type)
    {
        Tower newTower = Instantiate(towersPrefabs[type]);
        newTower.gameObject.SetActive(false);
        towerPools[type].Add(newTower, newTower.gameObject);

        return newTower;
    }

    private UiMoneyDroppedElement InstansiateNewMoneyDropped()
    {
        UiMoneyDroppedElement newMoneyDropped = Instantiate(uiMoneyDroppedPrefab);
        newMoneyDropped.gameObject.SetActive(false);
        droppedMoneyPool.Add(newMoneyDropped, newMoneyDropped.gameObject);

        return newMoneyDropped;
    }

    void DeactivateAllActiveObjects()
    {
        Dictionary<EnemyManager, GameObject> activeEnemies = GetAllActiveEnemies();
        List<EnemyManager> newCollection = new List<EnemyManager>();
        foreach (var item in activeEnemies.Keys)
        {
            newCollection.Add(item);
        }

        foreach (var item in newCollection)
        {
            RemoveFromActiveEnemies(item);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameRestart -= DeactivateAllActiveObjects;
    }
}
