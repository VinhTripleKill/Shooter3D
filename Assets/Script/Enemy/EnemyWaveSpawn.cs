using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> enemiesPrefab = new List<GameObject>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Wave Settings")]
    [SerializeField] private int startingWave = 1;
    [SerializeField] private float delayBeforeFirstWave = 2f;
    [SerializeField] private float timeBetweenWaves = 1.5f;
    [SerializeField] private CoreGameUI coreGameUI;

    private int currentWave = 1;
    private int playerKillCount = 0;

    private List<BaseEnemy> currentWaveEnemies =
        new List<BaseEnemy>();

    private bool isSpawning = false;
    private bool waveInProgress = false;
    private bool gameOver = false;

    private Coroutine spawnCoroutine;
    private Coroutine nextWaveCoroutine;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        currentWave = startingWave;

        StartCoroutine(
            WaitForPlayerAndStartWave()
        );
    }


    private IEnumerator WaitForPlayerAndStartWave()
    {
        yield return new WaitForSeconds(0.5f);

        while (GameObject.FindGameObjectWithTag("Player") == null)
        {
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(
            delayBeforeFirstWave
        );

        StartNextWave();
    }


    // =====================================================
    // START WAVE
    // =====================================================

    public void StartNextWave()
    {
        if (isSpawning)
            return;

        if (waveInProgress)
            return;

        if (gameOver)
            return;

        // Kiểm tra prefab
        if (enemiesPrefab == null ||
            enemiesPrefab.Count == 0)
        {
            Debug.LogError(
                "EnemyWaveSpawn: Chưa gán Enemy Prefab!"
            );

            return;
        }

        // Kiểm tra spawn point
        if (spawnPoints == null ||
            spawnPoints.Count == 0)
        {
            Debug.LogError(
                "EnemyWaveSpawn: Chưa gán Spawn Points!"
            );

            return;
        }

        waveInProgress = true;

        currentWaveEnemies.Clear();

        int totalEnemyCount =
            enemiesPrefab.Count * currentWave;

        Debug.Log(
            $"=== WAVE {currentWave} BẮT ĐẦU === " +
            $"SPAWN {enemiesPrefab.Count} LOẠI x {currentWave} = " +
            $"{totalEnemyCount} ENEMIES"
        );

        spawnCoroutine =
            StartCoroutine(
                SpawnWaveCoroutine()
            );
    }


    // =====================================================
    // SPAWN WAVE
    // =====================================================

    private IEnumerator SpawnWaveCoroutine()
    {
        isSpawning = true;

        foreach (GameObject enemyPrefab in enemiesPrefab)
        {
            // Kiểm tra prefab null
            if (enemyPrefab == null)
            {
                Debug.LogWarning(
                    "EnemyWaveSpawn: Có Enemy Prefab đang bị null!"
                );

                continue;
            }

            // Mỗi loại enemy spawn = currentWave
            for (int i = 0; i < currentWave; i++)
            {
                // Nếu Game Over thì dừng spawn
                if (gameOver)
                {
                    isSpawning = false;
                    yield break;
                }

                // Lấy Spawn Point random
                Transform spawnPoint =
                    spawnPoints[
                        Random.Range(
                            0,
                            spawnPoints.Count
                        )
                    ];

                // Spawn enemy
                GameObject enemyObj =
                    Instantiate(
                        enemyPrefab,
                        spawnPoint.position,
                        spawnPoint.rotation
                    );

                // Lấy BaseEnemy
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
                        $"Enemy Prefab '{enemyPrefab.name}' " +
                        $"không có BaseEnemy component!"
                    );
                }

                /*
                 * Nếu muốn spawn từng enemy cách nhau,
                 * có thể bật dòng này:
                 *
                 * yield return new WaitForSeconds(0.2f);
                 */
            }
        }

        isSpawning = false;

        spawnCoroutine = null;

        coreGameUI?.OnWaveChanged();
    }


    // =====================================================
    // ENEMY DIED
    // =====================================================

    public void OnEnemyDied(BaseEnemy enemy)
    {
        if (enemy == null)
            return;

        currentWaveEnemies.Remove(enemy);

        coreGameUI?.OnWaveChanged();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (gameOver)
            return;

        if (!waveInProgress)
            return;

        // Xóa enemy null
        currentWaveEnemies.RemoveAll(
            enemy => enemy == null
        );

        /*
         * Khi:
         *
         * - Không còn enemy
         * - Và đã spawn xong
         *
         * => Wave hoàn thành
         */

        if (currentWaveEnemies.Count == 0 &&
            !isSpawning)
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


    // =====================================================
    // NEXT WAVE DELAY
    // =====================================================

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


    // =====================================================
    // GAME OVER
    // =====================================================

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        Debug.Log(
            "EnemyWaveSpawn: GAME OVER - DỪNG TOÀN BỘ SPAWN"
        );

        // Dừng coroutine spawn
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);

            spawnCoroutine = null;
        }

        // Dừng coroutine wave tiếp theo
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


    // =====================================================
    // KILL ALL ENEMIES
    // =====================================================

    private void KillAllEnemies()
    {
        /*
         * Tạo bản sao để tránh lỗi
         * khi danh sách bị thay đổi trong lúc Die()
         */

        List<BaseEnemy> enemiesToKill =
            new List<BaseEnemy>(
                BaseEnemy.AllEnemies
            );

        foreach (BaseEnemy enemy in enemiesToKill)
        {
            if (enemy == null)
                continue;

            if (enemy.IsDead())
                continue;

            enemy.ForceKill();
        }

        currentWaveEnemies.Clear();
    }


    // =====================================================
    // DESTROY ALL ENEMIES IMMEDIATELY
    // =====================================================

    public void DestroyAllEnemiesImmediately()
    {
        // Dừng coroutine spawn / wave cũ
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

        /*
         * Tạo bản sao để tránh lỗi
         * khi enemy bị Destroy
         */

        List<BaseEnemy> enemiesToDestroy =
            new List<BaseEnemy>(
                BaseEnemy.AllEnemies
            );

        foreach (BaseEnemy enemy in enemiesToDestroy)
        {
            if (enemy == null)
                continue;

            Destroy(enemy.gameObject);
        }

        // Xóa danh sách enemy cũ
        BaseEnemy.AllEnemies.Clear();

        currentWaveEnemies.Clear();

        Debug.Log(
            "Đã Destroy toàn bộ enemy của màn chơi cũ."
        );
    }


    // =====================================================
    // GET CURRENT WAVE
    // =====================================================

    public int GetCurrentWave()
    {
        return currentWave;
    }


    // =====================================================
    // GET REMAINING ENEMIES
    // =====================================================

    public int GetRemainingEnemiesInWave()
    {
        currentWaveEnemies.RemoveAll(
            enemy => enemy == null
        );

        return currentWaveEnemies.Count;
    }


    // =====================================================
    // RESET WAVES
    // =====================================================

    public void ResetWaves()
    {
        // Xóa toàn bộ enemy cũ trước
        DestroyAllEnemiesImmediately();

        currentWave = startingWave;

        currentWaveEnemies.Clear();

        waveInProgress = false;
        isSpawning = false;
        gameOver = false;

        spawnCoroutine = null;
        nextWaveCoroutine = null;

        // Reset player kill
        playerKillCount = 0;

        Debug.Log(
            "EnemyWaveSpawn đã Reset hoàn toàn."
        );
    }


    // =====================================================
    // PLAYER KILL
    // =====================================================

    public int GetPlayerKillCount()
    {
        return playerKillCount;
    }


    public void RegisterPlayerKill(BaseEnemy enemy)
    {
        if (gameOver)
            return;

        if (enemy == null)
            return;

        playerKillCount++;

        Debug.Log(
            $"PLAYER KILL +1 | TOTAL = {playerKillCount}"
        );
    }
}
