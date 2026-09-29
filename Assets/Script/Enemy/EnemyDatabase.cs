using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase",menuName = "Dynher/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    [SerializeField] private List<EnemyData> enemies = new();

    public IReadOnlyList<EnemyData> Enemies => enemies;
}