using UnityEngine;
using UnityEngine.UI;

public class HpBarFollow : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image hpBar;
    [SerializeField] private RectTransform hpHolder;

    private BaseCharacter owner;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        owner = GetComponentInParent<BaseCharacter>();

        if (owner == null)
            return;

        owner.OnHpChanged += UpdateHp;

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
            owner.OnHpChanged -= UpdateHp;
    }

    private void UpdateHp(
        float currentHp,
        float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
    }
}