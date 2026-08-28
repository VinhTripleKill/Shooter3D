using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
[System.Serializable]
public class GameResultData
{
    public float playTime;
    public int survivedWave;
    public int enemyKilled;
}
public class CoreGameUI : MonoBehaviour
{
    public GameResultData ResultData { get; private set; }

public bool IsGameEnded => gameEnded;
    [Header("Wave Enemy")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI amountEnemyInWave;

    [Header("Time")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Pause Game")]
    [SerializeField] private GameObject pauseGameUI;
    [SerializeField] private Button pauseB;

    [Header("Result Game")]
    [SerializeField] private GameObject resultGameUI;

    private PlayerInput playerInput;
    private InputAction pauseGame;

    private EnemyWaveSpawn waveManager;
    private PlayerController playerController;

private const float GAME_DURATION = 120f;

private float elapsedPlayTime = 0f;

private bool isTimerRunning = false;
private bool gameEnded = false;

public float ElapsedPlayTime => elapsedPlayTime;

    private void Start()
    {
        if (waveText != null)
            waveText.text = "WAVE 1";

        if (amountEnemyInWave != null)
            amountEnemyInWave.text = "0";

        if (timeText != null)
            timeText.text = "02:00";

        if (pauseGameUI != null)
            pauseGameUI.SetActive(false);

        if (resultGameUI != null)
            resultGameUI.SetActive(false);

        if (pauseB != null)
        {
            pauseB.gameObject.SetActive(true);
            pauseB.onClick.AddListener(PauseGame);
        }
    }

    private void OnDestroy()
    {
        UnregisterPauseInput();

        if (pauseB != null)
            pauseB.onClick.RemoveListener(PauseGame);

        Time.timeScale = 1f;
    }

    // =====================================================
    // PLAYER INPUT
    // =====================================================

    public void RegisterPlayerInput(PlayerInput input)
    {
        UnregisterPauseInput();

        playerInput = input;

        if (playerInput == null)
        {
            Debug.LogWarning(
                "CoreGameUI: PlayerInput bị null!"
            );

            return;
        }

        pauseGame =
            playerInput.actions["Pause&ResumeGame"];

        if (pauseGame == null)
        {
            Debug.LogWarning(
                "CoreGameUI: Không tìm thấy Action Pause&ResumeGame!"
            );

            return;
        }

        pauseGame.performed += OnPausePerformed;

        Debug.Log(
            "CoreGameUI: Đã đăng ký Pause&ResumeGame"
        );
    }

    private void UnregisterPauseInput()
    {
        if (pauseGame != null)
        {
            pauseGame.performed -= OnPausePerformed;
            pauseGame = null;
        }

        playerInput = null;
    }

    private void OnPausePerformed(
        InputAction.CallbackContext context)
    {
        TogglePause();
    }

    // =====================================================
    // PAUSE
    // =====================================================

    private void TogglePause()
    {
        if (gameEnded)
            return;

        if (Time.timeScale > 0f)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (gameEnded)
            return;

        Time.timeScale = 0f;

        if (pauseGameUI != null)
            pauseGameUI.SetActive(true);

        if (pauseB != null)
            pauseB.gameObject.SetActive(false);

        if (playerController != null)
            playerController.SetCanMove(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (pauseGameUI != null)
            pauseGameUI.SetActive(false);

        if (pauseB != null)
            pauseB.gameObject.SetActive(true);

        if (playerController != null)
            playerController.SetCanMove(true);
    }

    // =====================================================
    // INITIALIZE
    // =====================================================

    public void Initialize(
        EnemyWaveSpawn waveSpawnManager,
        PlayerController playerCtrl)
    {
        waveManager = waveSpawnManager;
        playerController = playerCtrl;

        if (waveManager != null)
            UpdateWaveUI();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (gameEnded)
            return;

        UpdateTimer();
    }

    // =====================================================
    // WAVE UI
    // =====================================================

    private void UpdateWaveUI()
    {
        if (waveManager == null)
            return;

        int currentWave =
            waveManager.GetCurrentWave();

        int remaining =
            waveManager.GetRemainingEnemiesInWave();

        // Hiển thị Wave
        if (waveText != null)
        {
            waveText.text =
                $"WAVE {currentWave}";
        }

        // Hiển thị số Enemy còn lại
        if (amountEnemyInWave != null)
        {
            amountEnemyInWave.text =
                remaining.ToString();
        }
    }

    public void OnWaveChanged()
    {
        UpdateWaveUI();
    }

    // =====================================================
    // TIMER
    // =====================================================

private void UpdateTimeText()
{
    float timeLeft =
        Mathf.Max(
            GAME_DURATION - elapsedPlayTime,
            0f
        );

    int minutes =
        Mathf.FloorToInt(timeLeft / 60f);

    int seconds =
        Mathf.FloorToInt(timeLeft % 60f);

    if (timeText != null)
    {
        timeText.text =
            string.Format(
                "{0:00}:{1:00}",
                minutes,
                seconds
            );
    }
}
private void UpdateTimer()
{
    if (!isTimerRunning)
        return;

    elapsedPlayTime += Time.deltaTime;

    if (elapsedPlayTime >= GAME_DURATION)
    {
        elapsedPlayTime = GAME_DURATION;

        UpdateTimeText();

        EndGame("Time Out");

        return;
    }

    UpdateTimeText();
}

    // =====================================================
    // START TIMER
    // =====================================================

public void StartTimer()
{
    gameEnded = false;
    isTimerRunning = true;

    elapsedPlayTime = 0f;

    ResultData = new GameResultData();

    if (timeText != null)
        timeText.text = "02:00";

    if (pauseB != null)
        pauseB.gameObject.SetActive(true);

    if (pauseGameUI != null)
        pauseGameUI.SetActive(false);

    if (resultGameUI != null)
        resultGameUI.SetActive(false);

    Time.timeScale = 1f;

    if (playerController != null)
        playerController.SetCanMove(true);

    UpdateWaveUI();

    Debug.Log(
        "CoreGameUI | Game bắt đầu. Timer = 120 giây"
    );
}



public void EndGame(string reason)
{
    if (gameEnded)
        return;

    // =========================================
    // GAME END
    // =========================================

    gameEnded = true;
    isTimerRunning = false;

    Time.timeScale = 1f;

    // =========================================
    // SNAPSHOT TIME
    // =========================================

    float finalPlayTime =
        Mathf.Clamp(
            elapsedPlayTime,
            0f,
            GAME_DURATION
        );

    // =========================================
    // SNAPSHOT WAVE
    // =========================================

    int finalWave =
        waveManager != null
            ? waveManager.GetCurrentWave()
            : 1;

    // =========================================
    // SNAPSHOT KILL
    // =========================================

    int finalKills =
        waveManager != null
            ? waveManager.GetPlayerKillCount()
            : 0;

    // =========================================
    // SAVE RESULT
    // =========================================

    ResultData = new GameResultData
    {
        playTime = finalPlayTime,
        survivedWave = finalWave,
        enemyKilled = finalKills
    };

    Debug.Log(
        $"GAME END | " +
        $"Reason = {reason} | " +
        $"Time = {finalPlayTime:F2}s | " +
        $"Wave = {finalWave} | " +
        $"Kills = {finalKills}"
    );

    // =========================================
    // STOP ENEMY
    // =========================================

    if (waveManager != null)
        waveManager.GameOver();

    // =========================================
    // RESULT UI
    // =========================================

    if (resultGameUI != null)
        resultGameUI.SetActive(true);

    // =========================================
    // UPDATE RESULT UI
    // =========================================

    ResultGameUI resultUI =
        resultGameUI != null
            ? resultGameUI.GetComponent<ResultGameUI>()
            : null;

    if (resultUI != null)
    {
        resultUI.ShowResult(ResultData);
    }

    // =========================================
    // PAUSE BUTTON
    // =========================================

    if (pauseB != null)
        pauseB.gameObject.SetActive(false);

    Debug.Log(
        "CoreGameUI | gameEnd = TRUE"
    );
}
    public void StopTimer()
    {
        isTimerRunning = false;
    }
}