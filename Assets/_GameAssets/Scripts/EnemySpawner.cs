using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System.Collections;
using Random = UnityEngine.Random;
using System;

public class EnemySpawner : MonoBehaviour
{
    public event Action<int> OnWaveStarted;
    public event Action<int> OnEnemyKilled;

    [Header("Loot containers")]
    [SerializeField]
    private List<LootContainer> _containersPrefabs;
    [SerializeField]
    private float _dropPercent;

    [Space(10)]
    [SerializeField]
    private float _delayBetweenWaves;
    [SerializeField]
    private PlayerController _player;
    [SerializeField]
    private List<Collider> _spawnZones;
    [SerializeField]
    private List<Zombie> _zombiePrefabs;
    [SerializeField]
    private List<EnemyWave> _enemyWaves;

    private int _currentWaveIndex;
    private float _currentSpawnDelay;
    private WaitForSeconds _waitForNextSpawn;
    private WaitForSeconds _waitForWaveStart;
    private int _enemiesKilledOnWave;
    private int _enemiesSpawnedOnWave;
    private Transform _playerTransform;
    private int _currentWaveCount;
    private int _totalEnemiesKilled;

    private void Awake()
    {
        _playerTransform = _player.transform;
        _waitForWaveStart = new WaitForSeconds(_delayBetweenWaves);
    }

    private void Start()
    {
        _currentWaveIndex = 0;
        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        _enemiesKilledOnWave = 0;
        _enemiesSpawnedOnWave = 0;
        yield return _waitForWaveStart;
        _currentWaveCount++;
        _currentSpawnDelay = _enemyWaves[_currentWaveIndex].StartSpawnDelay;
        _waitForNextSpawn = new WaitForSeconds(_currentSpawnDelay);
        OnWaveStarted?.Invoke(_currentWaveCount);
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        Zombie enemy = Instantiate(_zombiePrefabs[Random.Range(0, _zombiePrefabs.Count)], 
                                GetNearestRandomSpawnPoint(), Quaternion.identity);

        enemy.SetPlayer(_player);
        enemy.OnKilled += HandleEnemyKill;
        _enemiesSpawnedOnWave++;
        if (_enemiesSpawnedOnWave < _enemyWaves[_currentWaveIndex].EnemyCount)
        {
            yield return _waitForNextSpawn;
            StartCoroutine(SpawnEnemy());
        }
    }

    private void HandleEnemyKill(Zombie enemy)
    {
        SpawnRandomLootContainer(enemy.transform.position);
        enemy.OnKilled -= HandleEnemyKill;
        _totalEnemiesKilled++;
        _enemiesKilledOnWave++;
        EnemyWave currentWave = _enemyWaves[_currentWaveIndex];
        if (_enemiesKilledOnWave < currentWave.EnemyCount)
        {
            var newSpawnDelay = _currentSpawnDelay - currentWave.SpawnDelayStep;
            _currentSpawnDelay = newSpawnDelay > currentWave.MinSpawnDelay ? 
                                 newSpawnDelay : currentWave.MinSpawnDelay;

            _waitForNextSpawn = new WaitForSeconds(_currentSpawnDelay);
        }
        else
        {
            var newCurrentWaveIndex = _currentWaveIndex + 1;

            _currentWaveIndex = newCurrentWaveIndex < _enemyWaves.Count ? 
                                newCurrentWaveIndex : _enemyWaves.Count - 1;

            StartCoroutine(StartWave());
        }
        OnEnemyKilled?.Invoke(_totalEnemiesKilled);
    }

    private void SpawnRandomLootContainer(Vector3 groundPosition)
    {
        if (Random.Range(0, 100f) > _dropPercent)
        {
            return;
        }
        Instantiate(_containersPrefabs[Random.Range(0, _containersPrefabs.Count)], groundPosition + Vector3.up, Quaternion.identity);
    }

    private Vector3 GetNearestRandomSpawnPoint()
    {
        Collider nearestSpawnZone = _spawnZones.OrderBy
            (collider => Vector3.Distance(collider.transform.position, _playerTransform.position))
            .FirstOrDefault();

        if (NavMesh.SamplePosition(RandomPointInBounds(nearestSpawnZone.bounds), out var hit, 100, 1))
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
        Random.Range(bounds.min.x, bounds.max.x),
        Random.Range(bounds.min.y, bounds.max.y),
        Random.Range(bounds.min.z, bounds.max.z)
    );
    }
}
