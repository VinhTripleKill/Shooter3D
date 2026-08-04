using System.Collections.Generic;
using UnityEngine;

public class ListGunManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private GunDatabase database;

    [SerializeField] private VisualCharacterInfo visualCharacterInfo;

    [Header("UI")]
    [SerializeField] private Transform content;

    [SerializeField] private ItemGun itemPrefab;

    private readonly List<ItemGun> items = new();

    private ItemGun currentSelectedItem;

    public GunData CurrentGun =>
        currentSelectedItem == null ? null : currentSelectedItem.Data;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        Clear();

        foreach (GunData data in database.Guns)
        {
            ItemGun item = Instantiate(itemPrefab, content);

            item.Initialize(data, OnGunSelected);

            items.Add(item);
        }

        if (items.Count > 0)
            OnGunSelected(items[0]);
    }

    private void Clear()
    {
        foreach (ItemGun item in items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        items.Clear();
        currentSelectedItem = null;
    }

    private void OnGunSelected(ItemGun item)
    {
        if (currentSelectedItem == item)
            return;

        if (currentSelectedItem != null)
            currentSelectedItem.SetSelected(false);

        currentSelectedItem = item;

        currentSelectedItem.SetSelected(true);

        visualCharacterInfo.ShowGun(item.Data);

        Debug.Log($"Selected Gun : {item.Data.gunName}");
    }
}