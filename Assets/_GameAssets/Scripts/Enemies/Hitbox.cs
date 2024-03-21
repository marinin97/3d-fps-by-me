using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Hitbox : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField]
    public Collider Collider { get; private set; }

    [SerializeField]
    private MonoBehaviour _mainDamageable;

    //Не используется, просто для реализации IDamageable
    public float Health { get; }
    public float MaxHealth { get; }
    /// 

    public IDamageable MainDamageable
    {
        get => _mainDamageable as IDamageable;
        set
        {
            if (value is not IDamageable)
            {
                throw new System.Exception($"{value} does not implement IDamageable!");
            }
            else
            {
                _mainDamageable = value as MonoBehaviour;
            }
        }
    }

    public void TakeDamage(float damageValue)
    {
        MainDamageable.TakeDamage(damageValue);
    }

    private void OnValidate()
    {
        if (_mainDamageable != null && _mainDamageable is not IDamageable)
        {
            _mainDamageable = null;
            throw new System.Exception($"{_mainDamageable} does not implement IDamageable!");
        }

        if (Rigidbody == null)
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
        else
        {
            Rigidbody.isKinematic = false;
            Rigidbody.useGravity = false;
        }

        if (Collider == null)
        {
            Collider = GetComponent<Collider>();
        }
    }
}
