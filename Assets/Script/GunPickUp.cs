using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GameObject gunVisualPrefab;

    public void Pickup(PlayerWeapon playerWeapon)   // ← đổi thành PlayerWeapon
    {
        playerWeapon.EquipGun(gunVisualPrefab);
        Destroy(gameObject);
    }
}