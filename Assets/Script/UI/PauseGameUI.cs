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
            resumeB.onClick.AddListener(OnResumeClicked);
        }

        if (replayB != null)
        {
            replayB.onClick.AddListener(OnReplayClicked);
        }

        if (exitB != null)
        {
            exitB.onClick.AddListener(OnExitClicked);
        }
    }

    private void OnDestroy()
    {
        if (resumeB != null)
        {
            resumeB.onClick.RemoveListener(OnResumeClicked);
        }

        if (replayB != null)
        {
            replayB.onClick.RemoveListener(OnReplayClicked);
        }

        if (exitB != null)
        {
            exitB.onClick.RemoveListener(OnExitClicked);
        }
    }

    private void OnResumeClicked()
    {
        if (coreGameUI != null)
        {
            coreGameUI.ResumeGame();
        }
    }
    private void OnReplayClicked()
    {
        if (pauseGameUI != null)
        {
            pauseGameUI.SetActive(false);
            pauseB.gameObject.SetActive(true);
        }
        if (playerSpawn != null)
        {
            playerSpawn.ReplayGame();
        }
    }
    private void OnExitClicked()
    {
        if (pauseGameUI != null)
        {
            pauseGameUI.SetActive(false);
        }
        Time.timeScale = 1f;
    }
}