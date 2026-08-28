using System;
using System.Collections.Generic;
using UnityEngine;

public class ListCharacterManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private CharacterDatabase database;
    [SerializeField] private VisualCharacterInfo visualCharacterInfo;

    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private ItemCharacterUI itemPrefab;

    private readonly List<ItemCharacterUI> items = new();

    private ItemCharacterUI currentSelectedItem;

    public CharacterData CurrentCharacter =>
        currentSelectedItem == null
            ? null
            : currentSelectedItem.Data;

    // =====================================================
    // EVENT
    // =====================================================

    public event Action<CharacterData> OnCharacterChanged;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        Refresh();
    }

    // =====================================================
    // REFRESH
    // =====================================================

    public void Refresh()
    {
        Clear();

        if (database == null)
        {
            Debug.LogError(
                "ListCharacterManager | CharacterDatabase chưa được gán!"
            );

            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError(
                "ListCharacterManager | ItemCharacterUI chưa được gán!"
            );

            return;
        }

        if (content == null)
        {
            Debug.LogError(
                "ListCharacterManager | Content chưa được gán!"
            );

            return;
        }

        foreach (CharacterData data in database.Characters)
        {
            if (data == null)
                continue;

            ItemCharacterUI item =
                Instantiate(
                    itemPrefab,
                    content
                );

            item.Initialize(
                data,
                OnCharacterSelected
            );

            items.Add(item);
        }

        // Chọn character đầu tiên
        if (items.Count > 0)
        {
            OnCharacterSelected(items[0]);
        }
    }

    // =====================================================
    // CLEAR
    // =====================================================

    private void Clear()
    {
        foreach (ItemCharacterUI item in items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        items.Clear();

        currentSelectedItem = null;
    }

    // =====================================================
    // CHARACTER SELECTED
    // =====================================================

    private void OnCharacterSelected(ItemCharacterUI item)
    {
        if (item == null)
            return;

        if (currentSelectedItem == item)
            return;

        // Bỏ selected item cũ
        if (currentSelectedItem != null)
        {
            currentSelectedItem.SetSelected(false);
        }

        // Gán item mới
        currentSelectedItem = item;

        currentSelectedItem.SetSelected(true);

        // Visual info
        visualCharacterInfo?.ShowCharacter(
            item.Data
        );

        // Thông báo cho hệ thống khác
        OnCharacterChanged?.Invoke(
            item.Data
        );

        Debug.Log(
            $"Selected Character: {item.Data.characterName}"
        );
    }
}