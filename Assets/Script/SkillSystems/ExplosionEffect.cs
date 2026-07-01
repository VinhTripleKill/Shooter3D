using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public enum ExplosionEffectType{Frag,Burn}
    
    [Header("Effect Type")]
    [SerializeField]
    private ExplosionEffectType effectType;

    [Header("Particle")]
    [SerializeField]
    private ParticleSystem explosionParticle;

    [Header("Burn")]
    [SerializeField]
    private BurnArea burnAreaPrefab;

    private GrenadeSkillData data;

    public void Initialize(
        GrenadeSkillData skillData)
    {
        data = skillData;

        DamageTargets();

        if (effectType == ExplosionEffectType.Burn)
        {
            BurnArea area =
                Instantiate(
                    burnAreaPrefab,
                    transform.position,
                    Quaternion.identity);

            area.Initialize(data);
        }

        PlayParticleAndDestroy();
    }

    private void DamageTargets()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                data.radius,
                data.effectMask);

        foreach (Collider hit in hits)
        {
            hit.GetComponentInParent<IDamageable>()
                ?.TakeDamage(data.damage);
        }
    }

    private void PlayParticleAndDestroy()
    {
        if (explosionParticle == null)
        {
            Destroy(gameObject);
            return;
        }

        explosionParticle.Play();

        Destroy(
            gameObject,
            explosionParticle.main.duration +
            explosionParticle.main.startLifetime.constantMax);
    }
}