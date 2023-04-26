using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHover : MonoBehaviour
{
    public bool mouseHover = false;
    private bool pMouseHover = false;

    public delegate void MouseOverHandler(bool gameWon);
    public event MouseOverHandler OnMouseOverChanged;

    public delegate void MouseClickedHandler();
    public event MouseClickedHandler OnMouseOverClicked;




    private void Update()
    {
        if (UiTowerShopManager.Instance.movingTower || UiUpgradeShopManager.Instance.GetShopActive()) return;
        if (mouseHover != pMouseHover)
        {
            OnMouseOverChanged.Invoke(mouseHover); 
        }
        pMouseHover = mouseHover;
        if (mouseHover && Input.GetMouseButtonDown(0)) OnMouseOverClicked.Invoke(); 
    }

    void OnMouseOver()
    {
        mouseHover = true; 
    }

    void OnMouseExit()
    {
        mouseHover = false;
    }
}
