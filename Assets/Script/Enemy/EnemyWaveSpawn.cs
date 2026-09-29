using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawn : MonoBehaviour
{
    // =========================================================
    // ENEMY DATABASE
    // =========================================================

    [Header("Enemy Database")]
    [SerializeField]
    private EnemyDatabase enemyDatabase;


    // =========================================================
    // SPAWN POINTS
    // =========================================================

    [Header("Spawn Settings")]
    [SerializeField]
    private List<Transform> spawnPoints =
        new List<Transform>();


    // =========================================================
    // WAVE SETTINGS
    // =========================================================

    [Header("Wave Settings")]
    [SerializeField]
    private int startingWave = 1;

    [SerializeField]
    private float delayBeforeFirstWave = 2f;

    [SerializeField]
    private float timeBetweenWaves = 1.5f;

    [SerializeField]
    private CoreGameUI coreGameUI;


    // =========================================================
    // RUNTIME
    // =========================================================

    private int currentWave = 1;
    private int playerKillCount = 0;

    private List<BaseEnemy> currentWaveEnemies =
        new List<BaseEnemy>();

    private bool isSpawning = false;
    private bool waveInProgress = false;
    private bool gameOver = false;

    private Coroutine spawnCoroutine;
    private Coroutine nextWaveCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        currentWave = startingWave;

        StartCoroutine(
            WaitForPlayerAndStartWave()
        );
    }


    // =========================================================
    // WAIT PLAYER
    // =========================================================

    private IEnumerator WaitForPlayerAndStartWave()
    {
        yield return new WaitForSeconds(0.5f);

        while (
            GameObject.FindGameObjectWithTag("Player")
            == null
        )
        {
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(
            delayBeforeFirstWave
        );

        StartNextWave();
    }


    // =========================================================
    // START WAVE
    // =========================================================

    public void StartNextWave()
    {
        if (isSpawning)
            return;

        if (waveInProgress)
            return;

        if (gameOver)
            return;


        // =====================================================
        // DATABASE
        // =====================================================

        if (
            enemyDatabase == null ||
            enemyDatabase.Enemies == null ||
            enemyDatabase.Enemies.Count == 0
        )
        {
            Debug.LogError(
                "EnemyWaveSpawn: Chưa gán EnemyDatabase " +
                "hoặc EnemyDatabase không có EnemyData!"
            );

            return;
        }


        // =====================================================
        // SPAWN POINT
        // =====================================================

        if (
            spawnPoints == null ||
            spawnPoints.Count == 0
        )
        {
            Debug.LogError(
                "EnemyWaveSpawn: Chưa gán Spawn Points!"
            );

            return;
        }


        // =====================================================
        // START
        // =====================================================

        waveInProgress = true;

        currentWaveEnemies.Clear();


        int totalEnemyCount =
            enemyDatabase.Enemies.Count *
            currentWave;


        Debug.Log(
            $"=== WAVE {currentWave} BẮT ĐẦU === " +
            $"SPAWN {enemyDatabase.Enemies.Count} LOẠI x " +
            $"{currentWave} = " +
            $"{totalEnemyCount} ENEMIES"
        );


        spawnCoroutine =
            StartCoroutine(
                SpawnWaveCoroutine()
            );
    }


    // =========================================================
    // SPAWN WAVE
    // =========================================================

    private IEnumerator SpawnWaveCoroutine()
    {
        isSpawning = true;


        foreach (
            EnemyData enemyData
            in enemyDatabase.Enemies
        )
        {
            // =================================================
            // NULL DATA
            // =================================================

            if (enemyData == null)
            {
                Debug.LogWarning(
                    "EnemyWaveSpawn: Có EnemyData null!"
                );

                continue;
            }


            // =================================================
            // PREFAB
            // =================================================

            if (enemyData.enemyPrefab == null)
            {
                Debug.LogWarning(
                    $"EnemyWaveSpawn: EnemyData " +
                    $"'{enemyData.enemyName}' " +
                    $"chưa có Enemy Prefab!"
                );

                continue;
            }


            // =================================================
            // SPAWN
            // =================================================

            for (
                int i = 0;
                i < currentWave;
                i++
            )
            {
                if (gameOver)
                {
                    isSpawning = false;

                    yield break;
                }


                // =============================================
                // RANDOM SPAWN POINT
                // =============================================

                Transform spawnPoint =
                    spawnPoints[
                        Random.Range(
                            0,
                            spawnPoints.Count
                        )
                    ];


                // =============================================
                // INSTANTIATE
                // =============================================

                GameObject enemyObj =
                    Instantiate(
                        enemyData.enemyPrefab,
                        spawnPoint.position,
                        spawnPoint.rotation
                    );


                // =============================================
                // APPLY ENEMY DATA
                // =============================================

                EnemyCharacter enemyCharacter =
                    enemyObj.GetComponent<EnemyCharacter>();


                if (enemyCharacter != null)
                {
                    enemyCharacter.SetEnemyData(
                        enemyData
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"Enemy Prefab " +
                        $"'{enemyData.enemyPrefab.name}' " +
                        $"không có EnemyCharacter!"
                    );
                }


                // =============================================
                // BASE ENEMY
                // =============================================

                BaseEnemy enemy =
                    enemyObj.GetComponent<BaseEnemy>();


                if (enemy != null)
                {
                    enemy.Initialize(this);

                    currentWaveEnemies.Add(enemy);
                }
                else
                {
                    Debug.LogWarning(
                        $"Enemy Prefab " +
                        $"'{enemyData.enemyPrefab.name}' " +
                        $"không có BaseEnemy component!"
                    );
                }


                /*
                 * Nếu muốn spawn từng enemy cách nhau:
                 *
                 * yield return new WaitForSeconds(0.2f);
                 */
            }
        }


        isSpawning = false;

        spawnCoroutine = null;

        coreGameUI?.OnWaveChanged();
    }


    // =========================================================
    // ENEMY DIED
    // =========================================================

    public void OnEnemyDied(BaseEnemy enemy)
    {
        if (enemy == null)
            return;

        currentWaveEnemies.Remove(enemy);

        coreGameUI?.OnWaveChanged();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (gameOver)
            return;

        if (!waveInProgress)
            return;


        currentWaveEnemies.RemoveAll(
            enemy => enemy == null
        );


        // =====================================================
        // WAVE COMPLETE
        // =====================================================

        if (
            currentWaveEnemies.Count == 0 &&
            !isSpawning
        )
        {
            waveInProgress = false;

            coreGameUI?.OnWaveChanged();

            currentWave++;


            nextWaveCoroutine =
                StartCoroutine(
                    NextWaveDelay()
                );
        }
    }


    // =========================================================
    // NEXT WAVE DELAY
    // =========================================================

    private IEnumerator NextWaveDelay()
    {
        yield return new WaitForSeconds(
            timeBetweenWaves
        );


        if (gameOver)
            yield break;


        StartNextWave();

        nextWaveCoroutine = null;
    }


    // =========================================================
    // GAME OVER
    // =========================================================

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;


        Debug.Log(
            "EnemyWaveSpawn: GAME OVER - " +
            "DỪNG TOÀN BỘ SPAWN"
        );


        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);

            spawnCoroutine = null;
        }


        if (nextWaveCoroutine != null)
        {
            StopCoroutine(nextWaveCoroutine);

            nextWaveCoroutine = null;
        }


        isSpawning = false;
        waveInProgress = false;


        KillAllEnemies();

        coreGameUI?.OnWaveChanged();
    }


    // =========================================================
    // KILL ALL ENEMIES
    // =========================================================

    private void KillAllEnemies()
    {
        List<BaseEnemy> enemiesToKill =
            new List<BaseEnemy>(
                BaseEnemy.AllEnemies
            );


        foreach (
            BaseEnemy enemy
            in enemiesToKill
        )
        {
            if (enemy == null)
                continue;

            if (enemy.IsDead())
                continue;

            enemy.ForceKill();
        }


        currentWaveEnemies.Clear();
    }


    // =========================================================
    // DESTROY ALL ENEMIES IMMEDIATELY
    // =========================================================

    public void DestroyAllEnemiesImmediately()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);

            spawnCoroutine = null;
        }


        if (nextWaveCoroutine != null)
        {
            StopCoroutine(nextWaveCoroutine);

            nextWaveCoroutine = null;
        }


        isSpawning = false;
        waveInProgress = false;


        List<BaseEnemy> enemiesToDestroy =
            new List<BaseEnemy>(
                BaseEnemy.AllEnemies
            );


        foreach (
            BaseEnemy enemy
            in enemiesToDestroy
        )
        {
            if (enemy == null)
                continue;

            Destroy(enemy.gameObject);
        }


        BaseEnemy.AllEnemies.Clear();

        currentWaveEnemies.Clear();


        Debug.Log(
            "Đã Destroy toàn bộ enemy của màn chơi cũ."
        );
    }


    // =========================================================
    // GET CURRENT WAVE
    // =========================================================

    public int GetCurrentWave()
    {
        return currentWave;
    }


    // =========================================================
    // GET REMAINING ENEMIES
    // =========================================================

    public int GetRemainingEnemiesInWave()
    {
        currentWaveEnemies.RemoveAll(
            enemy => enemy == null
        );

        return currentWaveEnemies.Count;
    }


    // =========================================================
    // RESET WAVES
    // =========================================================

    public void ResetWaves()
    {
        DestroyAllEnemiesImmediately();

        currentWave = startingWave;

        currentWaveEnemies.Clear();

        waveInProgress = false;
        isSpawning = false;
        gameOver = false;

        spawnCoroutine = null;
        nextWaveCoroutine = null;

        playerKillCount = 0;


        Debug.Log(
            "EnemyWaveSpawn đã Reset hoàn toàn."
        );
    }


    // =========================================================
    // PLAYER KILL
    // =========================================================

    public int GetPlayerKillCount()
    {
        return playerKillCount;
    }


    public void RegisterPlayerKill(
        BaseEnemy enemy
    )
    {
        if (gameOver)
            return;

        if (enemy == null)
            return;

        playerKillCount++;


        Debug.Log(
            $"PLAYER KILL +1 | TOTAL = " +
            $"{playerKillCount}"
        );
    }
}