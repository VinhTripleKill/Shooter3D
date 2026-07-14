using TMPro;
using UnityEngine;

public class DamageVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float lifeTime = 1f;

    private Camera cam;
    private float timer;

    public void Initialize(float damage)
    {
        damageText.text = Mathf.RoundToInt(damage).ToString();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // Bay lên
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Luôn nhìn camera
        if (cam != null)
            transform.forward = cam.transform.forward;

        timer += Time.deltaTime;

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}