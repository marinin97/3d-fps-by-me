using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    [SerializeField]
    private float _attackRadius;

    private float _timeBeforeShoot;

    #region Animations
    private Animator _animator;
    private int _hashAttackTrigger;
    private int _hashAttackSpeedFloat;
    #endregion

    private Vector3 _startLocalPosition;
    private Vector3 _startLocalRotation;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _hashAttackTrigger = Animator.StringToHash("Attack");
        _hashAttackSpeedFloat = Animator.StringToHash("AttackSpeed");
        _animator.keepAnimatorStateOnDisable = false;
        _startLocalPosition = transform.localPosition;
        _startLocalRotation = transform.localRotation.eulerAngles;
    }

    public override void Reload()
    {

    }

    private void Update()
    {
        _timeBeforeShoot -= Time.deltaTime;
        _timeBeforeShoot = _timeBeforeShoot < 0 ? 0 : _timeBeforeShoot;
    }

    public override void Shoot(bool fireIsPressed, Transform cameraTransform)
    {
        if (_timeBeforeShoot > 0 || fireIsPressed)
        {
            return;
        }

        _animator.SetTrigger(_hashAttackTrigger);
        _timeBeforeShoot = WeaponSettings.ShootDelay;
        Ray shootRay = new(cameraTransform.position, cameraTransform.forward);
        bool isHit = Physics.Raycast(shootRay, out RaycastHit hitInfo, WeaponSettings.HitDistance, _shootRayLayerMask, QueryTriggerInteraction.Collide);
        if (isHit)
        {
            hitInfo.collider.GetComponent<IMoveable>()?.Move();
            hitInfo.collider.GetComponent<IDamageable>()?.TakeDamage(WeaponSettings.Damage);
            return;
        }

        var collidersInAttackBox = Physics.OverlapSphere(
            cameraTransform.position + cameraTransform.forward * (WeaponSettings.HitDistance - _attackRadius / 2),
            _attackRadius, _shootRayLayerMask);

        //Для гизмов
        //boxPos = cameraTransform.position + cameraTransform.forward * (WeaponSettings.HitDistance - _attackRadius / 2);
        foreach (var collider in collidersInAttackBox)
        {
            var damageable = collider.GetComponent<IDamageable>();
            if (damageable is PlayerController || damageable == null)
            {
                continue;
            }

            damageable.TakeDamage(WeaponSettings.Damage);
            break;
        }
        Debug.DrawRay(shootRay.origin, isHit ? hitInfo.point - shootRay.origin : shootRay.direction * WeaponSettings.HitDistance, isHit ? Color.green : Color.red, 2);
    }

    private void OnEnable()
    {
        _animator.SetFloat(_hashAttackSpeedFloat, WeaponSettings.ShootDelay == 0 ? 0 : 1 / WeaponSettings.ShootDelay);
        IsReloading = false;
    }

    private void OnDisable()
    {
        transform.SetLocalPositionAndRotation(_startLocalPosition, Quaternion.Euler(_startLocalRotation));
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(boxPos, _attackRadius / 2);
    //}

    //Vector3 boxPos;
}
