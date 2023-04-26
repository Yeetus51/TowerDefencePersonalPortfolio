using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class UiManager : MonoBehaviour
{
    [SerializeField] TMP_Text totalMoney;
    [SerializeField] TMP_Text livesLeft;

    [SerializeField] GameObject gameLostWindow;
    [SerializeField] GameObject gameWonWindow;
    [SerializeField] Button RestartButtonWon; 
    [SerializeField] Button RestartButtonLost;

    [SerializeField] RectTransform gameView;

    [SerializeField] TMP_Text wavesInfoText; 
    [SerializeField] GameObject waveProgressBar;
    [SerializeField] GameObject waveWaitBar;

    [SerializeField] Image waitBarBg; 
    [SerializeField] Image waitBarImage;
    [SerializeField] TMP_Text waitText; 



    [SerializeField] Color greenColor; 
    [SerializeField] Color redColor;


    Tweener waveProgressBarTween; 
    Tweener waveWaitTimerBarColor; 
    Tweener waveWaitTimerBarXScale;





    public static UiManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
        GameManager.Instance.OnLivesChanged += OnLivesChanged;
        GameManager.Instance.OnGameFinishedGame += DisplaWinState;
        GameManager.Instance.OnPauseGame += KillAllDoTweens; 
    }
    public void OnMoneyChanged(int newAmount)
    {
        UpdateMoneytext(newAmount);
    }

    public void UpdateMoneytext(int value)
    {
        totalMoney.text = value + "$";
    }

    public void UpdateLives(int value)
    {
        livesLeft.text = value.ToString(); 
    }
    public void KillAllDoTweens()
    {
        waveProgressBarTween.Kill();
        waveWaitTimerBarColor.Kill();
        waveWaitTimerBarXScale.Kill();
    }

    public void ShowLostGameWindow()
    {
        gameLostWindow.SetActive(true);
        RestartButtonWon.onClick.AddListener(RestartGame);
        RestartButtonLost.onClick.AddListener(RestartGame);
        waveProgressBarTween.Kill();
    }
    public void ShowWonGameWindow()
    {
        gameWonWindow.SetActive(true);
        RestartButtonWon.onClick.AddListener(RestartGame);
        RestartButtonLost.onClick.AddListener(RestartGame);
        waveProgressBarTween.Kill();
    }

    // Create a method that handles the lives change event
    public void OnLivesChanged(int newLives)
    {
        UpdateLives(newLives);
    }

    // Create a method that handles the game state change event
    public void DisplaWinState(bool gameWon)
    {
        if (gameWon)
        {
            ShowWonGameWindow();
        }
        else
        {
            ShowLostGameWindow();
        }
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
        gameLostWindow.SetActive(false);
        gameWonWindow.SetActive(false);
        waveProgressBar.transform.localScale = new Vector3(0, waveProgressBar.transform.localScale.y, waveProgressBar.transform.localScale.z);
    }

    public void SpawnTextOnScreen(Vector3 worldPosition, int money)
    {
        Camera mainCamera = Camera.main;
        RectTransform canvasRectTransform = gameView;
        Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        UiMoneyDroppedElement newDrop = ObjectPooler.Instance.GetPooledUiMoneyDropped();
        newDrop.gameObject.SetActive(true);
        newDrop.moneyText.text = "+ " + money + "$";
        newDrop.transform.position = new Vector3(screenPosition.x, screenPosition.y, 0);
        newDrop.transform.SetParent(gameView.transform);
        newDrop.FadeOutAndMoveUp();
    }

    public void SetWaveProgressBar(float duration, int currnetWave, int totalWaves)
    {
        wavesInfoText.text = "Wave " + currnetWave + "/" + totalWaves; 
        waveProgressBar.transform.localScale = new Vector3(0, waveProgressBar.transform.localScale.y, waveProgressBar.transform.localScale.z);

        waveProgressBarTween =  waveProgressBar.transform.DOScaleX(1, duration);
        DeactivateWaveTimer();
    }
    public void SetWaveProgressBar(float duration)
    {
        waveWaitBar.transform.localScale = new Vector3(1, waveWaitBar.transform.localScale.y, waveWaitBar.transform.localScale.z);
        waitBarImage.color = greenColor;
        ActivateWaveTimer();
        //StartCoroutine(DeactivateWaveTimer(duration));

        waveWaitTimerBarXScale = waveWaitBar.transform.DOScaleX(0, duration);
        waveWaitTimerBarColor = waitBarImage.DOColor(redColor, duration);
    }


    void ActivateWaveTimer()
    {
        waitBarBg.DOFade(0.5f, 0.5f);
        waitBarImage.DOFade(1, 0.5f);
        waitText.DOFade(1, 0.5f);
    }

    private void  DeactivateWaveTimer()
    {
        waveWaitTimerBarColor.Kill();
        waveWaitTimerBarXScale.Kill();

        waitBarBg.DOFade(0, 0.5f);
        waitBarImage.DOFade(0, 0.5f);
        waitText.DOFade(0, 0.5f);
    }


    private void OnDestroy()
    {
        // Unsubscribe from the events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged -= OnMoneyChanged;
            GameManager.Instance.OnLivesChanged -= OnLivesChanged;
            GameManager.Instance.OnGameFinishedGame -= DisplaWinState;
            GameManager.Instance.OnPauseGame -= KillAllDoTweens;
        }
    }
}





