using System.Collections.Generic;
using UnityEngine;

public class ListSkillManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private SkillDatabase database;

    [SerializeField] private VisualCharacterInfo visualCharacterInfo;

    [Header("UI")]
    [SerializeField] private Transform content;

    [SerializeField] private ItemSkill itemPrefab;

    private readonly List<ItemSkill> items = new();

    private ItemSkill currentSelectedItem;

    public SkillData CurrentSkill => currentSelectedItem == null ? null : currentSelectedItem.Data;

    private void Start()
    {
        Refresh();
        
    }
   
    public void Refresh()
    {
        Clear();

        foreach (SkillData data in database.Skills)
        {
            ItemSkill item = Instantiate(itemPrefab, content);

            item.Initialize(data, OnSkillSelected);

            items.Add(item);
        }

        if (items.Count > 0)
            OnSkillSelected(items[0]);
    }

    private void Clear()
    {
        foreach (ItemSkill item in items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        items.Clear();

        currentSelectedItem = null;
    }

    private void OnSkillSelected(ItemSkill item)
    {
        if (currentSelectedItem == item) return;

        if (currentSelectedItem != null)
            currentSelectedItem.SetSelected(false);

        currentSelectedItem = item;

        currentSelectedItem.SetSelected(true);

        visualCharacterInfo.ShowSkill(item.Data);

        Debug.Log($"Selected Skill : {item.Data.skillName}");
    }
}