using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultGameUI : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button returnGameModeSceneB;
    [SerializeField] private Button replayB;

    [Header("Visual Index")]
    [SerializeField] private TextMeshProUGUI timePlayT;
    [SerializeField] private TextMeshProUGUI waveSurvivalT;
    [SerializeField] private TextMeshProUGUI enemyKilledPlayT;

    [Header("Visual Score Bonus")]
    [SerializeField] private TextMeshProUGUI timePlayScoreT;
    [SerializeField] private TextMeshProUGUI waveSurvivalScoreT;
    [SerializeField] private TextMeshProUGUI enemyKilledPlayScoreT;

    [Header("Visual Score Current&Highest")]
    [SerializeField] private TextMeshProUGUI currentScoreT;
    [SerializeField] private TextMeshProUGUI highestScoreT;

    private GameResultData currentResult;

    private void Start()
    {
        if (replayB != null)
            replayB.onClick.AddListener(ReplayGameWhenEnd);

        if (returnGameModeSceneB != null)
            returnGameModeSceneB.onClick.AddListener(ReturnToLobby);
    }

    private void OnDestroy()
    {
        if (replayB != null)
            replayB.onClick.RemoveListener(ReplayGameWhenEnd);

        if (returnGameModeSceneB != null)
            returnGameModeSceneB.onClick.RemoveListener(ReturnToLobby);
    }

    // =====================================================
    // SHOW RESULT
    // =====================================================

    public void ShowResult(GameResultData result)
    {
        if (result == null) return;

        currentResult = result;

        // =================================================
        // TIME
        // =================================================

        if (timePlayT != null)
        {
            int minutes = Mathf.FloorToInt(result.playTime / 60f );

            int seconds = Mathf.FloorToInt(result.playTime % 60f );

            timePlayT.text = string.Format("{0:00}:{1:00}",minutes,seconds);
        }

        // =================================================
        // WAVE
        // =================================================

        if (waveSurvivalT != null)
        {
            waveSurvivalT.text = $"{result.survivedWave}";
        }

        // =================================================
        // ENEMY KILLED
        // =================================================

        if (enemyKilledPlayT != null)
        {
            enemyKilledPlayT.text = result.enemyKilled.ToString();
        }

        Debug.Log(
            $"ResultGameUI | " +
            $"Time = {result.playTime:F2}s | " +
            $"Wave = {result.survivedWave} | " +
            $"Enemy Kill = {result.enemyKilled}"
        );
    }

    // =====================================================
    // REPLAY
    // =====================================================

    private void ReplayGameWhenEnd()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    // =====================================================
    // RETURN
    // =====================================================

    private void ReturnToLobby()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameModeListScene");
    }
}