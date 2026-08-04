using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunDatabase", menuName = "Dynher/Gun Database")]
public class GunDatabase : ScriptableObject
{
    [SerializeField] private List<GunData> guns = new();

    public IReadOnlyList<GunData> Guns => guns;
}