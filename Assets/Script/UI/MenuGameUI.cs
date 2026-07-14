using UnityEngine;
using UnityEngine.UI;

public class MenuGameUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button ExitGameB;

    private void Start()
    {
        if (ExitGameB != null)
        {
            ExitGameB.onClick.AddListener(ExitGame);
        }
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        // Thoát Play Mode khi đang chạy trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Thoát game khi đã build
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (ExitGameB != null)
        {
            ExitGameB.onClick.RemoveListener(ExitGame);
        }
    }
}