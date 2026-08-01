using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemBase : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float stopDistance = 0.15f;
    [SerializeField] protected float rayLength = 5f;

    [Header("Idle Effect")]
    [SerializeField] protected float rotateSpeed = 90f;      // độ/giây
    [SerializeField] protected float floatAmplitude = 0.08f; // độ cao nhấp nhô
    [SerializeField] protected float floatSpeed = 2f;        // tốc độ nhấp nhô

    protected Rigidbody rb;
    protected bool isStopped = false;
    protected Vector3 basePosition;
    protected float floatTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (isStopped)
            return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.green);

            if (hit.distance <= stopDistance)
            {
                StopOnGround(hit);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);
        }
    }

    private void Update()
    {
        if (!isStopped)
            return;

        UpdateIdleEffect();
    }

    // Có thể override để chỉ animate visual con
    protected virtual void UpdateIdleEffect()
    {
        // Xoay theo chiều kim đồng hồ quanh trục Y
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // Hiệu ứng bay lơ lửng
        floatTimer += Time.deltaTime * floatSpeed;
        Vector3 pos = basePosition;
        pos.y += Mathf.Sin(floatTimer) * floatAmplitude;
        transform.position = pos;
    }

    private void StopOnGround(RaycastHit hit)
    {
        isStopped = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        Vector3 pos = transform.position;
        pos.y = hit.point.y + stopDistance;
        transform.position = pos;

        // Lưu vị trí gốc để hiệu ứng lơ lửng
        basePosition = transform.position;

        // Để các Item không nhấp nhô cùng nhịp
        floatTimer = Random.Range(0f, Mathf.PI * 2f);

        OnLanded(hit);
    }

    // Hook cho class con (GunItem dùng để lưu startVisualPos)
    protected virtual void OnLanded(RaycastHit hit) { }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);
    }
#endif
}