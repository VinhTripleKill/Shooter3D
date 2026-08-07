using UnityEngine;

public class GrenadeSkillBehaviour : SkillBehaviour
{
    [SerializeField] private GrenadeSkillData skillData;
    Vector3 direction;
    public override bool Execute(PlayerSkill playerSkill)
    {
         if (skillData == null) return false;

         Transform firePoint = playerSkill.GetFirePoint();
        

    if (playerSkill.IsAutoAim())
    {
        Transform target = CombatTargetFinder.RotateToNearestTarget(playerSkill.transform,playerSkill.targetRange);
    
        if (target != null)
        {
            direction = (target.position - firePoint.position).normalized;
        }
        else
        {
            direction = playerSkill.transform.forward;
        }
    }
    else
    {
        direction = playerSkill.GetAimDirection();
    }
    
    direction.y = 0;
    
    if (direction.sqrMagnitude > 0.001f)
    {
        playerSkill.transform.forward = direction.normalized;
    }
        GameObject grenade = Instantiate(skillData.grenadePrefab,firePoint.position,Quaternion.identity);
    
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
    
        if (rb == null) return false;
    
        rb.AddForce( direction * skillData.throwForce, ForceMode.Impulse);
    
        GrenadeProjectile projectile = grenade.GetComponent<GrenadeProjectile>();
    
        projectile?.Initialize(skillData);
    
        return true;
    }

    public float GetCooldown() => skillData.cooldown;
    

    public int GetMaxStack() =>  skillData.maxStack;
    
    public override SkillData GetSkillData() => skillData;
    

}