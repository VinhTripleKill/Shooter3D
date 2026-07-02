using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public string gunName;
    public enum GunFireType{Raycast,Projectile}

    [Header("Fire Type")]
    public GunFireType fireType;
    [Header("Raycast")]
    public float raycastDistance = 100f;
    [Tooltip("Bán kính hit của SphereCast")]
    public float hitRadius = 0.5f;
    [Tooltip("Các layer có thể bị trúng đạn")]
    public LayerMask hitMask;
    [Tooltip("Các layer bị tương tác bởi đạn")]
    public LayerMask interactionMask; // nếu là các loại gun chuyên tiêu gây damage, hồi mana ta tick layer enemy, targettraning(có thể cả player vì sau này có multiplayer or pvp mode), với các gundata kiểu hỗ trợ như bắn hồi máu thì ta tick layer player

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
    [Header("Ultimate")]public float manaRecoveryByHit = 2f;
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