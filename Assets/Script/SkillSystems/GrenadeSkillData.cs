using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Grenade Skill")]
public class GrenadeSkillData : SkillData
{
    [Header("Grenade")]
    public GameObject grenadePrefab;

    [Header("Explosion")]
    public GameObject explosionPrefab;

    public float explodeDelay = 2f;

    public float throwForce = 12f;

    [Header("Common Effect")]
    public float radius = 3f;

    public float damage = 20f;

    public LayerMask hitMask;

    public LayerMask effectMask;
}