using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public Action<PlayerProgress> OnLevelChanged;
    public Action<PlayerProgress> OnExpChanged;
    public int MaxLevel => maxLevel;
    [Header("Level")]
    [SerializeField] private int maxLevel = 20;

    [SerializeField] private int currentLevel = 1;

    [SerializeField] private int currentExp;

    private int requiredExp;

    private void Awake()
    {
        requiredExp = CalculateRequiredExp(currentLevel);

        OnExpChanged?.Invoke(this);
        OnLevelChanged?.Invoke(this);
    }

    public void AddExp(int amount)
    {
        if (currentLevel >= maxLevel) return;

        currentExp += amount;

        while (currentExp >= requiredExp && currentLevel < maxLevel)
        {
            currentExp -= requiredExp;

            LevelUp();
        }

        OnExpChanged?.Invoke(this);
    }

    private void LevelUp()
{
    currentLevel++;

    OnLevelChanged?.Invoke(this);

    if (currentLevel >= maxLevel)
    {
        currentExp = 0;
        requiredExp = 0;

        OnExpChanged?.Invoke(this);

        Debug.Log("MAX LEVEL");
        return;
    }

    requiredExp = CalculateRequiredExp(currentLevel);

    Debug.Log($"LEVEL UP -> {currentLevel}");
}

    private int CalculateRequiredExp(int level)
    {
        return Mathf.RoundToInt( 200f * Mathf.Pow(level, 1.35f));
    }

    public int CurrentLevel => currentLevel;

    public int CurrentExp => currentExp;

    public int RequiredExp => requiredExp;
}