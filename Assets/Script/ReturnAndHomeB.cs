using UnityEngine;
using UnityEngine.SceneManagement;
public class ReturnAndHomeB : MonoBehaviour
{
    public void returnLobbyUI()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    public void returnGameModeUI()
    {
        SceneManager.LoadScene("GameModeListScene");
    }
}