using System.Collections.Generic;
using UnityEngine;

public class GunItem : ItemBase
{
    [Header("Visual")]
    [Tooltip("Object con chứa model/vfx sẽ xoay và float")]
    public Transform itemVisual;

    [Header("Name Display")]
    [Tooltip("Component quản lý tên item")]
    public ItemNameVisual itemNameVisual;

    [Header("Data")]
    [Tooltip("Gán GunData cho item đặt sẵn trong Scene")]
    public GunData gunData;

    private SphereCollider sphereCollider;
    private Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();
    private Vector3 startVisualPos;

    protected override void Awake()
    {
        base.Awake(); // Khởi tạo Rigidbody từ ItemBase

        sphereCollider = GetComponent<SphereCollider>();

        // Lưu layer gốc
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child != null)
                originalLayers[child] = child.gameObject.layer;
        }

        TryFindItemVisual();
        TryFindItemNameVisual();

        // Hiển thị tên ngay từ đầu nếu có GunData
        if (gunData != null)
            SetGunData(gunData);

        // Ban đầu ẩn tên (chờ player vào vùng)
        SetNameVisible(false);
    }

    private void TryFindItemVisual()
    {
        if (itemVisual != null) return;

        itemVisual = transform.Find("ItemVisual");

        if (itemVisual == null)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<MeshRenderer>() || child.GetComponent<SkinnedMeshRenderer>())
                {
                    itemVisual = child;
                    break;
                }
            }
        }
    }

    private void TryFindItemNameVisual()
    {
        if (itemNameVisual != null) return;
        itemNameVisual = GetComponentInChildren<ItemNameVisual>(true);
    }

    public void Initialize()
    {
        TryFindItemVisual();
        TryFindItemNameVisual();
    }

    public void SetGunData(GunData data)
    {
        if (data == null) return;

        gunData = data;

        if (itemNameVisual != null)
            itemNameVisual.SetItemName(data.gunName);
        else
            Debug.LogWarning($"ItemNameVisual chưa được gán trên {gameObject.name}");
    }

    public void SetCanPickUp(bool value)
    {
        int pickupLayer = LayerMask.NameToLayer("CanPickUp");

        foreach (var pair in originalLayers)
        {
            if (pair.Key == null) continue;
            pair.Key.gameObject.layer = value ? pickupLayer : pair.Value;
        }

        // Hiển thị tên khi có thể pickup
        SetNameVisible(value);
    }

    private void SetNameVisible(bool visible)
    {
        if (itemNameVisual != null)
        {
            itemNameVisual.gameObject.SetActive(visible);
        }
    }

    // Override để chỉ xoay + nhấp nhô itemVisual, giữ root ổn định (collider/rigidbody)
    protected override void UpdateIdleEffect()
    {
        if (itemVisual == null)
        {
            // Fallback: dùng logic gốc của ItemBase nếu không có visual
            base.UpdateIdleEffect();
            return;
        }

        // Xoay visual
        itemVisual.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // Nhấp nhô visual
        floatTimer += Time.deltaTime * floatSpeed;
        Vector3 pos = startVisualPos;
        pos.y += Mathf.Sin(floatTimer) * floatAmplitude;
        itemVisual.position = pos;
    }

    // Override để lưu vị trí visual khi đáp đất
    protected override void OnLanded(RaycastHit hit)
    {
        base.OnLanded(hit); // Vẫn set position root + kinematic từ ItemBase

        if (itemVisual != null)
            startVisualPos = itemVisual.position;
        else
            startVisualPos = transform.position;
    }
}