using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [Header("References từ Model Prefab")]
    public Animator animator;
    public Transform gunHolder;

    private CharacterData characterData;

    public Animator Animator => animator;
    public Transform GunHolder => gunHolder;
    public CharacterData Data => characterData;

    public void Initialize(CharacterData data)
    {
        characterData = data;

        // Tự động tìm nếu chưa gán
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (gunHolder == null)
        {
            // Tìm theo tên bạn đã setup trong prefab
            gunHolder = transform.Find("GunHolder");
            if (gunHolder == null)
                gunHolder = GetComponentInChildren<Transform>(true); // fallback
        }

        Debug.Log($"PlayerModel initialized cho {data.characterName}");
    }
}