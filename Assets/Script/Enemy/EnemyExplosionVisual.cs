using UnityEngine;
using UnityEngine.UI;

public class EnemyExplosionVisual : MonoBehaviour
{
    [Header("Explosion Visual")]
    [SerializeField] private Image warningZone;
    [SerializeField] private Image runingZone;

    private RectTransform warningRect;
    private RectTransform runingRect;

    private float explosionRadius;
    private float explosionDelay;

    public void Initialize(
        float explosionRadius,
        float explosionDelay)
    {
        this.explosionRadius = explosionRadius;
        this.explosionDelay = explosionDelay;

        warningRect =
            warningZone != null
                ? warningZone.rectTransform
                : null;

        runingRect =
            runingZone != null
                ? runingZone.rectTransform
                : null;

        SetupSize();

        Hide();
    }

    private void SetupSize()
    {
        float size =
            (explosionRadius * 100f) * 2f;

        if (warningRect != null)
        {
            warningRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                size
            );

            warningRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                size
            );
        }

        if (runingRect != null)
        {
            runingRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                size
            );

            runingRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                size
            );
        }
    }

    public void StartCountdown()
    {
        if (warningZone != null)
            warningZone.gameObject.SetActive(true);

        if (runingZone != null)
            runingZone.gameObject.SetActive(true);

        ResetRunningScale();
    }

    public void UpdateCountdown(
        float explosionTimer)
    {
        if (runingRect == null)
            return;

        if (explosionDelay <= 0f)
        {
            runingRect.localScale =
                Vector3.one;

            return;
        }

        float progress =
            explosionTimer / explosionDelay;

        progress =
            Mathf.Clamp01(progress);

        runingRect.localScale =
            Vector3.one * progress;
    }

    public void Hide()
    {
        if (warningZone != null)
            warningZone.gameObject.SetActive(false);

        if (runingZone != null)
            runingZone.gameObject.SetActive(false);

        ResetRunningScale();
    }

    private void ResetRunningScale()
    {
        if (runingRect != null)
        {
            runingRect.localScale =
                Vector3.zero;
        }
    }
}