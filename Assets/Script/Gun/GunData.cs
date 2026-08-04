using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Gun Data")]
public class GunData : ScriptableObject
{
    
    public enum GunFireType{
        Raycast,
        Bullet,
        Missile
        }

    public Sprite icon;
    [Header("Info")]
    public string gunName;
    [Header("Fire Type")]
    public GunFireType fireType;
    [Header("range attack auto&aim")]
    public float rangeAttack = 15f; 
    [Tooltip("Bán kính hit của SphereCast")]
    public float hitRadius = 0.5f;
    [Tooltip("Các layer có thể bị trúng đạn")]
    public LayerMask hitMask;
    [Tooltip("Các layer bị tương tác bởi đạn")]
    public LayerMask interactionMask; 

    [Header("Debug")]
    public bool showRay = true;
    [Header("Visual")]
    public GameObject gunVisualPrefab;
    [Header("World Item")]
    public GameObject gunItemPrefab;
    
    [Header("Projectile")]
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    [Header("Stats")]
    public float damage;
    public float timeBetweenShots;
    public float bulletSpeed;
    public float missileLifeTime;
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