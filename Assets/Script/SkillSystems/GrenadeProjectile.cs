using UnityEngine;
using System.Collections;   
public class GrenadeProjectile : MonoBehaviour
{
    private GrenadeSkillData data;

    private bool exploded;

    public void Initialize(GrenadeSkillData skillData)
    {
        data = skillData;

        StartCoroutine(ExplodeRoutine());
    }

    IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds(data.explodeDelay);

        Explode();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (exploded)
            return;

        if (((1 << collision.gameObject.layer) & data.hitMask) == 0)
            return;

        Explode();
    }

    void Explode()
    {
        exploded = true;
        GameObject obj =
    Instantiate(
        data.explosionPrefab,
        transform.position,
        Quaternion.identity);

ExplosionEffect effect =
    obj.GetComponent<ExplosionEffect>();

if(effect != null)
{
    effect.Initialize(data);
}



        
        Destroy(gameObject);
    }
}