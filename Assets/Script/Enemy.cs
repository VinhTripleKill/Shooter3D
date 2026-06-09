using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> AllEnemies =
        new List<Enemy>();

    [Header("Circle Move")]
    [SerializeField]
    private float radius = 3f;

    [SerializeField]
    private float rotateSpeed = 60f;

    private Vector3 centerPoint;
    private float currentAngle;

    private void OnEnable()
    {
        AllEnemies.Add(this);
    }

    private void OnDisable()
    {
        AllEnemies.Remove(this);
    }

    private void Start()
    {
        centerPoint = transform.position;

        currentAngle =
            Random.Range(0f, 360f);
    }

    private void Update()
    {
        currentAngle +=
            rotateSpeed * Time.deltaTime;

        float rad =
            currentAngle * Mathf.Deg2Rad;

        Vector3 offset =
            new Vector3(
                Mathf.Cos(rad),
                0f,
                Mathf.Sin(rad))
            * radius;

        transform.position =
            centerPoint + offset;
    }
}