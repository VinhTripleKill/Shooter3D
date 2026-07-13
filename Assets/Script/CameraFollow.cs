using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 12, 8); // Khoảng cách mặc định

    private void LateUpdate()
    {
        if (target == null || target.GetComponent<BaseCharacter>()?.IsDead() == true)
            return; // Giữ nguyên vị trí khi player chưa spawn hoặc đã chết

        // Follow mượt mà theo toàn bộ vị trí Player + offset
        transform.position = target.position + offset;
    }

    // Gọi khi spawn Player
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            // Force camera theo X của Player ngay lập tức (giữ Y,Z hiện tại)
            Vector3 newPos = transform.position;
            newPos.x = target.position.x;
            transform.position = newPos;

            // Cập nhật offset để follow mượt sau này
            offset = transform.position - target.position;

            Debug.Log("Camera đã lock vào Player mới");
        }
    }
}