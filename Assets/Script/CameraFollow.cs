
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // =====================================================
    // CAMERA TARGET
    // =====================================================

    [Header("Camera Target")]
    [SerializeField] private Transform target;


    // =====================================================
    // FOLLOW SETTINGS
    // =====================================================

    [Header("Follow Settings")]
    [SerializeField] private bool smoothFollow = false;

    [SerializeField] private float followSpeed = 10f;


    // =====================================================
    // CAMERA POSITION
    // =====================================================

    [Header("Fixed Camera Offset")]
    [SerializeField]
    private Vector3 cameraOffset = new Vector3(
        0f,
        12f,
        -7f
    );


    // =====================================================
    // CAMERA ROTATION
    // =====================================================

    [Header("Fixed Camera Rotation")]
    [SerializeField]
    private Vector3 fixedCameraRotation = new Vector3(
        60f,
        0f,
        0f
    );


    // =====================================================
    // WALL FADE
    // =====================================================

    [Header("Wall Fade Detection")]
    [SerializeField]
    private LayerMask obstructionMask;

    [SerializeField]
    private bool enableWallFade = true;


    // =====================================================
    // CURRENT OBSTACLE
    // =====================================================

    private FadeObstacle currentObstacle;


    // =====================================================
    // LIFECYCLE
    // =====================================================

    private void LateUpdate()
    {
        if (target == null)
            return;

        BaseCharacter character =
            target.GetComponent<BaseCharacter>();

        if (character != null && character.IsDead())
        {
            ClearCurrentObstacle();
            return;
        }


        // =================================================
        // CAMERA FOLLOW
        // =================================================

        Vector3 targetPosition =
            target.position + cameraOffset;

        if (smoothFollow)
        {
            transform.position =
                Vector3.Lerp(
                    transform.position,
                    targetPosition,
                    followSpeed * Time.deltaTime
                );
        }
        else
        {
            transform.position =
                targetPosition;
        }

        transform.rotation =
            Quaternion.Euler(
                fixedCameraRotation
            );


        // =================================================
        // WALL DETECTION
        // =================================================

        if (enableWallFade)
        {
            DetectObstacle();
        }
        else
        {
            ClearCurrentObstacle();
        }
    }


    // =====================================================
    // DETECT WALL BETWEEN CAMERA AND PLAYER
    // =====================================================

    private void DetectObstacle()
    {
        if (target == null)
        {
            ClearCurrentObstacle();
            return;
        }

        Vector3 direction =
            target.position - transform.position;

        float distance =
            direction.magnitude;

        if (distance <= 0.01f)
        {
            ClearCurrentObstacle();
            return;
        }

        direction.Normalize();


        // =================================================
        // CAMERA → PLAYER RAYCAST
        // =================================================

        bool hitObstacle =
            Physics.Raycast(
                transform.position,
                direction,
                out RaycastHit hit,
                distance,
                obstructionMask,
                QueryTriggerInteraction.Ignore
            );


        // =================================================
        // WALL HIT
        // =================================================

        if (hitObstacle)
        {
            FadeObstacle fade =
                hit.collider
                    .GetComponentInParent<FadeObstacle>();


            // ---------------------------------------------
            // Có FadeObstacle
            // ---------------------------------------------

            if (fade != null)
            {
                if (fade != currentObstacle)
                {
                    // Khôi phục Wall trước đó
                    if (currentObstacle != null)
                    {
                        currentObstacle.FadeIn();
                    }


                    // Làm mờ Wall mới
                    fade.FadeOut();

                    currentObstacle = fade;
                }

                return;
            }
        }


        // =================================================
        // KHÔNG CÒN WALL CHE PLAYER
        // =================================================

        ClearCurrentObstacle();
    }


    // =====================================================
    // CLEAR CURRENT WALL
    // =====================================================

    private void ClearCurrentObstacle()
    {
        if (currentObstacle == null)
            return;

        currentObstacle.FadeIn();

        currentObstacle = null;
    }


    // =====================================================
    // SET PLAYER TARGET
    // =====================================================

    public void SetTarget(Transform newTarget)
    {
        // Nếu đang fade một Wall
        // thì khôi phục trước khi đổi Player
        ClearCurrentObstacle();

        target = newTarget;

        if (target == null)
            return;


        // Đặt camera ngay lập tức
        // tránh frame đầu tiên camera nằm sai vị trí
        transform.position =
            target.position + cameraOffset;

        transform.rotation =
            Quaternion.Euler(
                fixedCameraRotation
            );
    }


    // =====================================================
    // CLEAR TARGET
    // =====================================================

    public void ClearTarget()
    {
        ClearCurrentObstacle();

        target = null;
    }


    // =====================================================
    // OPTIONAL GETTER
    // =====================================================

    public Transform GetTarget()
    {
        return target;
    }


    // =====================================================
    // DEBUG RAY
    // =====================================================

    private void OnDrawGizmos()
    {
        if (target == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            transform.position,
            target.position
        );
    }
}