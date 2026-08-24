using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultGameUI : MonoBehaviour
{
    [SerializeField] private Button returnGameModeSceneB;
    [SerializeField] private Button replayB;
    [SerializeField] private PlayerSpawn playerSpawn;

    void Start()
    {
        if (replayB != null)
            replayB.onClick.AddListener(ReplayGameWhenEnd);

        if (returnGameModeSceneB != null)
            returnGameModeSceneB.onClick.AddListener(ReturnToLobby);
    }

    void OnDestroy()
    {
        if (replayB != null)
            replayB.onClick.RemoveListener(ReplayGameWhenEnd);

        if (returnGameModeSceneB != null)
            returnGameModeSceneB.onClick.RemoveListener(ReturnToLobby);
    }

    private void ReplayGameWhenEnd()
    {
        if (playerSpawn != null)
            playerSpawn.ReplayGame();

        gameObject.SetActive(false);
    }

    private void ReturnToLobby()
    {
        SceneManager.LoadScene("GameModeListScene");
    }
}