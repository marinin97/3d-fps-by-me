using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class LootContainer : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed;
    [SerializeField]
    private float _autoDestroyTime;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        Destroy(gameObject, _autoDestroyTime);
    }
    private void Update()
    {
        _transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
    }
}
