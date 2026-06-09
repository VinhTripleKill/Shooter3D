using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 distanceFromPlayer;

    private void Start()
    {
        // Lưu khoảng cách ban đầu giữa camera và player
        distanceFromPlayer = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Camera luôn giữ nguyên khoảng cách với player
        transform.position = target.position + distanceFromPlayer;
    }
}