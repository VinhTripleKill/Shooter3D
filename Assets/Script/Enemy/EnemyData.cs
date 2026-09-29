using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "Dynher/Enemy Data"
)]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;

    public Sprite icon;

    [TextArea]
    public string descriptionEnemy;

    [Header("Prefab")]
    [Tooltip("Prefab Enemy sẽ được EnemyWaveSpawn sử dụng để Spawn.")]
    public GameObject enemyPrefab;


    // =========================================================
    // BASE STATS
    // =========================================================

    [Header("Base Stats")]

    [Min(0f)]
    public float maxHp = 100f;

    [Min(0f)]
    public float moveSpeed = 3.5f;

    [Min(0f)]
    public float atk = 10f;

    [Min(0f)]
    public float defense = 0f;


    // =========================================================
    // ENEMY ATTACK
    // =========================================================

    [Header("Enemy Attack")]

    [Min(0f)]
    public float enemyRanged = 4f;

    [Min(0f)]
    public float atkCD = 2f;


    // =========================================================
    // REWARD
    // =========================================================

    [Header("Reward")]

    [Min(0)]
    public int expReward = 200;

    [Min(0)]
    public int coinReward = 1;

    [Tooltip("Coin Prefab được spawn khi Player tiêu diệt Enemy.")]
    public GameObject coinPrefab;
}