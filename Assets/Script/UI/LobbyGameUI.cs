using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LobbyGameUI : MonoBehaviour
{
    [SerializeField] private Button BattleB;

    void Start()
    {
        if(BattleB!=null)
            BattleB.onClick.AddListener(GoToBattle);
    }
    void OnDestroy()
    {
        if(BattleB!=null)
            BattleB.onClick.RemoveListener(GoToBattle);
    }

    private void GoToBattle()
    {
        SceneManager.LoadScene("GameModeListScene");
    }
}
