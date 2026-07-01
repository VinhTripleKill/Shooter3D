using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GameObject gunVisualPrefab;

    [Tooltip("-1 = Full Ammo lần đầu")]
    public int currentAmmo = -1;

    public bool isReloading;

    // SỬA Ở ĐÂY
    public void Pickup(PlayerInteraction playerInteraction)
    {
        playerInteraction.EquipGun(
            gunVisualPrefab,
            currentAmmo,
            isReloading);

        Destroy(gameObject);
    }
}