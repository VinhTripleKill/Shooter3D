using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button ExitGameB;
    [SerializeField] private Button StartB;
    [SerializeField] private Button AccountB;   
    [SerializeField] private Button NotificationB;
    [Header("Touch To Start")]
    [SerializeField] private TextMeshProUGUI touchToStart;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;

    private CanvasGroup touchToStartCanvasGroup;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        gameObject.SetActive(true);

        if (StartB != null)
            StartB.onClick.AddListener(StartGame);

        if (ExitGameB != null)
            ExitGameB.onClick.AddListener(ExitGame);

        // Tạo CanvasGroup nếu Text chưa có
        if (touchToStart != null)
        {
            touchToStartCanvasGroup = touchToStart.GetComponent<CanvasGroup>();

            if (touchToStartCanvasGroup == null)
                touchToStartCanvasGroup = touchToStart.gameObject.AddComponent<CanvasGroup>();

            fadeCoroutine = StartCoroutine(FadeTouchToStart());
        }
    }

    private IEnumerator FadeTouchToStart()
    {
        while (true)
        {
            // Fade từ rõ -> mờ
            yield return StartCoroutine(FadeAlpha(maxAlpha, minAlpha));

            // Fade từ mờ -> rõ
            yield return StartCoroutine(FadeAlpha(minAlpha, maxAlpha));
        }
    }

    private IEnumerator FadeAlpha(float startAlpha, float targetAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / fadeDuration
            );

            touchToStartCanvasGroup.alpha = alpha;

            yield return null;
        }

        touchToStartCanvasGroup.alpha = targetAlpha;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("LobbyScene");
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

        if (StartB != null)
            StartB.onClick.RemoveListener(StartGame);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
    }
}