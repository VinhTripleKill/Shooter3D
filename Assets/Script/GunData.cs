using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public string gunName;
    public enum GunFireType
{
    Raycast,
    Projectile
}

[Header("Fire Type")]
public GunFireType fireType;
    [Header("Raycast")]
    public float raycastDistance = 100f;
    [Tooltip("Bán kính hit của SphereCast")]
    public float hitRadius = 0.5f;
    [Tooltip("Các layer có thể bị trúng đạn")]
    public LayerMask hitMask;
    [Header("Debug")]
    public bool showRay = true;
    [Header("Visual")]
    public GameObject gunVisualPrefab;
    [Header("World Item")]
    public GameObject gunItemPrefab;
    [Header("Bullet")]
    public GameObject bulletPrefab;

    [Header("Stats")]
    public float damage;
    public float timeBetweenShots;
    public float bulletSpeed;
    public float bulletLifeTime;
    [Header("Magazine")]
    public int maxCountShot = 5;

    public float timeReload = 2f;
    [Header("Shotgun")]
    public int pelletCount = 1;
    [Header("Fixed Spread")]
    public float angleBetweenBullets = 0f;

    [Header("Random Spread")]
    public float randomSpreadX = 0f;
    public float randomSpreadY = 0f;
    public float randomSpreadZ = 0f;
    [Header("Recoil")]
    public float recoilForce;
}