using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Wave Settings")]
    [SerializeField] private int startingWave = 1;
    [SerializeField] private float delayBeforeFirstWave = 2f;
    [SerializeField] private float timeBetweenWaves = 1.5f;
    [SerializeField] private CoreGameUI coreGameUI;

    private int currentWave = 1;
    private List<BaseEnemy> currentWaveEnemies = new List<BaseEnemy>();
    private bool isSpawning = false;
    private bool waveInProgress = false;
    private bool gameOver = false;                    // <--- THÊM

    [SerializeField] private PlayerSpawn playerSpawn;

    private void Start()
    {
        currentWave = startingWave;
        StartCoroutine(WaitForPlayerAndStartWave());
    }

    private IEnumerator WaitForPlayerAndStartWave()
    {
        yield return new WaitForSeconds(0.5f);
        
        while (GameObject.FindGameObjectWithTag("Player") == null)
            yield return new WaitForSeconds(0.2f);

        yield return new WaitForSeconds(delayBeforeFirstWave);
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (isSpawning || waveInProgress || gameOver) return;   // <--- THÊM gameOver

        waveInProgress = true;
        currentWaveEnemies.Clear();

        int enemyCount = currentWave;

        Debug.Log($"=== WAVE {currentWave} BẮT ĐẦU - Spawn {enemyCount} Zombie ===");

        StartCoroutine(SpawnWaveCoroutine(enemyCount));
    }

    private IEnumerator SpawnWaveCoroutine(int count)
    {
        isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogError("Chưa gán Spawn Points!");
                yield break;
            }

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            GameObject enemyObj = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
            BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();

            if (enemy != null)
                currentWaveEnemies.Add(enemy);

            yield return new WaitForSeconds(0.3f);
        }

        isSpawning = false;
        coreGameUI?.OnWaveChanged();
    }

    private void Update()
    {
        if (!waveInProgress || gameOver) return;          // <--- THÊM gameOver

        currentWaveEnemies.RemoveAll(enemy => enemy == null || enemy.IsDead());

        if (currentWaveEnemies.Count == 0 && !isSpawning)
        {
            waveInProgress = false;
            coreGameUI?.OnWaveChanged();

            currentWave++;
            StartCoroutine(NextWaveDelay());
        }
    }

    private IEnumerator NextWaveDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
    }

    // ====================== PUBLIC ======================
    public int GetCurrentWave() => currentWave;

    public int GetRemainingEnemiesInWave()
    {
        currentWaveEnemies.RemoveAll(enemy => enemy == null || enemy.IsDead());
        return currentWaveEnemies.Count;
    }

    public void GameOver()                                // <--- THÊM METHOD NÀY
    {
        gameOver = true;
        waveInProgress = false;
        isSpawning = false;
        Debug.Log("EnemyWaveSpawn: Game Over - Dừng spawn wave mới");
    }

    public void ResetWaves()
    {
        currentWave = startingWave;
        currentWaveEnemies.Clear();
        waveInProgress = false;
        isSpawning = false;
        gameOver = false;
    }
}