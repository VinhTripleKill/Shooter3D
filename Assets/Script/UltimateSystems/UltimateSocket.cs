using UnityEngine;

public class UltimateSocket : MonoBehaviour
{
    private UltimateBehaviour currentUltimate;

    public UltimateBehaviour CurrentUltimate => currentUltimate;

    public void SetUltimate(UltimateBehaviour prefab, PlayerUltimate owner)
{
    Clear();

    if (prefab == null) return;

    currentUltimate = Instantiate(prefab, transform);

    currentUltimate.transform.localPosition = Vector3.zero;
    currentUltimate.transform.localRotation = Quaternion.identity;

    currentUltimate.Initialize(owner);
}

    public void Clear()
    {
        if (currentUltimate != null)
            Destroy(currentUltimate.gameObject);

        currentUltimate = null;
    }
}