using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoreGameUI : MonoBehaviour
{
    [Header("Wave Enemy")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Image waveBarProgress;

    [Header("Time")]
    [SerializeField] private TextMeshProUGUI timeText;

    private EnemyWaveSpawn waveManager;
    private PlayerController playerController;

    private float currentTime = 120f;
    private bool isTimerRunning = false;
    private bool gameEnded = false;

    private void Start()
    {
        waveBarProgress.fillAmount = 0f;   // Mặc định ban đầu
        timeText.text = "02:00";
    }

    private void Update()
    {
        if (gameEnded) return;
        UpdateTimer();
    }

    public void Initialize(EnemyWaveSpawn waveSpawnManager, PlayerController playerCtrl)
    {
        waveManager = waveSpawnManager;
        playerController = playerCtrl;

        if (waveManager != null)
            UpdateWaveUI();
    }

    private void UpdateWaveUI()
    {
        if (waveManager == null) return;

        int currentWave = waveManager.GetCurrentWave();
        int remaining = waveManager.GetRemainingEnemiesInWave();
        int total = currentWave;

        waveText.text = $"WAVE {currentWave}";

        // === PROGRESS BAR THEO YÊU CẦU ===
        if (total > 0)
        {
            float progress = (float)remaining / total;   // 2/2 = 1, 1/2 = 0.5, 0/2 = 0
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

    // ====================== TIMER ======================
    private void UpdateTimer()
    {
        if (!isTimerRunning) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame("Time Out");
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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

    int currentWave = waveManager != null ? waveManager.GetCurrentWave() : 1;
    Debug.Log($"Kết thúc tại wave {currentWave} - Lý do: {reason}. Không sang wave tiếp theo.");

    // Dừng wave spawn
    if (waveManager != null)
        waveManager.GameOver();
}

    public void StopTimer()
    {
        isTimerRunning = false;
    }
}