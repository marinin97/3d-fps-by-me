using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamageable
{
    public event Action<Zombie> OnKilled;

    [field: SerializeField]
    public float MaxHealth { get; private set; }
    public float Health { get; private set; }

    [SerializeField]
    private float _damage;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private CharacterController _characterController;
    [SerializeField]
    private List<Hitbox> _hitBoxes;
    [SerializeField]
    private Transform _transform;
    [SerializeField]
    private float _attackRadius;
    [SerializeField]
    private Transform _attackSphereCenterTransform;

    private bool _isAttacking;
    [SerializeField]
    private Collider _collider;

    private int _hashIsAttackingBool;

    [SerializeField]
    private PlayerController _player;

    private Transform _playerTransform;
    private bool _isDead;
    private bool _playerIsDead;


    private void Awake()
    {
        Health = MaxHealth;
        if (_player != null)
        {
            _player.OnDeath += HandlePlayerDeath;
            _playerTransform = _player.transform;
        }
        _hashIsAttackingBool = Animator.StringToHash("IsAttacking");
    }

    public void SetPlayer(PlayerController player)
    {
        if (_player != null)
        {
            _player.OnDeath -= HandlePlayerDeath;
        }
        _player = player;
        _player.OnDeath += HandlePlayerDeath;
        _playerTransform = _player.transform;
    }

    private void Update()
    {
        if (_isDead || _playerTransform == null || _playerIsDead)
        {
            return;
        }

        _navMeshAgent.SetDestination(_playerTransform.position);
        if (!_isAttacking && Vector3.Distance(_navMeshAgent.destination, _transform.position) <= _navMeshAgent.stoppingDistance)
        {
            _animator.SetBool(_hashIsAttackingBool, true);
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.isStopped = true;
            _isAttacking = true;
        }
        else if (_isAttacking && Vector3.Distance(_navMeshAgent.destination, _transform.position) > _navMeshAgent.stoppingDistance)
        {
            _animator.SetBool(_hashIsAttackingBool, false);
            _navMeshAgent.isStopped = false;
            _isAttacking = false;
        }
    }

    public void Attack()
    {
        if (_playerIsDead)
        {
            return;
        }
        var collidersInRadius = Physics.OverlapSphere(_attackSphereCenterTransform.position, _attackRadius);
        foreach (var collider in collidersInRadius)
        {
            if (collider == _collider)
            {
                continue;
            }

            if (collider.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.TakeDamage(_damage);
                break;
            }
        }
    }

    public void TakeDamage(float damageValue)
    {
        if (Health == 0)
        {
            return;
        }

        Health -= damageValue;
        if (Health <= 0)
        {
            _isDead = true;
            Health = 0;


            _hitBoxes.ForEach(hitbox =>
            {
                if (hitbox.Rigidbody != null)
                {
                    hitbox.Rigidbody.useGravity = true;
                }
                if (hitbox.Collider != null)
                {
                    hitbox.Collider.isTrigger = false;
                }
            });

            _animator.enabled = false;
            _navMeshAgent.enabled = false;
            _characterController.enabled = false;
            OnKilled?.Invoke(this);
            Destroy(gameObject, 10);
        }
    }

    private void HandlePlayerDeath()
    {
        _playerIsDead = true;
    }
}
