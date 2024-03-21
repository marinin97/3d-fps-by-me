using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigigdbody;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Transform _bulletHolePrefab;

    private float _damage;

    private const int IGNORE_RAYCAST_LAYER_INDEX = 2;

    public void Shoot(float damage, Vector3 direction)
    {
        _damage = damage;
        transform.rotation = Quaternion.LookRotation(direction);
        _rigigdbody.velocity = transform.forward * _speed;
        Destroy(gameObject, 5);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == IGNORE_RAYCAST_LAYER_INDEX)
        {
            return;
        }

        var collisionGameObject = collision.gameObject;
        var damageable = collisionGameObject.GetComponent<IDamageable>();
        if (damageable is not Hitbox)
        {
            var bulletHole = Instantiate(_bulletHolePrefab);
            bulletHole.SetPositionAndRotation(
                collision.contacts[0].point, 
                Quaternion.LookRotation(collision.contacts[0].normal));

            bulletHole.Translate(transform.forward * 0.02f);
            bulletHole.SetParent(collision.transform);
            Destroy(bulletHole.gameObject, 5);
        }

        collisionGameObject.GetComponent<IMoveable>()?.Move();
        damageable?.TakeDamage(_damage);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Hitbox>(out var hitbox))
        {
            hitbox.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
