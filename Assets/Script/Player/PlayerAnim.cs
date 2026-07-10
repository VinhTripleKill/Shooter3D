using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Cho phép gán động sau khi spawn model
    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
        Debug.Log("PlayerAnim: Animator đã được cập nhật từ model");
    }

    public void SetSpeed(float speed)
    {
        if (animator != null)
            animator.SetFloat("Speed", speed);
        else
            Debug.LogWarning("Animator chưa được gán!");
    }

    public void SetHoldGun(bool value)
    {
        if (animator != null)
            animator.SetLayerWeight(1, value ? 1f : 0f);
    }

    public void TriggerAttack()
    {
        if (animator != null)
            animator.SetTrigger("isAttack");
    }

    public void PlayDead()
    {
        if (animator != null)
        {
            animator.SetTrigger("isDead");
            SetHoldGun(false);
        }
    }
}