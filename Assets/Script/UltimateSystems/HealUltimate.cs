using UnityEngine;

public class HealerUltimate : UltimateBehaviour
{
    [SerializeField] private float healAmount = 20;

    public override bool Execute(){ playerController.Heal(healAmount); return true; }
}