using System.Collections;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetWalk(bool value)
    {
        animator.SetBool("isWalk", value);
    }

    public void PlayDead(System.Action onFinished)
    {
        animator.SetTrigger("isDead");

        StartCoroutine(WaitDeathAnim(onFinished));
    }

    private IEnumerator WaitDeathAnim(System.Action onFinished)
    {
        // thời gian fallback nếu animation không có event
        float waitTime = 2f;

        yield return new WaitForSeconds(waitTime);

        onFinished?.Invoke();
    }
}