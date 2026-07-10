using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private PlayerController playerController;
    public GamePlayUI gameplayUI;

    private GunPickup nearbyGun;
    private GunPickup currentTargetGun;
    private List<GunPickup> gunsInRange = new();

    private InputAction dropAction;
    private InputAction interactionAction;

    private void Awake()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
        playerController = GetComponent<PlayerController>();

        
        var playerInput = GetComponent<PlayerInput>();
        dropAction = playerInput.actions["Drop"];
        interactionAction = playerInput.actions["Interaction"];

    }
    public void SetGameplayUI(GamePlayUI ui)
{
    gameplayUI = ui;
    if (gameplayUI != null)
    {
        gameplayUI.GetPickUpButton().onClick.AddListener(OnPickUpButtonClicked);
    }
}
    public void EquipGun(GameObject gunPrefab, int ammo = -1, bool wasReloading = false)
    {
        playerWeapon.EquipGun(gunPrefab, ammo, wasReloading);
    }
    private void DropGun()
    {
        // Gọi DropGun của PlayerWeapon (đã có SpawnDroppedGun bên trong)
        playerWeapon.DropGun();
    }
    private void OnEnable()
    {
        dropAction.performed += DropPerformed;
        interactionAction.performed += InteractionPerformed;
    }

    private void OnDisable()
    {
        dropAction.performed -= DropPerformed;
        interactionAction.performed -= InteractionPerformed;
    }

    private void OnDestroy()
    {
        if (gameplayUI != null)
        {
            gameplayUI.GetPickUpButton().onClick.RemoveListener(OnPickUpButtonClicked);
        }
    }

    private void Update()
    {
        if (playerController.IsDead()) return;
        UpdatePickupTarget();
    }

   

    private void DropPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.IsDead()) return;
        DropGun();
    }



    private void SpawnDroppedGun(GunData gunData, int ammo, bool wasReloading)
    {
        playerWeapon.SpawnDroppedGun(gunData, ammo, wasReloading);
    }

    
    private void InteractionPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.IsDead()) return;
        if (nearbyGun == null) return;

        nearbyGun.Pickup(this);
        nearbyGun = null;
        gameplayUI?.HidePickUp();
    }

    private void OnPickUpButtonClicked()
    {
        if (nearbyGun == null) return;

        nearbyGun.Pickup(this);
        nearbyGun = null;
        gameplayUI?.HidePickUp();
    }

    private void UpdatePickupTarget()
    {
        GunPickup bestGun = null;
        float bestScore = Mathf.Infinity;

        foreach (GunPickup gun in gunsInRange)
        {
            if (gun == null) continue;

            float distance = Vector3.Distance(transform.position, gun.transform.position);
            if (distance < bestScore)
            {
                bestScore = distance;
                bestGun = gun;
            }
        }

        if (bestGun == currentTargetGun) return;

        // Bỏ highlight khẩu cũ
        if (currentTargetGun != null)
            currentTargetGun.GetComponent<GunItem>()?.SetCanPickUp(false);

        currentTargetGun = bestGun;
        nearbyGun = bestGun;

        // Highlight khẩu mới
        if (currentTargetGun != null)
        {
            currentTargetGun.GetComponent<GunItem>()?.SetCanPickUp(true);
            gameplayUI?.ShowPickUp();
        }
        else
        {
            gameplayUI?.HidePickUp();
        }
    }

    #region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GunPickup gun))
        {
            if (!gunsInRange.Contains(gun))
                gunsInRange.Add(gun);

            gameplayUI?.ShowPickUp();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GunPickup gun))
        {
            gunsInRange.Remove(gun);

            if (currentTargetGun == gun)
            {
                gun.GetComponent<GunItem>()?.SetCanPickUp(false);
                currentTargetGun = null;
                nearbyGun = null;
                gameplayUI?.HidePickUp();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out GunPickup gun))
        {
            nearbyGun = gun;
            gameplayUI?.ShowPickUp();
        }
    }

    #endregion
}