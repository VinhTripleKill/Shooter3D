using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GameObject gunVisualPrefab;

    [Tooltip("-1 = Full Ammo lần đầu")]
    public int currentAmmo = -1;
    public bool isReloading;

    public void Pickup(PlayerInteraction playerInteraction)
    {
        playerInteraction.EquipGun(gunVisualPrefab, currentAmmo, isReloading);
        Destroy(gameObject);
    }

    // Đảm bảo itemVisual được tìm lại sau khi Instantiate
    private void Awake()
    {
        GetComponent<GunItem>()?.Initialize();
    }
}