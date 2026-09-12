using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : BaseCharacter, IAutoAimTarget
{

    [Header("Reward")]
    [SerializeField] protected int expReward = 200;
    [SerializeField] protected int coinReward = 1;
    [SerializeField] protected GameObject coinPrefab;


    protected EnemyAnim enemyAnim;

    private Collider[] enemyColliders;

    private EnemyWaveSpawn waveManager;

    private bool shouldGiveExp = true;


    public static List<BaseEnemy> AllEnemies { get; } = new List<BaseEnemy>();


    public void Initialize(EnemyWaveSpawn manager)
    {
        waveManager = manager;
    }


    protected override void Awake()
    {
        base.Awake();

        enemyColliders = GetComponentsInChildren<Collider>();

        enemyAnim = GetComponentInChildren<EnemyAnim>();
    }


    protected virtual void DisableCollision()
    {
        if (enemyColliders == null) return;

        foreach (Collider col in enemyColliders)
        {
            if (col != null)
                col.enabled = false;
        }
    }

    protected virtual void OnEnable()
    {
        if (!AllEnemies.Contains(this))
        {
            AllEnemies.Add(this);
        }

        AutoAimManager.Register(this);
    }


    protected virtual void OnDisable()
    {
        AllEnemies.Remove(this);

        AutoAimManager.Unregister(this);
    }


    public virtual Transform GetTargetTransform() => transform;
    
    
    public void ForceKill()
    {
        if (isDead) return;

        // Enemy bị hệ thống ép chết
        // => KHÔNG tính player kill
        // => KHÔNG nhận EXP / COIN

        shouldGiveExp = false;

        Die();
    }


    protected override void Die()
    {
        
        base.Die();

        StopAllCoroutines();

        DisableCollision();

        if (shouldGiveExp)
        {
            GiveReward();
        }
        else
        {
            Debug.Log($"[{name}] Enemy tự động chết / ForceKill " + "-> Không nhận EXP / COIN");
        }


        // =====================================================
        // PLAYER KILL COUNT
        // =====================================================

        /*
         * Enemy chết bình thường
         * => tính là Player Kill.
         *
         * ForceKill()
         * => shouldGiveExp = false
         * => không tính Player Kill.
         */

        if (shouldGiveExp)
        {
            waveManager?.RegisterPlayerKill(this);
        }



        waveManager?.OnEnemyDied(this);


        AllEnemies.Remove(this);

        AutoAimManager.Unregister(this);


        if (enemyAnim != null)
        {
            enemyAnim.PlayDead(() => Destroy(gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
    }


    protected virtual void GiveReward()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerProgress pp = playerObject.GetComponent<PlayerProgress>();

            if (pp != null)
            {
                pp.AddExp(expReward);
            }
        }


        SpawnCoinReward();
    }


    protected virtual void SpawnCoinReward()
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning($"[{name}] Chưa gán Coin Prefab.");

            return;
        }

        if (coinReward <= 0) return;


        GameObject coinObject = Instantiate( coinPrefab, transform.position, Quaternion.identity );


        ItemCoin coin = coinObject.GetComponent<ItemCoin>();


        if (coin == null)
        {
            Debug.LogError($"[{name}] Coin Prefab không có ItemCoin." );

            Destroy(coinObject);

            return;
        }


        coin.SetCoinValue(coinReward);
    }
}