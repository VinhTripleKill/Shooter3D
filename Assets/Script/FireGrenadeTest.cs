using UnityEngine;
using UnityEngine.UI;

public class FireGrenadeTest : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button fireButton;

    [Header("Fire")]
    [SerializeField] private Transform firePos;

    [Header("Target")]
    [SerializeField] private LayerMask objectTarget;

    [Header("Ground")]
    [SerializeField] private LayerMask groundMaskCollider;
    [SerializeField] private float groundRayDistance = 100f;

    [Header("Projectile")]
    [SerializeField] private GameObject grenadeProjectileTest;

    private Vector3 currentTargetGroundPoint;
    private bool hasTargetGroundPoint;

    private GameObject currentTarget;

    private void Start()
    {
        if (fireButton != null)
            fireButton.onClick.AddListener(FireGrenade);

        UpdateTargetPoint();
    }

    private void Update()
    {
        UpdateTargetPoint();
    }

    private void UpdateTargetPoint()
    {
        /*
         * Tìm Target thuộc objectTarget.
         */
        currentTarget = FindTarget();

        if (currentTarget == null)
        {
            hasTargetGroundPoint = false;
            return;
        }

        /*
         * Target di chuyển thì ray này cũng di chuyển theo.
         */
        Vector3 origin =
            currentTarget.transform.position;

        if (Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            groundRayDistance,
            groundMaskCollider,
            QueryTriggerInteraction.Ignore))
        {
            currentTargetGroundPoint = hit.point;
            hasTargetGroundPoint = true;

            // Target -> Ground
            Debug.DrawLine(
                origin,
                hit.point,
                Color.yellow
            );

            // FirePos -> Target Ground
            if (firePos != null)
            {
                Debug.DrawLine(
                    firePos.position,
                    hit.point,
                    Color.red
                );
            }
        }
        else
        {
            hasTargetGroundPoint = false;
        }
    }

    private GameObject FindTarget()
    {
        /*
         * Lấy tất cả Collider thuộc objectTarget.
         */
        Collider[] targets = Physics.OverlapSphere(
            firePos != null
                ? firePos.position
                : transform.position,
            1000f,
            objectTarget,
            QueryTriggerInteraction.Ignore
        );

        if (targets.Length == 0)
            return null;

        /*
         * Lấy target đầu tiên.
         */
        return targets[0].gameObject;
    }

    public void FireGrenade()
    {
        if (firePos == null ||
            grenadeProjectileTest == null)
            return;

        if (!hasTargetGroundPoint)
        {
            Debug.LogWarning(
                "Không tìm thấy Target hoặc Ground."
            );
            return;
        }

        GameObject grenadeObject =
            Instantiate(
                grenadeProjectileTest,
                firePos.position,
                firePos.rotation
            );

        GrenadeProjectileTest projectile =
            grenadeObject.GetComponent<GrenadeProjectileTest>();

        if (projectile == null)
        {
            Debug.LogError(
                "GrenadeProjectileTest prefab thiếu component."
            );

            Destroy(grenadeObject);
            return;
        }

        /*
         * Lấy điểm Ground hiện tại tại thời điểm bấm Fire.
         */
        projectile.Initialize(
            firePos.position,
            currentTargetGroundPoint
        );

        Debug.Log(
            $"[Grenade] Target: {currentTarget.name} | " +
            $"Ground Point: {currentTargetGroundPoint}"
        );
    }

    private void OnDestroy()
    {
        if (fireButton != null)
            fireButton.onClick.RemoveListener(FireGrenade);
    }

    private void OnDrawGizmos()
    {
        if (!hasTargetGroundPoint)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(
            currentTargetGroundPoint,
            0.08f
        );
    }
}