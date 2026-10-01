
using System.Collections;
using UnityEngine;

public class FadeObstacle : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField, Range(0f, 1f)]
    private float fadedAlpha = 0.25f;

    [SerializeField]
    private float fadeDuration = 0.2f;

    [Header("Material")]
    [SerializeField]
    private Renderer targetRenderer;

    private Material material;

    private Color originalColor;
    private float originalAlpha = 1f;

    private Coroutine fadeRoutine;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogError(
                $"FadeObstacle | Không tìm thấy Renderer trên {gameObject.name}"
            );

            return;
        }

        // Tạo material instance riêng cho Wall này.
        // Không làm thay đổi material gốc của các object khác.
        material = targetRenderer.material;

        if (material.HasProperty(BaseColorID))
        {
            originalColor = material.GetColor(BaseColorID);
        }
        else if (material.HasProperty(ColorID))
        {
            originalColor = material.GetColor(ColorID);
        }
        else
        {
            Debug.LogError(
                $"FadeObstacle | Material của {gameObject.name} không có _BaseColor hoặc _Color."
            );

            return;
        }

        originalAlpha = originalColor.a;
    }

    public void FadeOut()
    {
        if (material == null)
            return;

        StartFade(fadedAlpha);
    }

    public void FadeIn()
    {
        if (material == null)
            return;

        StartFade(originalAlpha);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(
            FadeCoroutine(targetAlpha)
        );
    }

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        Color color = GetMaterialColor();

        float startAlpha = color.a;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / Mathf.Max(fadeDuration, 0.01f);

            float alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                time
            );

            color.a = alpha;

            SetMaterialColor(color);

            yield return null;
        }

        color.a = targetAlpha;

        SetMaterialColor(color);

        fadeRoutine = null;
    }

    private Color GetMaterialColor()
    {
        if (material.HasProperty(BaseColorID))
            return material.GetColor(BaseColorID);

        if (material.HasProperty(ColorID))
            return material.GetColor(ColorID);

        return Color.white;
    }

    private void SetMaterialColor(Color color)
    {
        if (material.HasProperty(BaseColorID))
        {
            material.SetColor(BaseColorID, color);
        }
        else if (material.HasProperty(ColorID))
        {
            material.SetColor(ColorID, color);
        }
    }

    private void OnDestroy()
    {
        if (material != null)
        {
            Destroy(material);
        }
    }
}
