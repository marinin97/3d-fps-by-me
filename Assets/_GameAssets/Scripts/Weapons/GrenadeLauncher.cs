using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : Weapon
{
    [SerializeField]
    private Grenade _grenadePrefab;
    [SerializeField]
    private Transform _shootPointTransform;

    private float _timeBeforeShoot;

    #region Animations
    private Animator _animator;
    private int _hashShootTrigger;
    private int _hashShootSpeedFloat;
    private int _hashReloadTrigger;
    private int _hashReloadSpeedFloat;
    #endregion

    private Vector3 _startLocalPosition;
    private Vector3 _startLocalRotation;

    private WaitForSeconds _reloadAnimTime;


    private void Awake()
    {
        TotalAmmo = WeaponSettings.TotalAmmo;
        CurrentMagazineAmmo = WeaponSettings.MagazineAmmo;

        _animator = GetComponent<Animator>();
        _hashShootSpeedFloat = Animator.StringToHash("ShootSpeed");
        _hashShootTrigger = Animator.StringToHash("ShootGrenadeLauncher");
        _hashReloadTrigger = Animator.StringToHash("ReloadGrenadeLauncher");
        _hashReloadSpeedFloat = Animator.StringToHash("ReloadSpeed");
        _animator.keepAnimatorStateOnDisable = false;
        _startLocalPosition = transform.localPosition;
        _startLocalRotation = transform.localRotation.eulerAngles;
        _reloadAnimTime = new WaitForSeconds(WeaponSettings.ReloadTime);
    }

    private void Update()
    {
        _timeBeforeShoot -= Time.deltaTime;
        _timeBeforeShoot = _timeBeforeShoot < 0 ? 0 : _timeBeforeShoot;
    }

    private void OnEnable()
    {
        _animator.SetFloat(_hashShootSpeedFloat, WeaponSettings.ShootDelay == 0 ? 0 : 1 / WeaponSettings.ShootDelay);
        _animator.SetFloat(_hashReloadSpeedFloat, WeaponSettings.ReloadTime == 0 ? 0 : 1 / WeaponSettings.ReloadTime);
        IsReloading = false;
    }

    private void OnDisable()
    {
        transform.SetLocalPositionAndRotation(_startLocalPosition, Quaternion.Euler(_startLocalRotation));
    }

    public override void Reload()
    {
        if (CurrentMagazineAmmo == WeaponSettings.MagazineAmmo || TotalAmmo == 0 || IsReloading)
        {
            return;
        }

        int ammoNeeded = WeaponSettings.MagazineAmmo - CurrentMagazineAmmo;

        if (TotalAmmo - ammoNeeded < 0)
        {
            ammoNeeded = TotalAmmo;
        }

        _animator.SetTrigger(_hashReloadTrigger);
        StartCoroutine(StartReloadDelay(ammoNeeded));
    }
    private IEnumerator StartReloadDelay(int ammoNeeded)
    {
        IsReloading = true;
        yield return _reloadAnimTime;
        TotalAmmo -= ammoNeeded;
        CurrentMagazineAmmo += ammoNeeded;
        IsReloading = false;
        OnReloadInvoke();
    }

    public override void Shoot(bool fireIsPressed, Transform cameraTransform)
    {
        if (_timeBeforeShoot > 0 || IsReloading)
        {
            return;
        }
        if (CurrentMagazineAmmo > 0)
        {
            _animator.SetTrigger(_hashShootTrigger);
            _timeBeforeShoot = WeaponSettings.ShootDelay;

            int shootDistance = 1000;
            Vector3 targetPoint = transform.forward * shootDistance;
            _timeBeforeShoot = WeaponSettings.ShootDelay;
            Ray shootRay = new(cameraTransform.position, cameraTransform.forward);
            bool isHit = Physics.Raycast(shootRay, out RaycastHit hitInfo, WeaponSettings.HitDistance, _shootRayLayerMask, QueryTriggerInteraction.Collide);
            if (isHit)
            {
                targetPoint = hitInfo.point;
            }

            Instantiate(_grenadePrefab, _shootPointTransform.position, Quaternion.identity)
                .Shoot(WeaponSettings.Damage, targetPoint - _shootPointTransform.position);

            Debug.DrawRay(shootRay.origin, isHit ? hitInfo.point - shootRay.origin : shootRay.direction * WeaponSettings.HitDistance, isHit ? Color.green : Color.red, 2);
            CurrentMagazineAmmo--;
            OnShootInvoke();
        }
    }
}
