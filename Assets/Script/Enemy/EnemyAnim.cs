using System;
using System.Collections;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public event Action OnAttackHit;

    public event Action OnExplosionHit;
    public event Action OnEnemyShoot;
    // =========================================================
    // MOVEMENT
    // =========================================================
    public void EnemyShootEvent()
    {
        OnEnemyShoot?.Invoke();
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat(
            "SpeedMagnitude",
            speed
        );
    }

    // =========================================================
    // ATTACK
    // =========================================================

    public void PlayAttack()
    {
        animator.SetTrigger(
            "isAttack"
        );
    }

    // =========================================================
    // DEAD
    // =========================================================

    public void PlayDead(Action onFinished)
    {
        animator.SetTrigger(
            "isDead"
        );

        StartCoroutine(
            WaitDeathAnim(onFinished)
        );
    }

    private IEnumerator WaitDeathAnim(
        Action onFinished)
    {
        yield return new WaitForSeconds(2f);

        onFinished?.Invoke();
    }

    // =========================================================
    // MELEE ANIMATION EVENT
    // =========================================================

    public void AttackHitEvent()
    {
        OnAttackHit?.Invoke();
    }

    // =========================================================
    // BOMBER ANIMATION EVENT
    // =========================================================
    //
    // Gắn event này vào frame đầu tiên của isDead.
    //
    // =========================================================

    public void ExplosionHitEvent()
    {
        OnExplosionHit?.Invoke();
    }
}