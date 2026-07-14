using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private DamageVisual damagePrefab;
    [SerializeField] private Transform popupPoint;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

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
        DamageVisual popup =
            Instantiate(
                damagePrefab,
                popupPoint.position + offset,
                Quaternion.identity);

        popup.Initialize(damage);
    }
}