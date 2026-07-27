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

    private List<BaseEnemy> currentWaveEnemies =
        new List<BaseEnemy>();

    private bool isSpawning = false;
    private bool waveInProgress = false;
    private bool gameOver = false;

    private Coroutine spawnCoroutine;
    private Coroutine nextWaveCoroutine;

    [SerializeField] private PlayerSpawn playerSpawn;

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

        while (
            GameObject.FindGameObjectWithTag("Player") == null
        )
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

        waveInProgress = true;
        currentWaveEnemies.Clear();

        int enemyCount = currentWave;

        Debug.Log(
            $"=== WAVE {currentWave} BẮT ĐẦU - SPAWN {enemyCount} ZOMBIE ==="
        );

        spawnCoroutine =
            StartCoroutine(
                SpawnWaveCoroutine(enemyCount)
            );
    }

    // =====================================================
    // SPAWN ENEMY
    // =====================================================

    private IEnumerator SpawnWaveCoroutine(int count)
    {
        isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            // Nếu hết thời gian thì dừng ngay
            if (gameOver)
            {
                isSpawning = false;
                yield break;
            }

            if (spawnPoints.Count == 0)
            {
                Debug.LogError(
                    "Chưa gán Spawn Points!"
                );

                isSpawning = false;
                yield break;
            }

            Transform spawnPoint =
                spawnPoints[
                    Random.Range(
                        0,
                        spawnPoints.Count
                    )
                ];

            GameObject enemyObj =
                Instantiate(
                    zombiePrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            BaseEnemy enemy =
                enemyObj.GetComponent<BaseEnemy>();

            if (enemy != null)
            {
                enemy.Initialize(this);
                currentWaveEnemies.Add(enemy);
            }

            // Nếu bạn muốn spawn từng con cách nhau
            // thì có thể thêm:
            //
            // yield return new WaitForSeconds(0.2f);
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

        currentWaveEnemies.RemoveAll(
            enemy => enemy == null
        );

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

        // -----------------------------------------
        // 1. Dừng coroutine spawn hiện tại
        // -----------------------------------------

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        // -----------------------------------------
        // 2. Dừng chờ wave tiếp theo
        // -----------------------------------------

        if (nextWaveCoroutine != null)
        {
            StopCoroutine(nextWaveCoroutine);
            nextWaveCoroutine = null;
        }

        isSpawning = false;
        waveInProgress = false;

        // -----------------------------------------
        // 3. Kill toàn bộ enemy đang tồn tại
        // -----------------------------------------

        KillAllEnemies();

        // -----------------------------------------
        // 4. Cập nhật UI
        // -----------------------------------------

        coreGameUI?.OnWaveChanged();
    }

    private void KillAllEnemies()
    {
        // Tạo bản sao để tránh lỗi
        // khi danh sách bị thay đổi trong lúc Die()
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

   public void DestroyAllEnemiesImmediately()
{
    // Dừng các coroutine spawn / wave cũ
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

    // Tạo bản sao để tránh lỗi khi enemy bị Destroy
    List<BaseEnemy> enemiesToDestroy =
        new List<BaseEnemy>(BaseEnemy.AllEnemies);

    foreach (BaseEnemy enemy in enemiesToDestroy)
    {
        if (enemy == null)
            continue;

        Destroy(enemy.gameObject);
    }

    // Xóa danh sách enemy cũ
    BaseEnemy.AllEnemies.Clear();
    currentWaveEnemies.Clear();

    Debug.Log("Đã Destroy toàn bộ enemy của màn chơi cũ.");
}

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public int GetRemainingEnemiesInWave()
    {
        currentWaveEnemies.RemoveAll(
            enemy => enemy == null
        );

        return currentWaveEnemies.Count;
    }

    public void ResetWaves()
{
    // XÓA TOÀN BỘ ENEMY CŨ TRƯỚC
    DestroyAllEnemiesImmediately();

    currentWave = startingWave;

    currentWaveEnemies.Clear();

    waveInProgress = false;
    isSpawning = false;
    gameOver = false;

    spawnCoroutine = null;
    nextWaveCoroutine = null;

    Debug.Log("EnemyWaveSpawn đã Reset hoàn toàn.");
}
}