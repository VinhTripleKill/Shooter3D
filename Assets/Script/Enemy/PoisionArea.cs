using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    [Header("Poison Settings")]
    [SerializeField] private float damagePerTick = 2f;
    [SerializeField] private float tickInterval = 2f;     // mỗi 2 giây gây damage 1 lần

    private float timer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player vào vùng Poison");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            timer = 0f;

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damagePerTick);
                Debug.Log($"Poison Area gây {damagePerTick} damage cho Player");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0f;
            Debug.Log("Player rời vùng Poison");
        }
    }
}