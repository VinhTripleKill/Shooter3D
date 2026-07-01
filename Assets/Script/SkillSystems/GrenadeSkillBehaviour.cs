using UnityEngine;

public class GrenadeSkillBehaviour : SkillBehaviour
{
    [SerializeField]
    private GrenadeSkillData skillData;

    public override void Execute(PlayerSkill playerSkill)
    {
        if (skillData == null)
            return;

        Transform target =
     CombatTargetFinder.RotateToNearestTarget(
         playerSkill.transform,
         playerSkill.targetRange);

        Transform firePoint =
            playerSkill.GetFirePoint();

        GameObject grenade =
            Instantiate(
                skillData.grenadePrefab,
                firePoint.position,
                Quaternion.identity);

        Rigidbody rb =
            grenade.GetComponent<Rigidbody>();

        if (rb == null)
            return;

        Vector3 direction =
            (target.position - firePoint.position).normalized;

        rb.AddForce(
            direction * skillData.throwForce,
            ForceMode.Impulse);
        GrenadeProjectile projectile =  grenade.GetComponent<GrenadeProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(skillData);
        }

    }

    public float GetCooldown()
    {
        return skillData.cooldown;
    }

    public int GetMaxStack()
    {
        return skillData.maxStack;
    }
    public override SkillData GetSkillData()
    {
        return skillData;
    }

}