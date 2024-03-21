using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigigdbody;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private GameObject _explosionEffect;
    [SerializeField]
    private float _explosionRadius;
    [SerializeField]
    private float _explosionForce;
    [SerializeField]
    private float _gravityAcceleration;
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private LayerMask _explosionLayerMask;

    private float _damage;

    public void Shoot(float damage, Vector3 direction)
    {
        _damage = damage;
        transform.rotation = Quaternion.LookRotation(direction);
        _rigigdbody.velocity = transform.forward * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] collidersInRadius = 
            Physics.OverlapSphere(transform.position, _explosionRadius, _explosionLayerMask);

        foreach (var collider in collidersInRadius)
        {
            if (collider == _collider)
            {
                continue;
            }

            if (collider.TryGetComponent<Rigidbody>(out var colliderRb))
            {
                colliderRb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 5);
            }
            collider.GetComponent<IMoveable>()?.Move();
            collider.GetComponent<IDamageable>()?.TakeDamage(_damage);
        }
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        _rigigdbody.AddForce(Vector3.up * _gravityAcceleration, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Hitbox>(out var hitbox))
        {
            hitbox.TakeDamage(_damage);
            Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
