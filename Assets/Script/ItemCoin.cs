using UnityEngine;

public class ItemCoin : ItemBase
{
    public enum Mode
    {
        Passive,
        ActivelyChase
    }

    [Header("Coin Value")]
    [SerializeField] private int coinValue = 1;

    [Header("Coin Mode")]
    [SerializeField] private Mode mode = Mode.Passive;

    [Header("Active Chase")]
    [SerializeField] private float rangeChasePlayer = 10f;
    [SerializeField] private float chaseSpeed = 8f;
    [SerializeField] private float checkTargetInterval = 0.1f;

    [Header("Visual")]
    [SerializeField] private CoinVisualAnim coinVisualAnim;

    private Transform currentTarget;

    private float targetCheckTimer;

    private bool isChasing;

    protected override void Awake()
    {
        base.Awake();

        if (coinVisualAnim == null)
        {
            coinVisualAnim =
                GetComponentInChildren<CoinVisualAnim>();
        }
    }

    // =====================================================
    // SET COIN VALUE
    // =====================================================

    public void SetCoinValue(int value)
    {
        coinValue = Mathf.Max(0, value);
    }

    public int GetCoinValue()
    {
        return coinValue;
    }

    // =====================================================
    // UPDATE
    // =====================================================

    protected override void Update()
    {
        if (!isStopped)
            return;

        // =================================================
        // PASSIVE
        // =================================================

        if (mode == Mode.Passive)
        {
            // Không gọi base.Update()
            // CoinVisualAnim tự xử lý visual.
            return;
        }

        // =================================================
        // ACTIVELY CHASE
        // =================================================

        if (isChasing && currentTarget != null)
        {
            ChaseTarget();
            return;
        }

        // =================================================
        // CHECK PLAYER
        // =================================================

        targetCheckTimer -= Time.deltaTime;

        if (targetCheckTimer <= 0f)
        {
            targetCheckTimer =
                checkTargetInterval;

            FindTarget();
        }
    }

    // =====================================================
    // LAND
    // =====================================================

    protected override void OnLanded(RaycastHit hit)
    {
        base.OnLanded(hit);

        if (mode != Mode.ActivelyChase)
            return;

        targetCheckTimer = 0f;

        currentTarget = null;

        isChasing = false;
    }

    // =====================================================
    // FIND PLAYER
    // =====================================================

    private void FindTarget()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                rangeChasePlayer,
                ~0,
                QueryTriggerInteraction.Collide
            );

        if (hits == null || hits.Length == 0)
            return;

        float closestDistanceSqr =
            float.MaxValue;

        PlayerController closestPlayer = null;

        foreach (Collider hit in hits)
        {
            PlayerController player =
                hit.GetComponentInParent<PlayerController>();

            if (player == null)
                continue;

            // Không chase Player đã chết.
            if (player.IsDead())
                continue;

            float distanceSqr =
                (player.transform.position -
                 transform.position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr =
                    distanceSqr;

                closestPlayer = player;
            }
        }

        if (closestPlayer == null)
            return;

        currentTarget =
            closestPlayer.transform;

        isChasing = true;
    }

    // =====================================================
    // CHASE PLAYER
    // =====================================================

    private void ChaseTarget()
    {
        if (currentTarget == null)
        {
            isChasing = false;
            return;
        }

        Vector3 direction =
            currentTarget.position -
            transform.position;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        transform.position +=
            direction.normalized *
            chaseSpeed *
            Time.deltaTime;
    }

    // =====================================================
    // COLLECT
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        if (player.IsDead())
            return;

        // ActiveChase:
        // chỉ collect đúng Player đang được target.
        if (mode == Mode.ActivelyChase)
        {
            if (currentTarget != null &&
                player.transform != currentTarget)
            {
                return;
            }
        }

        PlayerProgress progress =
            player.GetComponent<PlayerProgress>();

        if (progress != null)
        {
            progress.AddCoins(coinValue);
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            Vector3.down * rayLength
        );

        if (mode == Mode.ActivelyChase)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawWireSphere(
                transform.position,
                rangeChasePlayer
            );
        }
    }

#endif
}