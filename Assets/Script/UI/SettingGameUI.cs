using UnityEngine;
using UnityEngine.UI;
public class SettingGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button ExitGameB;
    [SerializeField] private Button TermsAndPrivacyB;
    void Start()
    {
        if (ExitGameB != null)
            ExitGameB.onClick.AddListener(ExitGame);

    }
    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (ExitGameB != null)
            ExitGameB.onClick.RemoveListener(ExitGame);
    }
}
