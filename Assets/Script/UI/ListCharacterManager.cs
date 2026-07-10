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

    public CharacterData CurrentCharacter => currentSelectedItem == null ? null : currentSelectedItem.Data;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        Clear();

        foreach (CharacterData data in database.Characters)
        {
            ItemCharacterUI item = Instantiate(itemPrefab, content);

            item.Initialize(data, OnCharacterSelected);

            items.Add(item);
        }

        if (items.Count > 0)
        {
            OnCharacterSelected(items[0]);
        }
    }

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

    private void OnCharacterSelected(ItemCharacterUI item)
    {
        if (currentSelectedItem == item) return;
    
        if (currentSelectedItem != null)
            currentSelectedItem.SetSelected(false);
    
        currentSelectedItem = item;
    
        currentSelectedItem.SetSelected(true);
    
        visualCharacterInfo?.ShowCharacter(item.Data);
    
        Debug.Log($"Selected : {item.Data.characterName}");
    }
}