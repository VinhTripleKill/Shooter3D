using UnityEngine;

public class GrenadeSkillBehaviour : SkillBehaviour
{
    [SerializeField]
    private GrenadeSkillData skillData;

    public override bool Execute(PlayerSkill playerSkill)
{
    if (skillData == null)
        return false;

    Transform firePoint =
        playerSkill.GetFirePoint();

    Transform target =
        CombatTargetFinder.RotateToNearestTarget(
            playerSkill.transform,
            playerSkill.targetRange);

    Vector3 direction =
        target != null
        ? (target.position - firePoint.position).normalized
        : playerSkill.transform.forward;

    GameObject grenade =
        Instantiate(
            skillData.grenadePrefab,
            firePoint.position,
            Quaternion.identity);

    Rigidbody rb =
        grenade.GetComponent<Rigidbody>();

    if (rb == null)
        return false;

    rb.AddForce(
        direction * skillData.throwForce,
        ForceMode.Impulse);

    GrenadeProjectile projectile =
        grenade.GetComponent<GrenadeProjectile>();

    projectile?.Initialize(skillData);
    return true;
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