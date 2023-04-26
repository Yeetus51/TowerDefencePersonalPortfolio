using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiTowerShopManager : MonoBehaviour
{
    [SerializeField] List<TMP_Text> towerCostTexts = new List<TMP_Text>();
    [SerializeField] List<TowerTypeBase> towerSOs = new List<TowerTypeBase>();
    [SerializeField] List<Button> buttons = new List<Button>();
    [SerializeField] GameObject towersHolder;
    [SerializeField] WayPointPath path;

    Plane plane = new Plane(Vector3.up, 0);

    public Tower movingTower { get; private set; } = null;

    public static UiTowerShopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

        for (int i = 0; i < towerCostTexts.Count; i++)
        {
            towerCostTexts[i].text = towerSOs[i].cost + "$"; 
        }
        GameManager.Instance.OnGameRestart += RemoveAllTowers;
        GameManager.Instance.OnGameRestart += EnableButtons; 
        GameManager.Instance.OnMoneyChanged += CheckBuyableTowers;
        GameManager.Instance.OnPauseGame += DisableButtons; 
    }

    public void BoughtTower(int type)
    {
        Tower newTower = ObjectPooler.Instance.GetPooledTower(type);
        newTower.SetTowerData(); 
        newTower.enabled = false;
        newTower.transform.position = Vector3.zero;
        newTower.transform.rotation = Quaternion.identity;
        newTower.transform.parent = towersHolder.transform;
        newTower.gameObject.SetActive(true);
        GameManager.Instance.SubtractMoney(newTower.towerData.cost);
        movingTower = newTower;
        DisableButtons();
    }

    public void SetMovingTower(Tower pTower) { movingTower = pTower; }

    public void DisableButtons()
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }
    public void EnableButtons()
    {
        int totalMoney = GameManager.Instance.GetMoney(); 
        CheckBuyableTowers(totalMoney);
    }
    void CheckBuyableTowers(int toaltMoney)
    {
        if (UiUpgradeShopManager.Instance.GetShopActive()) return;
        for (int i = 0; i < buttons.Count; i++)
        {
            int money = GameManager.Instance.GetMoney();
            if (money < towerSOs[i].cost) buttons[i].interactable = false; 
            else buttons[i].interactable = true;
        }
    }

    void MoveTower(Tower tower)
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 position = ray.GetPoint(distance);
            Vector3 possibleLocation = GetPossibleLocation(position, tower, path.path, path.pathWidth * 10);
            tower.transform.position = new Vector3(possibleLocation.x, 0, possibleLocation.z);
        }

        if (Input.GetMouseButtonDown(0))
        {
            movingTower = null;
            tower.enabled = true;
            tower.ignoreOnClick = true;
            StartCoroutine(tower.ExtraWaitForReclickable());
            EnableButtons();
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            movingTower = null;
            tower.gameObject.SetActive(false);
            GameManager.Instance.AddMoney(tower.towerData.cost);
            EnableButtons();
        }
    }

    Vector3 GetPossibleLocation(Vector3 pPosition, Tower tower, Vector2[] path, float pathCollisionRange)
    {
        Vector3 position = pPosition;

        int maxIterations = 10;
        bool collisionResolved;

        for (int i = 0; i < maxIterations; i++)
        {
            collisionResolved = true;

            // Check for collisions with existing towers
            foreach (var existingTower in ObjectPooler.Instance.GetAllActivePooledTowers())
            {
                if (existingTower.Key == tower) continue;

                Vector3 otherPosition = existingTower.Value.transform.position;
                Vector3 diff = position - otherPosition;
                float range = tower.towerData.collisionRange + existingTower.Key.towerData.collisionRange;

                if (diff.magnitude < range)
                {
                    position = otherPosition + diff.normalized * range;
                    collisionResolved = false;
                }
            }

            // Check for collisions with the path line segments
            for (int j = 0; j < path.Length - 1; j++)
            {
                Vector3 startPoint = new Vector3(path[j].x, 0, path[j].y);
                Vector3 endPoint = new Vector3(path[j + 1].x, 0, path[j + 1].y);
                Vector3 closestPoint = ClosestPointOnLineSegment(startPoint, endPoint, position);
                Vector3 diff = position - closestPoint;

                if (diff.magnitude < pathCollisionRange)
                {
                    position = closestPoint + diff.normalized * pathCollisionRange;
                    collisionResolved = false;
                }
            }

            if (collisionResolved)
            {
                break;
            }
        }
        return position;
    }
    Vector3 ClosestPointOnLineSegment(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(p - a, ab) / Vector3.Dot(ab, ab);
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }

    private void Update()
    {
        if(movingTower)MoveTower(movingTower); 
    }

    void RemoveAllTowers()
    {
        int childCount = towersHolder.transform.childCount;

        if (childCount == 0) return;

        for (int i = childCount-1; i >= 0; i--)
        {
            towersHolder.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnGameRestart -= RemoveAllTowers;
        GameManager.Instance.OnGameRestart -= EnableButtons;
        GameManager.Instance.OnMoneyChanged -= CheckBuyableTowers;
        GameManager.Instance.OnPauseGame -= DisableButtons;
    }
}
