using System;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public Action<PlayerProgress> OnLevelChanged;
    public Action<PlayerProgress> OnExpChanged;
    public Action<PlayerProgress> OnCoinChanged;

    public int MaxLevel => maxLevel;

    [Header("Level")]
    [SerializeField] private int maxLevel = 20;

    [SerializeField] private int currentLevel = 1;

    [SerializeField] private int currentExp;

    private int requiredExp;

    [Header("Coin")]
    [SerializeField] private int currentCoins = 0;

    private void Awake()
    {

        currentCoins = 0;
        requiredExp = CalculateRequiredExp(currentLevel);
        
        OnExpChanged?.Invoke(this);
        OnLevelChanged?.Invoke(this);
        OnCoinChanged?.Invoke(this);
    }

    // =====================================================
    // EXP
    // =====================================================

    public void AddExp(int amount)
    {
        if (amount <= 0)
            return;

        if (currentLevel >= maxLevel)
            return;

        currentExp += amount;

        while (
            currentExp >= requiredExp &&
            currentLevel < maxLevel
        )
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

        requiredExp =
            CalculateRequiredExp(currentLevel);

        Debug.Log(
            $"LEVEL UP -> {currentLevel}"
        );
    }

    private int CalculateRequiredExp(int level)
    {
        return Mathf.RoundToInt(
            200f * Mathf.Pow(level, 1.35f)
        );
    }

    // =====================================================
    // COIN
    // =====================================================

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        currentCoins += amount;

        OnCoinChanged?.Invoke(this);

        Debug.Log(
            $"PLAYER COIN +{amount} | TOTAL = {currentCoins}"
        );
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0)
            return false;

        if (currentCoins < amount)
            return false;

        currentCoins -= amount;

        OnCoinChanged?.Invoke(this);

        Debug.Log(
            $"PLAYER COIN -{amount} | TOTAL = {currentCoins}"
        );

        return true;
    }

    // =====================================================
    // GET
    // =====================================================

    public int CurrentLevel => currentLevel;

    public int CurrentExp => currentExp;

    public int RequiredExp => requiredExp;

    public int CurrentCoins => currentCoins;
}