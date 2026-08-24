using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameModeUI : MonoBehaviour
{
    [SerializeField] private Button returnLobbyB;
    [SerializeField] private Button returnHomeB;
    [SerializeField] private Button goGameB;
    void Start()
    {
        if(returnLobbyB!=null)
            returnLobbyB.onClick.AddListener(returnToLobby);
        if(returnHomeB!=null)
            returnHomeB.onClick.AddListener(returnToLobby);
        if(goGameB!=null)
            goGameB.onClick.AddListener(goGame);
    }
    void OnDestroy()
    {
        if(returnLobbyB!=null)
            returnLobbyB.onClick.RemoveListener(returnToLobby);
        if(returnHomeB!=null)
            returnHomeB.onClick.RemoveListener(returnToLobby);
        if(goGameB!=null)
            goGameB.onClick.RemoveListener(goGame);
    }

    private void returnToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    private void goGame()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
