using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private DamageVisual damagePrefab;
    [SerializeField] private Transform popupPoint;
    [SerializeField] private Vector3 baseOffset = new Vector3(0, 1.8f, 0);

    [Header("Random Offset")]
    [SerializeField] private float randomX = 0.6f;
    [SerializeField] private float randomY = 0.4f;
    [SerializeField] private float randomZ = 0.6f;

    private BaseCharacter owner;

    private void Awake()
    {
        owner = GetComponent<BaseCharacter>();

        if (popupPoint == null)
            popupPoint = transform;
    }

    private void OnEnable()
    {
        if (owner != null)
            owner.OnTakeDamage += SpawnDamage;
    }

    private void OnDisable()
    {
        if (owner != null)
            owner.OnTakeDamage -= SpawnDamage;
    }

    private void SpawnDamage(float damage)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-randomX, randomX),
            Random.Range(-randomY, randomY),
            Random.Range(-randomZ, randomZ)
        );

        Vector3 spawnPos = popupPoint.position + baseOffset + randomOffset;

        DamageVisual popup = Instantiate(damagePrefab, spawnPos, Quaternion.identity);
        popup.Initialize(damage);
    }
}