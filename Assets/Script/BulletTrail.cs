using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private float speed;

    private float distance;
    private float traveled;

    private bool initialized;

    public void Initialize(Vector3 start, Vector3 end, float bulletSpeed)
    {
        startPos = start;
        endPos = end;
        speed = bulletSpeed;

        transform.position = startPos;

        distance = Vector3.Distance(startPos, endPos);
        traveled = 0f;

        initialized = true;

        Destroy(gameObject, 5f); // safety
    }

    private void Update()
    {
        if (!initialized) return;

        Vector3 dir = (endPos - startPos).normalized;

        float move = speed * Time.deltaTime;
        traveled += move;

        transform.position += dir * move;

        if (traveled >= distance)
        {
            transform.position = endPos;
            Destroy(gameObject);
        }
    }
}