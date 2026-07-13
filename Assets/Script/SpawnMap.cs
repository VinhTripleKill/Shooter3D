using UnityEngine;

public class SpawnMap : MonoBehaviour
{
    [Header("Prefab Block")]
    public GameObject blockPrefab; // Kéo prefab "blockCube1:1" vào đây

    [Header("Map Settings")]
    public int mapSize = 100;
    public float spacing = 1f;           // Khoảng cách giữa các block (thường là 1 nếu block size = 1)
    public float defaultY = -1f;         // Y mặc định như bạn yêu cầu

    void Start()
    {
        SpawnGrid();
    }

    public void SpawnGrid()
    {
        if (blockPrefab == null)
        {
            Debug.LogError("Chưa gán Prefab blockPrefab!");
            return;
        }

        // Xóa các block cũ nếu có (tránh spawn nhiều lần khi test)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                Vector3 spawnPos = new Vector3(
                    x * spacing,     // pos.x từ 0 đến 99
                    defaultY,        // Y = -1
                    z * spacing      // pos.z từ 0 đến 99 (thay vì y như bạn nhầm)
                );

                GameObject block = Instantiate(blockPrefab, spawnPos, Quaternion.identity, transform);
                block.name = $"Block_{x}_{z}";
            }
        }

        Debug.Log($"Đã spawn xong map {mapSize}x{mapSize} block!");
    }
}