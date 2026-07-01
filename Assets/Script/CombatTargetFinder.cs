using UnityEngine;

public static class CombatTargetFinder
{
    public static Transform GetNearestTarget(
        Vector3 origin,
        float range)
    {
        Transform nearest = null;

        float nearestDistance = Mathf.Infinity;

        foreach (IAutoAimTarget target in AutoAimManager.Targets)
        {
            if (target == null)
                continue;

            Transform targetTransform =
                target.GetTargetTransform();

            if (targetTransform == null)
                continue;

            float distance =
                Vector3.Distance(
                    origin,
                    targetTransform.position);

            if (distance > range)
                continue;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = targetTransform;
            }
        }

        return nearest;
    }
    public static Transform RotateToNearestTarget(
        Transform owner,
        float range)
    {
        Transform target =
            GetNearestTarget(
                owner.position,
                range);

        if (target == null)
            return null;

        Vector3 dir =
            target.position - owner.position;

        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            owner.forward =
                dir.normalized;
        }

        return target;
    }
}