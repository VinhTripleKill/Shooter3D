using UnityEngine;

public class PlayerVisualChar : MonoBehaviour
{
    [Header("Vị trí spawn Model Nhân Vật")]
    [SerializeField] private Transform modelPos;

    public Transform ModelPos => modelPos;

    private GameObject currentModel;
    private PlayerModel playerModel;

    public void SpawnCharacterModel(CharacterData data)
    {
        if (data == null || data.modelPrefab == null)
        {
            Debug.LogError("CharacterData hoặc modelPrefab bị null!");
            return;
        }

        if (currentModel != null)
            Destroy(currentModel);

        currentModel = Instantiate(data.modelPrefab, modelPos);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;

        playerModel = currentModel.GetComponent<PlayerModel>();
        if (playerModel == null)
            playerModel = currentModel.AddComponent<PlayerModel>();

        playerModel.Initialize(data);

        // === QUAN TRỌNG: Gán reference cho các component chính ===
        AssignReferencesToPlayerComponents();

        Debug.Log($"Đã spawn model: {data.characterName}");
    }

    private void AssignReferencesToPlayerComponents()
    {
        if (playerModel == null) return;

        // Gán Animator cho PlayerAnim
        PlayerAnim playerAnim = GetComponent<PlayerAnim>();
        if (playerAnim != null)
        {
            playerAnim.SetAnimator(playerModel.Animator);
        }

        PlayerWeapon playerWeapon = GetComponent<PlayerWeapon>();
        if (playerWeapon != null && playerModel.GunHolder != null)
        {
            playerWeapon.SetGunHolder(playerModel.GunHolder);
        }
    }

    public PlayerModel GetPlayerModel() => playerModel;
}