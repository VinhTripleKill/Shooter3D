using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : BaseCharacter, IAutoAimTarget
{
    protected EnemyCharacter enemyCharacter;

public EnemyData EnemyData =>
    enemyCharacter != null
        ? enemyCharacter.Data
        : null;
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

    enemyCharacter =
        GetComponent<EnemyCharacter>();

    enemyColliders =
        GetComponentsInChildren<Collider>();

    enemyAnim =
        GetComponentInChildren<EnemyAnim>();
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
    public void SelfDestruct()
{
    // Bomber đã chết rồi thì không xử lý lần nữa
    if (isDead)
        return;

    // =====================================================
    // ĐÁNH DẤU ĐÃ CHẾT
    // =====================================================

    isDead = true;

    StopAllCoroutines();

    DisableCollision();

    // =====================================================
    // KHÔNG REWARD
    // KHÔNG PLAYER KILL
    // =====================================================

    Debug.Log(
        $"[{name}] Bomber tự phát nổ -> Không EXP / COIN / Player Kill"
    );

    // =====================================================
    // WAVE MANAGER
    // =====================================================
    //
    // Bomber đã chết khỏi wave,
    // nhưng không tính là Player Kill.
    //
    // =====================================================

    waveManager?.OnEnemyDied(this);

    // =====================================================
    // REMOVE TARGET
    // =====================================================

    AllEnemies.Remove(this);

    AutoAimManager.Unregister(this);

    // =====================================================
    // DEAD ANIMATION
    // =====================================================
    //
    // Explosion damage KHÔNG gọi ở đây.
    //
    // Animation Event:
    //
    // EnemyAnim.ExplosionHitEvent()
    //
    // sẽ gọi:
    //
    // EnemyBomber.ExplosionHitEvent()
    //
    // =====================================================

    if (enemyAnim != null)
    {
        enemyAnim.PlayDead(
            () => Destroy(gameObject)
        );
    }
    else
    {
        Destroy(gameObject);
    }
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
    EnemyData data = EnemyData;

    if (data == null)
    {
        Debug.LogWarning(
            $"[{name}] Không có EnemyData -> Không thể nhận Reward."
        );

        return;
    }


    // =========================================================
    // EXP
    // =========================================================

    if (data.expReward > 0)
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerProgress pp =
                playerObject.GetComponent<PlayerProgress>();

            if (pp != null)
            {
                pp.AddExp(
                    data.expReward
                );
            }
        }
    }


    // =========================================================
    // COIN
    // =========================================================

    SpawnCoinReward(data);
}

protected virtual void SpawnCoinReward(
    EnemyData data
)
{
    if (data == null)
        return;


    if (data.coinPrefab == null)
    {
        Debug.LogWarning(
            $"[{name}] EnemyData chưa có Coin Prefab."
        );

        return;
    }


    if (data.coinReward <= 0)
        return;


    GameObject coinObject =
        Instantiate(
            data.coinPrefab,
            transform.position,
            Quaternion.identity
        );


    ItemCoin coin =
        coinObject.GetComponent<ItemCoin>();


    if (coin == null)
    {
        Debug.LogError(
            $"[{name}] Coin Prefab trong EnemyData " +
            $"không có ItemCoin."
        );

        Destroy(coinObject);

        return;
    }


    coin.SetCoinValue(
        data.coinReward
    );
}




}