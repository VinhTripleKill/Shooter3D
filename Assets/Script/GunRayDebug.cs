using UnityEngine;

public class GunRayDebug : MonoBehaviour
{
    [SerializeField] private GameObject lineDebugPrefab;

    private LineRenderer[] aimLines;
    private GunData currentGunData;
    private Transform currentFirePoint;

    public void Initialize(GunData gunData, Transform firePoint)
    {
        currentGunData = gunData;
        currentFirePoint = firePoint;
        CreateAimLines();
    }

    public void UpdateAimLines()
    {
        if (aimLines == null || currentGunData == null || currentFirePoint == null)
            return;

        // Tắt nếu không cần hiển thị
        if (!currentGunData.showRay || currentGunData.fireType != GunData.GunFireType.Raycast)
        {
            foreach (var line in aimLines)
                if (line != null) line.enabled = false;
            return;
        }

        int pelletCount = currentGunData.pelletCount;
        float angleStep = currentGunData.angleBetweenBullets;
        float startAngle = -(angleStep * (pelletCount - 1)) / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            LineRenderer line = aimLines[i];
            if (line == null) continue;

            line.enabled = true;

            float currentAngle = startAngle + angleStep * i;
            Quaternion spread = Quaternion.Euler(0, currentAngle, 0);

            Vector3 direction = (currentFirePoint.rotation * spread) * Vector3.forward;

            Vector3 endPos;
            if (Physics.SphereCast(
     currentFirePoint.position,
     currentGunData.hitRadius,
     direction,
     out RaycastHit hit,
     currentGunData.raycastDistance,
     currentGunData.hitMask,
     QueryTriggerInteraction.Ignore))
            {
                endPos = hit.point;
            }
            else
            {
                endPos = currentFirePoint.position + direction * currentGunData.raycastDistance;
            }
            float width =
    currentGunData.hitRadius * 2f;

            line.startWidth = width;
            line.endWidth = width;
            line.positionCount = 2;
            line.SetPosition(0, currentFirePoint.position);
            line.SetPosition(1, endPos);
        }
    }

    private void CreateAimLines()
    {
        ClearAimLines();

        if (lineDebugPrefab == null)
            return;

        int pelletCount = currentGunData.pelletCount;

        aimLines = new LineRenderer[pelletCount];

        for (int i = 0; i < pelletCount; i++)
        {
            GameObject lineObj =
                Instantiate(lineDebugPrefab, transform);

            LineRenderer line =
                lineObj.GetComponent<LineRenderer>();

            float width =
                currentGunData.hitRadius * 2f;

            line.startWidth = width;
            line.endWidth = width;

            aimLines[i] = line;
        }
    }

    private void ClearAimLines()
    {
        if (aimLines == null) return;

        foreach (var line in aimLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }

        aimLines = null;
    }

    public void Cleanup()
    {
        ClearAimLines();
    }
}