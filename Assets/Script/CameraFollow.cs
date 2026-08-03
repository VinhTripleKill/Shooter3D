using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private bool smoothFollow = false;
    [SerializeField] private float followSpeed = 10f;

    [Header("Fixed Camera Offset")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 12f, -7f);

    [Header("Fixed Camera Rotation")]
    [SerializeField] private Vector3 fixedCameraRotation = new Vector3(60f, 0f, 0f);

    private void LateUpdate()
    {
        if (target == null) return;

        BaseCharacter character = target.GetComponent<BaseCharacter>();

        if ( character != null && character.IsDead()) return;
        

        Vector3 targetPosition = target.position + cameraOffset;

        if (smoothFollow)
        {
            transform.position =Vector3.Lerp( transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
        }

        transform.rotation = Quaternion.Euler(fixedCameraRotation );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target == null) return;

        transform.position = target.position + cameraOffset;

        transform.rotation = Quaternion.Euler(fixedCameraRotation );
    }

    public void ClearTarget()
    {
        target = null;
    }
}