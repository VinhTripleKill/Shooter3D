using UnityEngine;

[CreateAssetMenu(
    fileName = "UltimateData",
    menuName = "Dynher/Ultimate Data")]
public class UltimateData : ScriptableObject
{
    [Header("Info")]
    public string ultimateName;

    [TextArea]
    public string descriptionUltimate;

    public Sprite icon;

    [Header("Config")]
    public float manaCost;

    public float cooldown;
}