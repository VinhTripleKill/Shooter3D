using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class HpBarFollow : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image hpBar;
    [SerializeField] private RectTransform hpHolder;
    public enum HpBarMode
    {
        Hidden,
        Visible,
        Fade,
        AlwaysVisibleAfterAttack,
    }
    private BaseCharacter owner;
    private Camera mainCam;
    [Header("Mode")]
    [SerializeField] private HpBarMode hpBarMode = HpBarMode.Visible;

    [SerializeField]
    private float timeFade = 1f;
    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;
    private void Awake()
    {
        mainCam = Camera.main;

        owner = GetComponentInParent<BaseCharacter>();
        canvasGroup = hpHolder.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = hpHolder.gameObject.AddComponent<CanvasGroup>();
        switch (hpBarMode)
        {
            case HpBarMode.Hidden:
            case HpBarMode.Fade:
            case HpBarMode.AlwaysVisibleAfterAttack:

                hpHolder.gameObject.SetActive(false);
                canvasGroup.alpha = 1f;

                break;

            case HpBarMode.Visible:

                hpHolder.gameObject.SetActive(true);
                canvasGroup.alpha = 1f;

                break;
        }
        if (owner == null)
            return;

        owner.OnHpChanged += UpdateHp;
        owner.OnTakeDamage += HandleTakeDamage;
        UpdateHp(
            owner.GetCurrentHp(),
            owner.GetMaxHp());
    }

    private void LateUpdate()
    {
        if (mainCam == null || hpHolder == null)
            return;

        hpHolder.forward = mainCam.transform.forward;
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.OnHpChanged -= UpdateHp;
            owner.OnTakeDamage -= HandleTakeDamage;
        }
    }

    private void UpdateHp(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
    }
    private void HandleTakeDamage(float damage)
    {
        switch (hpBarMode)
        {
            case HpBarMode.Hidden:
                break;

            case HpBarMode.Visible:
                break;

            case HpBarMode.Fade:

                ShowTemporary();

                break;

            case HpBarMode.AlwaysVisibleAfterAttack:

                hpHolder.gameObject.SetActive(true);
                canvasGroup.alpha = 1f;

                break;
        }
    }
    private void ShowTemporary()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        hpHolder.gameObject.SetActive(true);

        canvasGroup.alpha = 1f;

        fadeRoutine = StartCoroutine(FadeRoutine());
    }
    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(timeFade);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        hpHolder.gameObject.SetActive(false);
    }
}