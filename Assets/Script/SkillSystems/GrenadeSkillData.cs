using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Grenade Skill")]
public class GrenadeSkillData : SkillData
{
    [Header("Grenade")]
    public GameObject grenadePrefab;

    [Header("Explosion")]
    public GameObject explosionPrefab;

    [Header("Trajectory")]
    [Tooltip("Độ cao đỉnh quỹ đạo tăng theo khoảng cách.")]
    public float arcHeightPerMeter = 0.35f;

    [Tooltip("Độ cao tối thiểu của đỉnh quỹ đạo.")]
    public float minArcHeight = 0.5f;
    [Header("Movement")]
[Tooltip(
    "Tốc độ thời gian bay của grenade.\n" +
    "1 = tốc độ hiện tại.\n" +
    "2 = bay nhanh gấp 2 lần.\n" +
    "0.5 = bay chậm một nửa."
)]
[Min(0.01f)]
public float speedGrenadeMove = 1f;

    [Header("Collision")]
    [Tooltip(
        "Collider thuộc layer này sẽ làm grenade nổ ngay khi va chạm."
    )]
    public LayerMask hitMask;

    [Tooltip(
        "Sau khoảng thời gian này grenade sẽ tự nổ " +
        "nếu chưa chạm hitMask."
    )]
    public float explodeDelay = 3f;

    [Header("Explosion Damage")]
    public float explodeRadius = 3f;

    public float damage = 20f;

    [Tooltip("Layer của các sinh vật nhận damage.")]
    public LayerMask effectMask;
}