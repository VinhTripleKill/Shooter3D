using System;
using System.Collections;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public event Action OnAttackHit;

    public void SetSpeed(float speed)
    {
        animator.SetFloat(
            "SpeedMagnitude",
            speed
        );
    }

    public void PlayAttack()
    {
        animator.SetTrigger(
            "isAttack"
        );
    }

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
    // ANIMATION EVENT
    // =========================================================

    public void AttackHitEvent()
    {
        OnAttackHit?.Invoke();
    }
}