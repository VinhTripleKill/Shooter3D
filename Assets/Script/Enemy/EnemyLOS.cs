
using UnityEngine;

public class EnemyLOS : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Player mục tiêu. Nếu để trống, script sẽ tự tìm object có Tag = Player.")]
    [SerializeField] private Transform target;

    [Header("LOS Settings")]
    [Tooltip("Layer được xem là vật cản Line Of Sight.")]
    [SerializeField] private LayerMask obstacleMask;

    [Tooltip("Độ cao của điểm Ray trên trục Y.")]
    [SerializeField] private float rayHeight = 0.5f;

    [Header("Blocked Combat")]
    [Tooltip("AttackRange tối thiểu khi Player nấp sau vật cản.")]
    [SerializeField] private float blockedAttackRange = 1f;

    [Tooltip("Tốc độ giảm AttackRange khi LOS bị chặn.")]
    [SerializeField] private float rangeReduceSpeed = 5f;

    [Header("Debug")]
    [SerializeField] private bool drawRay = true;

    [SerializeField] private Color clearColor = Color.green;
    [SerializeField] private Color blockedColor = Color.red;

    private EnemyAttack enemyAttack;
    private EnemyBehaviour enemyBehaviour;

    private float originalAttackRange;

    private bool isLOSBlocked;
    private bool hasStartedLOSCheck;

    public Transform Target => target;

    public bool HasLineOfSight
    {
        get
        {
            return CheckLineOfSight();
        }
    }

    public bool IsLOSBlocked => isLOSBlocked;

    public float OriginalAttackRange =>
        originalAttackRange;

    public float BlockedAttackRange =>
        blockedAttackRange;

    public bool HasStartedLOSCheck =>
        hasStartedLOSCheck;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        enemyAttack =
            GetComponent<EnemyAttack>();

        enemyBehaviour =
            GetComponent<EnemyBehaviour>();
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        FindPlayerIfNeeded();

        if (enemyAttack != null)
        {
            originalAttackRange =
                enemyAttack.AttackRange;
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (target == null)
        {
            FindPlayerIfNeeded();

            if (target == null)
                return;
        }

        if (enemyAttack == null)
            return;


        // =====================================================
        // DISTANCE
        // =====================================================

        float distance =
            Vector3.Distance(
                transform.position,
                target.position
            );


        // =====================================================
        // CHƯA ĐẾN ATTACK RANGE
        // =====================================================
        //
        // Không check LOS.
        //
        // Enemy cứ để EnemyBehaviour tự Chase.
        //
        // =====================================================

        if (!hasStartedLOSCheck)
        {
            if (distance > originalAttackRange)
            {
                return;
            }

            // Player vừa bước vào AttackRange gốc.
            hasStartedLOSCheck = true;
        }


        // =====================================================
        // CHECK LOS
        // =====================================================

        bool hasLOS =
            CheckLineOfSight();


        // =====================================================
        // LOS XANH
        // =====================================================
        //
        // Player nhìn thấy được.
        //
        // Lập tức:
        //
        // - hết trạng thái bị block
        // - restore AttackRange về giá trị Inspector
        //
        // Không MoveTowards ở đây.
        //
        // Phải restore NGAY LẬP TỨC.
        //
        // =====================================================

        if (hasLOS)
        {
            isLOSBlocked = false;

            RestoreAttackRangeImmediately();

            return;
        }


        // =====================================================
        // LOS ĐỎ
        // =====================================================

        isLOSBlocked = true;

        ReduceAttackRange();
    }


    // =========================================================
    // FIND PLAYER
    // =========================================================

    private void FindPlayerIfNeeded()
    {
        if (target != null)
            return;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target =
                playerObject.transform;
        }
    }


    // =========================================================
    // SET TARGET
    // =========================================================

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }


    // =========================================================
    // START LOS CHECK
    // =========================================================
    //
    // Cho phép EnemyBehaviour gọi để bắt đầu hệ thống LOS.
    //
    // =========================================================

    public void StartLOSCheck()
    {
        hasStartedLOSCheck = true;
    }


    // =========================================================
    // RESET LOS CHECK
    // =========================================================
    //
    // Khi Player ra khỏi AttackRange gốc,
    // có thể reset để lần sau vào range mới check lại.
    //
    // =========================================================

    public void ResetLOSCheck()
    {
        hasStartedLOSCheck = false;

        isLOSBlocked = false;

        RestoreAttackRangeImmediately();
    }


    // =========================================================
    // GET ENEMY RAY POINT
    // =========================================================

    private Vector3 GetEnemyRayPoint()
    {
        return new Vector3(
            transform.position.x,
            rayHeight,
            transform.position.z
        );
    }


    // =========================================================
    // GET TARGET RAY POINT
    // =========================================================

    private Vector3 GetTargetRayPoint()
    {
        return new Vector3(
            target.position.x,
            rayHeight,
            target.position.z
        );
    }


    // =========================================================
    // CHECK LOS
    // =========================================================

    public bool CheckLineOfSight()
    {
        if (target == null)
        {
            FindPlayerIfNeeded();

            if (target == null)
                return false;
        }

        Vector3 origin =
            GetEnemyRayPoint();

        Vector3 destination =
            GetTargetRayPoint();

        Vector3 direction =
            destination - origin;

        float distance =
            direction.magnitude;

        if (distance <= 0.001f)
            return true;

        direction.Normalize();

        return !Physics.Raycast(
            origin,
            direction,
            distance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );
    }


    // =========================================================
    // REDUCE ATTACK RANGE
    // =========================================================

    private void ReduceAttackRange()
    {
        float currentRange =
            enemyAttack.AttackRange;

        float newRange =
            Mathf.MoveTowards(
                currentRange,
                blockedAttackRange,
                rangeReduceSpeed *
                Time.deltaTime
            );

        enemyAttack.SetAttackRange(
            newRange
        );
    }


    // =========================================================
    // RESTORE ATTACK RANGE IMMEDIATELY
    // =========================================================

    private void RestoreAttackRangeImmediately()
    {
        enemyAttack.SetAttackRange(
            originalAttackRange
        );
    }


    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmos()
    {
        if (!drawRay)
            return;

        if (target == null)
        {
            FindPlayerIfNeeded();
        }

        if (target == null)
            return;


        Vector3 origin =
            GetEnemyRayPoint();

        Vector3 destination =
            GetTargetRayPoint();


        bool hasLOS =
            CheckLineOfSight();


        Gizmos.color =
            hasLOS
                ? clearColor
                : blockedColor;


        Gizmos.DrawLine(
            origin,
            destination
        );


        Gizmos.DrawSphere(
            origin,
            0.06f
        );


        Gizmos.DrawSphere(
            destination,
            0.06f
        );
    }
}

