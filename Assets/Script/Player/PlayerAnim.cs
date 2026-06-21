using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetHoldGun(bool value)
    {
        animator.SetLayerWeight(
            1,
            value ? 1f : 0f
        );
    }
}