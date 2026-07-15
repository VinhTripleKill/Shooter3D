using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class GunItem : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("Object con chứa model/vfx sẽ xoay và float")]
    public Transform itemVisual;

    [Header("Name Display")]
    [Tooltip("Component quản lý tên item")]
    public ItemNameVisual itemNameVisual;

    [Header("Floating")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.15f;

    [Header("Rotation")]
    public float rotateSpeed = 50f;

    [Header("Ground Check")]
    public LayerMask groundMask;

    [Header("Data")]
    [Tooltip("Gán GunData cho item đặt sẵn trong Scene")]
    public GunData gunData;

    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private Dictionary<Transform, int> originalLayers = new Dictionary<Transform, int>();

    private Vector3 startVisualPos;
    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

        itemVisual = transform.Find("itemVisual") 
                  ?? transform.Find("Visual") 
                  ?? transform.Find("Model");

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

    private void Update()
    {
        if (!isGrounded || itemVisual == null) return;

        float newY = startVisualPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        itemVisual.position = new Vector3(itemVisual.position.x, newY, itemVisual.position.z);

        itemVisual.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrounded) return;
        if (((1 << collision.gameObject.layer) & groundMask) == 0) return;

        LandItem();
    }

    private void LandItem()
    {
        isGrounded = true;

        rb.isKinematic = true;
        rb.useGravity = false;
        sphereCollider.isTrigger = true;

        if (itemVisual != null)
            startVisualPos = itemVisual.position;
        else
            startVisualPos = transform.position;
    }
}