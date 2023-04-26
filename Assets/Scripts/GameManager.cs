using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] UiManager uiManager;

    [SerializeField] LevelData levelData; 

    int money;
    int lives;
    bool gameLost;



    public static GameManager Instance { get; private set; }


    public delegate void GameRestartHandler();
    public event GameRestartHandler OnGameRestart;

    public delegate void MoneyChangedHandler(int newAmount);
    public event MoneyChangedHandler OnMoneyChanged;

    // Add the delegate and event for lives
    public delegate void LivesChangedHandler(int newLives);
    public event LivesChangedHandler OnLivesChanged;

    // Add the delegate and event for game state changes
    public delegate void GameStateChangedHandler();
    public event GameStateChangedHandler OnPauseGame;

    public delegate void GameFinishedHandler(bool gameWon);
    public event GameFinishedHandler OnGameFinishedGame;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
        OnGameRestart += StartGame;
    }

    private void Start()
    {
        StartGame(); 
    }

    void StartGame()
    {
        money = levelData.startingMoney;
        lives = levelData.startingLives;

        uiManager.UpdateMoneytext(money);
        uiManager.UpdateLives(lives);
    }
    public int AddMoney(int amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke(money); // Trigger the event
        return money;
    }

    public int SubtractMoney(int amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke(money); // Trigger the event
        return money;
    }

    public int GetMoney()
    {
        return money;
    }

    public void LifeLost()
    {
        lives--;
        OnLivesChanged?.Invoke(lives); // Trigger the event
        CheckGameLost();
    }
    void CheckGameLost()
    {
        if (lives <= 0) {
            gameLost = true;
            DisplayGameLost();
        }
    }

    void DisplayGameLost()
    {
        uiManager.ShowLostGameWindow();
        OnPauseGame?.Invoke(); // Trigger the event
        OnGameFinishedGame?.Invoke(false); 
    }

    public void DisplayGameWon()
    {
        uiManager.ShowWonGameWindow();
        OnPauseGame?.Invoke(); // Trigger the event
        OnGameFinishedGame?.Invoke(true);
    }

    public void RestartGame()
    {
        OnGameRestart.Invoke(); 
    }
}
