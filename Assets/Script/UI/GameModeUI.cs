using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameModeUI : MonoBehaviour
{

    [SerializeField] private Button GoGameModeB;
    void Start()
    {

        if(GoGameModeB!=null)
            GoGameModeB.onClick.AddListener(GoGameMode);
    }
    void OnDestroy()
    {

        if(GoGameModeB!=null)
            GoGameModeB.onClick.RemoveListener(GoGameMode);
    }

    private void GoGameMode()
    {
        SceneManager.LoadScene("EquipmentScene");
    }
}
