using System.Collections.Generic;
using UnityEngine;

public class TargetTraining : MonoBehaviour, IDamageable, IAutoAimTarget
{
    public static List<TargetTraining> AllTargets =
        new();

    public enum TargetMode
    {
        Dummy,
        Move
    }

    [Header("Mode")]
    [SerializeField] private TargetMode targetMode;

    [Header("HP")]
    [SerializeField] private float maxHp = 50f;

    [SerializeField]private float currentHp;

    [Header("Move")]
    [SerializeField] private Transform posA;

    [SerializeField] private Transform posB;

    [SerializeField] private float speedMove = 10f;

    [SerializeField] private float timeStop = 2f;

    private Transform currentTargetPos;

    private float stopTimer;

    private bool isWaiting;

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

    private void Awake()
    {
        currentHp = maxHp;
    }

    private void Start()
    {
        if (posA == null || posB == null)
            return;

        // Bắt đầu ở giữa A và B
        transform.position =
            (posA.position + posB.position) * 0.5f;

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
        if (posA == null || posB == null)
            return;

        if (isWaiting)
        {
            stopTimer -= Time.deltaTime;

            if (stopTimer <= 0)
            {
                isWaiting = false;

                currentTargetPos =
                    currentTargetPos == posA ?
                    posB :
                    posA;
            }

            return;
        }

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                currentTargetPos.position,
                speedMove * Time.deltaTime);

        if (Vector3.Distance(
            transform.position,
            currentTargetPos.position) < 0.01f)
        {
            transform.position =
                currentTargetPos.position;

            isWaiting = true;
            stopTimer = timeStop;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        Debug.Log($"Target took {damage} damage. Current HP: {currentHp}");
        if (currentHp <= 0)
        {
            Debug.Log("Target die");
            currentHp = maxHp;

            // Có thể thêm effect hoặc reset ở đây
        }
    }
    public Transform GetTargetTransform()
    {
        return transform;
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }
}