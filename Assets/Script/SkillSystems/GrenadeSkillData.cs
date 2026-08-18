using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Grenade Skill")]
public class GrenadeSkillData : SkillData
{
    [Header("Grenade")]
    public GameObject grenadePrefab;

    [Header("Explosion")]
    public GameObject explosionPrefab;

    [Header("Movement")]
    [Tooltip("Tốc độ di chuyển dọc theo parabola. Tăng giá trị sẽ làm grenade bay nhanh hơn.")]
    public float moveSpeed = 8f;

    [Tooltip("Khoảng cách tối thiểu để xem là đã tới target.")]
    public float arriveDistance = 0.01f;

    [Header("Parabola")]
    [Tooltip("Độ cao parabola khi khoảng cách rất gần.")]
    public float arcHeightNear = 0.2f;

    [Tooltip("Độ cao parabola khi đạt khoảng cách tối đa.")]
    public float arcHeightFar = 4f;

    [Tooltip("Khoảng cách để đạt arcHeightFar.")]
    public float distanceForMaxArc = 15f;

    [Header("Rotation")]
    public float speedRotX = 30f;
    public float speedRotY = 60f;
    public float speedRotZ = 90f;

    [Header("Wall")]
    [Tooltip("Layer mà grenade được phép va vào và phản lại.")]
    public LayerMask wallMask;

    [Tooltip("Hệ số tốc độ sau khi phản tường.")]
    public float wallBounceForce = 1f;

    [Tooltip("Lực hướng lên sau khi phản tường.")]
    public float wallUpwardForce = 0.25f;

    [Tooltip("Tốc độ phản tối thiểu.")]
    public float minimumBounceSpeed = 1f;

    [Header("Ground / Target")]
    [Tooltip(
        "Layer dùng để tìm mặt đất tại vị trí target. " +
        "Target sẽ raycast thẳng xuống layer này."
    )]
    public LayerMask groundMask;

    [Header("Explosion")]
    [Tooltip(
        "Sau khoảng thời gian này grenade sẽ tự phát nổ " +
        "nếu chưa nổ trước đó."
    )]
    public float explodeDelay = 3f;

    [Header("Explosion Damage")]
    public float explodeRadius = 3f;

    public float damage = 20f;

    [Tooltip("Layer của các sinh vật nhận damage.")]
    public LayerMask effectMask;
}