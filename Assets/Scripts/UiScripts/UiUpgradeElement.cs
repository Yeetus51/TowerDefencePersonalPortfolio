using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class UiUpgradeElement : MonoBehaviour
{
    public TMP_Text title;
    [HideInInspector] public int upgradeID;  
    [HideInInspector] public UiStatElement statUiElement;
    [HideInInspector] public Upgradable upgrade;

    public Button upgradeButton;
    public Button downgradeButton;

    public TMP_Text levelText;
    public TMP_Text upgradeCost; 
    public TMP_Text downgradeCost;

    public TMP_Text currentValue;
    public TMP_Text upgradedValue;
    public TMP_Text downgradedValue;

    public UiUpgradeShopManager upgradeShopManager;


    // Start is called before the first frame update
    void Start()
    {
        upgradeButton.onClick.AddListener(Upgrade);
        downgradeButton.onClick.AddListener(Downgrade);
    }
    void Upgrade()
    {
        upgradeShopManager.Upgraded(upgrade, this, statUiElement); 
    }
    void Downgrade()
    {
        upgradeShopManager.Downgraded(upgrade, this, statUiElement);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
