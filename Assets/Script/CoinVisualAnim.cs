using UnityEngine;

public class CoinVisualAnim : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 360f;
    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startLocalPosition;
    private float floatTimer;

    private bool isAnimating = true;

    private void Awake()
    {
        startLocalPosition = transform.localPosition;

        floatTimer = Random.Range( 0f, Mathf.PI * 2f );
    }

    private void Update()
    {
        if (!isAnimating) return;

        UpdateRotation();
        UpdateFloating();
    }

    private void UpdateRotation()
    {
        transform.Rotate( Vector3.up, rotateSpeed * Time.deltaTime, Space.Self );
    }

    private void UpdateFloating()
    {
        floatTimer += Time.deltaTime * floatSpeed;

        Vector3 localPosition = startLocalPosition;

        localPosition.y += Mathf.Sin(floatTimer) * floatAmplitude;

        transform.localPosition = localPosition;
    }

    public void SetAnimating(bool value)
    {
        isAnimating = value;

        if (!isAnimating)
        {
            transform.localPosition = startLocalPosition;
        }
    }
}