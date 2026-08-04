using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGun : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text gunName;
    [SerializeField] private Button button;

    [Header("State")]
    [SerializeField] private GameObject notSelect;

    private GunData data;

    public GunData Data => data;

    public void Initialize(GunData gunData, System.Action<ItemGun> onClick)
    {
        data = gunData;

        icon.sprite = data.icon;
        gunName.text = data.gunName;

        SetSelected(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke(this);
        });
    }

    public void SetSelected(bool selected)
    {
        if (notSelect != null)
            notSelect.SetActive(!selected);
    }
}