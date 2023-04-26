using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UiUpgradeShopManager : MonoBehaviour
{
    [SerializeField] private GameObject upgradeShop; 
    [SerializeField] private List<UiUpgradeElement> upgradeUiElements = new List<UiUpgradeElement>();
    [SerializeField] private List<UiStatElement> statUiElements = new List<UiStatElement>();
    [SerializeField] private TMP_Text towerName;
    [SerializeField] private Image TowerImage;
    [SerializeField] private Button moveButton;
    [SerializeField] TMP_Text towerSellText; 
    [SerializeField] int movingCost;
    int towerSellingPay; 

    private Tower tower;

    public static UiUpgradeShopManager Instance { get; private set; }




    private void Awake()
    {
        upgradeShop.gameObject.SetActive(false);

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GameManager.Instance.OnPauseGame += CloseUpgradeWindowOnRestart;
    }

    public void Initiate(Tower pTower)
    {
        upgradeShop.gameObject.SetActive(true);
        upgradeUiElements.ForEach(item => item.gameObject.SetActive(false));
        statUiElements.ForEach(item => item.gameObject.SetActive(false));
        UiTowerShopManager.Instance.DisableButtons(); 


        tower = pTower;
        int numberOfUpgrades = tower.towerData.upgradables.Count;

        towerName.text = tower.towerData.towerName;
        TowerImage.sprite = tower.towerData.thumbnail;

        for (int i = 0; i < numberOfUpgrades; i++)
        {
            Upgradable upgrade = tower.towerData.upgradables[i];
            upgradeUiElements[i].gameObject.SetActive(true);
            statUiElements[i].gameObject.SetActive(true);

            upgradeUiElements[i].statUiElement = statUiElements[i];
            SetUpgradeValues(upgrade, upgradeUiElements[i]);
            SetStatsValues(upgrade, statUiElements[i]);
            CheckPayableUpgrades();
            CheckDowngradeables();
        }
    }

    private void SetUpgradeValues(Upgradable upgrade, UiUpgradeElement upgradeUiElement)
    {
        int upgradeType = (int)upgrade.upgradeType;

        upgradeUiElement.upgradeID = upgradeType;

        upgradeUiElement.upgrade = upgrade;

        upgradeUiElement.title.text = upgrade.upgradeName;

        int costIndex = tower.costIndexes[upgrade];

        upgradeUiElement.levelText.text = "Lvl : " + costIndex;
        upgradeUiElement.upgradeCost.text = "Pay: " + upgrade.GetData(tower).cost + "$";
        upgradeUiElement.downgradeCost.text = "Sell: " + upgrade.GetData(tower).downgradePay + "$";

        upgradeUiElement.currentValue.text = upgrade.GetData(tower).value.ToString();

        if (costIndex + 1 < upgrade.upgradeLevels.Count)
        {
            upgradeUiElement.upgradedValue.text = upgrade.GetData(tower,1).value.ToString();
        }
        else
        {
            upgradeUiElement.upgradedValue.text = "";
            upgradeUiElement.upgradeCost.text = "MAX";
            upgradeUiElement.upgradeButton.interactable = false;
        }
        if (costIndex > 0)
        {
            upgradeUiElement.downgradedValue.text = upgrade.GetData(tower,-1).value.ToString();
        }
        else
        {
            upgradeUiElement.downgradedValue.text = "";
            upgradeUiElement.downgradeCost.text = "Default";
            upgradeUiElement.downgradeButton.interactable = false;
        }
    }

    public void Upgraded(Upgradable upgrade, UiUpgradeElement upgradeUi,UiStatElement statUi)
    {
        float upgradeValue = upgrade.GetData(tower,1).value;
        int cost = upgrade.GetData(tower).cost;

        if (!upgradeUi.downgradeButton.interactable)
        {
            upgradeUi.downgradeButton.interactable = true;
        }

        tower.Upgrade((int)upgrade.upgradeType, upgradeValue);

        GameManager.Instance.SubtractMoney(cost);
        tower.costIndexes[upgrade]++;

        CheckPayableUpgrades();

        SetUpgradeValues(upgrade, upgradeUi);
        SetStatsValues(upgrade, statUi);
    }

    public void CheckPayableUpgrades()
    {
        for (int i = 0; i < tower.towerData.upgradables.Count; i++)
        {
            CheckPayable(tower.towerData.upgradables[i], i);
        }
        if (GameManager.Instance.GetMoney() < movingCost) moveButton.interactable = false; 
        else moveButton.interactable = true;
        UpdateTowerSellPay();
    }
    void CheckDowngradeables()
    {
        for (int i = 0; i < tower.towerData.upgradables.Count; i++)
        {
            CheckDowngrade(tower.towerData.upgradables[i], i);
        }
    }
    void CheckDowngrade(Upgradable upgrade, int uiUpgradeIndex)
    {
        UiUpgradeElement upgradeUiElement = upgradeUiElements[uiUpgradeIndex];

        if (tower.costIndexes[upgrade] > 1) upgradeUiElement.downgradeButton.interactable = true;

    }
    private void CheckPayable(Upgradable upgrade, int uiUpgradeIndex)
    {
        UiUpgradeElement upgradeUiElement = upgradeUiElements[uiUpgradeIndex];

        if (CanPay(upgrade))
        {
            upgradeUiElement.upgradeCost.color = Color.green;
            if(tower.costIndexes[upgrade]  < upgrade.upgradeLevels.Count - 1) upgradeUiElement.upgradeButton.interactable = true;
        }
        else
        {
            upgradeUiElement.upgradeCost.color = Color.red;
            upgradeUiElement.upgradeButton.interactable = false;
        }
    }

    public void Downgraded(Upgradable upgrade, UiUpgradeElement upgradeUi, UiStatElement statUi)
    {
        float downgradeValue = upgrade.GetData(tower, -1).value;
        int payback = upgrade.GetData(tower).downgradePay;

        tower.Upgrade((int)upgrade.upgradeType, downgradeValue);

        GameManager.Instance.AddMoney(payback);

        tower.costIndexes[upgrade]--;

        SetUpgradeValues(upgrade, upgradeUi);
        SetStatsValues(upgrade, statUi);

        CheckPayableUpgrades();
    }
    public void MoveTower()
    {
        GameManager.Instance.SubtractMoney(movingCost);
        UiTowerShopManager.Instance.SetMovingTower(tower);
        UiTowerShopManager.Instance.EnableButtons();
        CloseWindow();
        tower.enabled = false;
    }
    void UpdateTowerSellPay()
    {
        int pay = (int)tower.towerData.cost/2;
        foreach (var upgrade in tower.costIndexes)
        {
            int index = upgrade.Value; 
            for (int i = index; i > 0; i--)
            {
                pay += upgrade.Key.upgradeLevels[i].downgradePay; 
            }    
        }
        towerSellingPay = pay;
        towerSellText.text = "Sell:" + pay + "$"; 
    }

    public void SellTower()
    {
        CloseWindow();
        GameManager.Instance.AddMoney(towerSellingPay);
        tower.gameObject.SetActive(false);
    }


    private bool CanPay(Upgradable upgrade)
    {
        return upgrade.GetData(tower).cost <= GameManager.Instance.GetMoney();
    }

    void SetStatsValues(Upgradable upgrade, UiStatElement upgradeUiElement)
    {

        upgradeUiElement.upgradeName.text = upgrade.upgradeName;
        upgradeUiElement.SetBarValue(CalcBarValue(upgrade.GetData(tower).value, upgrade.GetLastDataPoint().value)); 
    }
    void CloseUpgradeWindowOnRestart()
    {
        upgradeShop.gameObject.SetActive(false);
    }
    public void CloseWindow()
    {
        upgradeShop.gameObject.SetActive(false);
        UiTowerShopManager.Instance.EnableButtons();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPauseGame -= CloseUpgradeWindowOnRestart;
    }
    float CalcBarValue(float value, float max) => value / max;
    public bool GetShopActive() => upgradeShop.activeInHierarchy; 
}