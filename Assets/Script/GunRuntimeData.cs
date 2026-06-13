using UnityEngine;

public class GunRuntimeData : MonoBehaviour
{
    [HideInInspector]
    public int currentAmmo;

    [HideInInspector]
    public bool isReloading;

    public void Initialize(GunData gunData)
    {
        currentAmmo = gunData.maxCountShot;
        isReloading = false;
    }
}