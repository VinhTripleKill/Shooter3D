using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CoreGameUI : MonoBehaviour
{
    [Header("Wave Enemy")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Image waveBarProgress;

    [Header("Time")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Pause Game")]
    [SerializeField] private GameObject pauseGameUI;
    [SerializeField] private Button pauseB;

    private PlayerInput playerInput;
    private InputAction pauseGame;

    private EnemyWaveSpawn waveManager;
    private PlayerController playerController;

    private float currentTime = 120f;
    private bool isTimerRunning = false;
    private bool gameEnded = false;

    private void Start()
    {
        waveBarProgress.fillAmount = 0f;
        timeText.text = "02:00";

        if (pauseGameUI != null)
            pauseGameUI.SetActive(false);

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

        // Đảm bảo game không bị kẹt pause
        Time.timeScale = 1f;
    }

    // =====================================================
    // ĐĂNG KÝ PLAYER INPUT
    // =====================================================

    public void RegisterPlayerInput(PlayerInput input)
    {
        // Nếu trước đó đã có input cũ thì hủy đăng ký
        UnregisterPauseInput();

        playerInput = input;

        if (playerInput == null)
        {
            Debug.LogWarning("CoreGameUI: PlayerInput bị null!");
            return;
        }

        pauseGame = playerInput.actions["Pause&ResumeGame"];

        if (pauseGame == null)
        {
            Debug.LogError(
                "Không tìm thấy Action 'Pause&ResumeGame' trong Input Actions!"
            );

            return;
        }

        pauseGame.performed += OnPausePerformed;

        Debug.Log("CoreGameUI: Đã đăng ký Pause&ResumeGame");
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

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    // =====================================================
    // TOGGLE PAUSE / RESUME
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

        Debug.Log("GAME PAUSED");
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

        Debug.Log("GAME RESUMED");
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

        int currentWave = waveManager.GetCurrentWave();
        int remaining = waveManager.GetRemainingEnemiesInWave();
        int total = currentWave;

        waveText.text = $"WAVE {currentWave}";

        if (total > 0)
        {
            float progress = (float)remaining / total;
            waveBarProgress.fillAmount = progress;
        }
        else
        {
            waveBarProgress.fillAmount = 0f;
        }
    }

    public void OnWaveChanged()
    {
        UpdateWaveUI();
    }

    // =====================================================
    // TIMER
    // =====================================================

    private void UpdateTimer()
    {
        if (!isTimerRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame("Time Out");
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timeText.text = string.Format(
            "{0:00}:{1:00}",
            minutes,
            seconds
        );
    }

    public void StartTimer()
    {
        currentTime = 120f;
        isTimerRunning = true;
        timeText.text = "02:00";
    }

    private void EndGame(string reason)
{
    gameEnded = true;
    isTimerRunning = false;

    int currentWave =
        waveManager != null
        ? waveManager.GetCurrentWave()
        : 1;

    Debug.Log(
        $"Kết thúc tại wave {currentWave} - Lý do: {reason}"
    );

    // Dừng toàn bộ spawn
    if (waveManager != null)
    {
        waveManager.GameOver();
    }
}

    public void StopTimer()
    {
        isTimerRunning = false;
    }
}