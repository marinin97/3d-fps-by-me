using System;
using UnityEngine;

[Serializable]
public class EnemyWave
{
    [field: SerializeField]
    public int EnemyCount { get; private set; }
    [field: SerializeField]
    public float StartSpawnDelay { get; private set; }
    [field: SerializeField]
    public float MinSpawnDelay { get; private set; }
    [field: SerializeField]
    public float SpawnDelayStep { get; private set; }
}
