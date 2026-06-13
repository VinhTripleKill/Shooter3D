using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GameObject gunVisualPrefab;

    [Tooltip("-1 = Full Ammo lần đầu")]
    public int currentAmmo = -1;

    public bool isReloading;

    public void Pickup(PlayerWeapon playerWeapon)
    {
        playerWeapon.EquipGun(
            gunVisualPrefab,
            currentAmmo,
            isReloading);

        Destroy(gameObject);
    }
}