using UnityEngine;
using TMPro;

public class ItemNameVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;

    private void Awake()
    {
        if (itemNameText == null)
        {
            itemNameText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void SetItemName(string name)
    {
        if (itemNameText != null)
            itemNameText.text = name;
        else
            Debug.LogWarning("Không tìm thấy TextMeshProUGUI trên ItemNameCanvas", gameObject);
    }
}