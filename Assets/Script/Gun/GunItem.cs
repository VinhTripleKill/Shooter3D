using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class GunItem : MonoBehaviour
{
    [Header("Floating")]
    public float floatSpeed = 2f;
    public float floatHeight = 0.15f;
    [Header("Ground Check")]
    public LayerMask groundMask;
    [Header("Rotation")]
    public float rotateSpeed = 50f;
    private int defaultLayer;
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private Dictionary<Transform, int> originalLayers =
    new Dictionary<Transform, int>();
    private Vector3 startPos;

    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            originalLayers.Add(child, child.gameObject.layer);
        }
    }
    public void SetCanPickUp(bool value)
    {
        int pickupLayer =
            LayerMask.NameToLayer("CanPickUp");

        foreach (var pair in originalLayers)
        {
            if (pair.Key == null)
                continue;

            pair.Key.gameObject.layer =
                value ?
                pickupLayer :
                pair.Value;
        }
    }
    private void Update()
    {
        if (!isGrounded) return;

        // Floating
        float newY = startPos.y +
                     Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );

        // Rotation
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrounded) return;

        // Check LayerMask ground
        if (((1 << collision.gameObject.layer) & groundMask) == 0)
            return;

        LandItem();
    }

    private void LandItem()
    {
        isGrounded = true;

        // Stop physics
        rb.isKinematic = true;
        rb.useGravity = false;

        // Cho phép player trigger
        sphereCollider.isTrigger = true;

        startPos = transform.position;
    }
}