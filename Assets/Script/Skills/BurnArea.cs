


using System.Collections;
using UnityEngine;

public class BurnArea : MonoBehaviour
{
    [Header("Burn")]
    [SerializeField] private float duration = 3f;

    [SerializeField] private float damagePerTick = 5f;

    [SerializeField] private float tickRate = 0.5f;

    private GrenadeSkillData data;

    public void Initialize(GrenadeSkillData skillData)
    {
        data = skillData;

        SphereCollider col = GetComponent<SphereCollider>();

        col.radius = data.explodeRadius;

        StartCoroutine(BurnRoutine());

        Destroy(gameObject, duration);
    }

    private IEnumerator BurnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(tickRate);

        while (true)
        {
            DealDamage();

            yield return wait;
        }
    }

    private void DealDamage()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                data.explodeRadius,
                data.effectMask);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null) continue;

            damageable.TakeDamage(damagePerTick);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        SphereCollider col = GetComponent<SphereCollider>();

        if (col == null) return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere( transform.position, col.radius);
    }
#endif
}