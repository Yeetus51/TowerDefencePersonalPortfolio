using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    bool waitForNextWave = true;
    int waveIndex = 0;

    List<WaveSO> allWaves = new List<WaveSO>();
    List<float> timers = new List<float>();
    List<float> spawnTimers = new List<float>();

    [SerializeField] Button startWaveButton;
    [SerializeField] int startWave; // Make sure its zero unless testing 

    bool stopSpawning;

    [SerializeField] float timeBetweenWaves;
    private bool checkGameWinState;

    private void Start()
    {

        ImportAllWaves();
        StartGame();

        GameManager.Instance.OnGameRestart += StartGame;

        GameManager.Instance.OnPauseGame += StopSpawning;

        SortWaves();
    }

    void ImportAllWaves()
    {
        string path = Application.dataPath + "/Resources/WaveScriptableObjects/";
        string[] files = Directory.GetFiles(path);
        int indexCheck = 0;
        for (int i = 0; i < files.Length/2; i++) allWaves.Add(null);
        int outOfOrderCheck = 0; 
        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]) == ".asset") // check if file is a ScriptableObject
            {
                outOfOrderCheck += i/2 + 1; 
                string filenameWithoutExtention = Path.GetFileNameWithoutExtension(files[i]);
                int fileIndex = GetFileIndex(filenameWithoutExtention);
                outOfOrderCheck -= fileIndex;
                if (fileIndex != indexCheck + 1) Debug.Log("Resorting will happen" + filenameWithoutExtention);
                WaveSO wave = Resources.Load<WaveSO>("WaveScriptableObjects/" + filenameWithoutExtention);
                if (wave != null)
                {
                    allWaves[fileIndex-1] = wave;
                    indexCheck++;
                }
            }
        }
        if (outOfOrderCheck == 0) Debug.Log("Waves are loaded correctly");
        else Debug.LogWarning("Some wave is missing, please double check the Waves folder" + outOfOrderCheck);

    }
    int GetFileIndex(string filenameWithoutExtension)
    {
        int index = filenameWithoutExtension.Length - 1;
        while (index >= 0 && char.IsDigit(filenameWithoutExtension[index]))
        {
            index--;
        }
        if (index < filenameWithoutExtension.Length - 1 && int.TryParse(filenameWithoutExtension.Substring(index + 1), out int number))
        {
            return number;
        }
        else
        {
            return 0;
        }
    }
    void SortWaves()
    {
        foreach (var wave in allWaves)
        {
            SubWaveSO lastWave = wave.subWaves[0];
            foreach (var subwave in wave.subWaves)
            {
                if (subwave.endTime > lastWave.endTime) lastWave = subwave;
            }
            wave.subWaves.Remove(lastWave);
            wave.subWaves.Add(lastWave);
        }
    }

    private IEnumerator TimeBetweenWaves(float waitTime)
    {
        UiManager.Instance.KillAllDoTweens();
        UiManager.Instance.SetWaveProgressBar(timeBetweenWaves);
        yield return new WaitForSeconds(waitTime);
        StartNewWaveButton(); 
    }

    void StartGame()
    {
        if (startWave != 0) waveIndex = startWave - 1;
        SetNewWave(waveIndex);
        stopSpawning = false;
        waitForNextWave = true;
        startWaveButton.interactable = true;
        checkGameWinState = false;
    }

    void SetNewWave(int wave)
    {
        //SubWaveSO last = allWaves[0]; 
        timers.Clear();
        spawnTimers.Clear();
        foreach (var item in allWaves[wave].subWaves)
        {
            timers.Add(0);
            spawnTimers.Add(0);
        }
        StartCoroutine(TimeBetweenWaves(timeBetweenWaves));
    }


    private void FixedUpdate()
    {
        if (stopSpawning) return;
        if (!waitForNextWave) SpawningWave(waveIndex);
    }
    private void Update()
    {
        if (checkGameWinState) CheckGameFinished(); 
    }

    void SpawningWave(int waveLevel)
    {

        bool allSubwavesFinished = false;
        for (int i = 0; i < allWaves[waveLevel].subWaves.Count; i++)
        {
            SubWaveSO wave = allWaves[waveLevel].subWaves[i];
            if (timers[i] > wave.endTime)
            {
                allSubwavesFinished = true;
                continue;
            }
            else allSubwavesFinished = false;

            timers[i] += Time.deltaTime;
            if (timers[i] >= wave.startTime)
            {
                spawnTimers[i] += Time.deltaTime;

                if (spawnTimers[i] > 1 / wave.frequency)
                {
                    spawnTimers[i] = 0;
                    SpawnNewEnemy((int)wave.enemyId);
                }
            }
        }
        foreach (var item in allWaves[waveLevel].subWaves)
        {

        }

        if (allSubwavesFinished) WaveFinished();
    }
    void WaveFinished()
    {
        waitForNextWave = true;
        startWaveButton.interactable = true;
        waveIndex++;
        if (waveIndex < allWaves.Count)
        {
            SetNewWave(waveIndex);
        }
        else
        {
            GameFinished();
            startWaveButton.interactable = false;
            //waitForNextWave = true;
        }
    }

    void GameFinished()
    {
        checkGameWinState = true; 
    }
    void CheckGameFinished()
    {
        if (ObjectPooler.Instance.GetAllActiveEnemies().Count < 1) GameManager.Instance.DisplayGameWon(); 
    }

    public void StartNewWaveButton()
    {
        startWaveButton.interactable = false;
        waitForNextWave = false;

        UiManager.Instance.KillAllDoTweens();
        UiManager.Instance.SetWaveProgressBar(allWaves[waveIndex].subWaves[allWaves[waveIndex].subWaves.Count - 1].endTime, waveIndex + 1, allWaves.Count);

        StopAllCoroutines();
    }

    void SpawnNewEnemy(int type)
    {
        EnemyManager newEnemy = ObjectPooler.Instance.GetPooledEnemy(type);
        newEnemy.ResetEnemy();
        newEnemy.gameObject.SetActive(true);
    }

    void StopSpawning()
    {
        stopSpawning = true;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameRestart -= StartGame;

        GameManager.Instance.OnPauseGame -= StopSpawning;
    }


}
