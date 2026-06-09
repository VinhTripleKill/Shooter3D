using UnityEngine;

public class GunVisual : MonoBehaviour
{
    public Transform firePoint;

    public GunData gunData;

    public Transform GetFirePoint()
    {
        return firePoint;
    }
}