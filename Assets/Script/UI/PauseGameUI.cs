using UnityEngine;
using UnityEngine.UI;

public class PauseGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button pauseB;
    [SerializeField] private Button resumeB;
    [SerializeField] private Button replayB;
    [SerializeField] private Button exitB;

    [Header("References")]
    [SerializeField] private CoreGameUI coreGameUI;
    [SerializeField] private PlayerSpawn playerSpawn;

    [SerializeField] private GameObject pauseGameUI;

    private void Start()
    {
        if (resumeB != null)
        {
            resumeB.onClick.AddListener(
                OnResumeClicked
            );
        }

        if (replayB != null)
        {
            replayB.onClick.AddListener(
                OnReplayClicked
            );
        }

        if (exitB != null)
        {
            exitB.onClick.AddListener(
                OnExitClicked
            );
        }
    }

    private void OnDestroy()
    {
        if (resumeB != null)
        {
            resumeB.onClick.RemoveListener(
                OnResumeClicked
            );
        }

        if (replayB != null)
        {
            replayB.onClick.RemoveListener(
                OnReplayClicked
            );
        }

        if (exitB != null)
        {
            exitB.onClick.RemoveListener(
                OnExitClicked
            );
        }
    }

    // ============================
    // RESUME
    // ============================

    private void OnResumeClicked()
    {
        if (coreGameUI != null)
        {
            coreGameUI.ResumeGame();
        }
    }

    // ============================
    // REPLAY
    // ============================

    private void OnReplayClicked()
    {
        Debug.Log("REPLAY BUTTON CLICKED");

        // Tắt Pause UI
        if (pauseGameUI != null)
        {
            pauseGameUI.SetActive(false);
            pauseB.gameObject.SetActive(true);
        }

        // Replay với character + skill đã chọn
        if (playerSpawn != null)
        {
            playerSpawn.ReplayGame();
        }
    }

    // ============================
    // EXIT
    // ============================

    private void OnExitClicked()
    {
        Debug.Log("EXIT BUTTON CLICKED");

        // Tắt Pause UI
        if (pauseGameUI != null)
        {
            pauseGameUI.SetActive(false);
        }

        // Đưa game về trạng thái bình thường
        Time.timeScale = 1f;

        // Bạn có thể thêm logic chuyển scene
        // hoặc mở lại panel chọn character ở đây.
    }
}