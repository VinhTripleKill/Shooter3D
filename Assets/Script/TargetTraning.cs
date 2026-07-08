using System.Collections.Generic;
using UnityEngine;

public class TargetTraining : BaseCharacter, IAutoAimTarget
{
    public static List<TargetTraining> AllTargets = new();

    public enum TargetMode
    {
        Dummy,
        Move
    }

    [Header("Mode")]
    [SerializeField] private TargetMode targetMode;
    [Header("Move")]
    [SerializeField] private Transform posA;

    [SerializeField] private Transform posB;

    [SerializeField] private float speedMove = 10f;

    [SerializeField] private float timeStop = 2f;

    private Transform currentTargetPos;

    private float stopTimer;

    private bool isWaiting;
    [Header("Revive")]
    [SerializeField] private bool canRevived = false;
    private void OnEnable()
    {
        AllTargets.Add(this);

        AutoAimManager.Register(this);
    }

    private void OnDisable()
    {
        AllTargets.Remove(this);

        AutoAimManager.Unregister(this);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (posA == null || posB == null) return;

        // Bắt đầu ở giữa A và B
        transform.position = (posA.position + posB.position) * 0.5f;

        // Đi tới A trước
        currentTargetPos = posA;
    }

    private void Update()
    {
        if (targetMode == TargetMode.Move)
        {
            UpdateMoveMode();
        }
    }

    private void UpdateMoveMode()
    {
        if (posA == null || posB == null) return;

        if (isWaiting)
        {
            stopTimer -= Time.deltaTime;

            if (stopTimer <= 0)
            {
                isWaiting = false;

                currentTargetPos = currentTargetPos == posA ? posB : posA;
            }

            return;
        }

        transform.position = Vector3.MoveTowards( transform.position, currentTargetPos.position, speedMove * Time.deltaTime);

        if (Vector3.Distance( transform.position, currentTargetPos.position) < 0.01f)
        {
            transform.position = currentTargetPos.position;

            isWaiting = true;
            stopTimer = timeStop;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        Debug.Log($"Target took {damage} damage. Current HP: {currentHp}");
    }
    public Transform GetTargetTransform()
    {
        return transform;
    }

    protected override void Die()
    {
        Debug.Log("Target Die");

        if (!canRevived)
        {
            Destroy(gameObject);
            return;
        }

        currentHp = maxHp;
        isDead = false;

        OnHpChanged?.Invoke(currentHp, maxHp);
    }
}