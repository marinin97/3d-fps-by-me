using System.Collections;
using UnityEngine;

public class Revolver : Weapon
{
    [SerializeField]
    private Transform _bulletHolePrefab;

    private float _timeBeforeShoot;
    [SerializeField]
    private ParticleSystem _muzzleFlash;

    #region Animations
    private Animator _animator;
    private int _hashShootTrigger;
    private int _hashReloadTrigger;
    private int _hashShootSpeedFloat;
    private int _hashReloadSpeedFloat;
    #endregion

    private Vector3 _startLocalPosition;
    private Vector3 _startLocalRotation;

    private WaitForSeconds _reloadAnimTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _hashShootTrigger = Animator.StringToHash("ShootRevolver");
        _hashShootSpeedFloat = Animator.StringToHash("ShootSpeed");
        _hashReloadTrigger = Animator.StringToHash("ReloadRevolver");
        _hashReloadSpeedFloat = Animator.StringToHash("ReloadSpeed");
        TotalAmmo = WeaponSettings.TotalAmmo;
        CurrentMagazineAmmo = WeaponSettings.MagazineAmmo;
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
        if (_timeBeforeShoot > 0 || IsReloading || fireIsPressed)
        {
            return;
        }

        if (CurrentMagazineAmmo > 0)
        {
            _animator.SetTrigger(_hashShootTrigger);
            _timeBeforeShoot = WeaponSettings.ShootDelay;
            Ray shootRay = new(cameraTransform.position, cameraTransform.forward);
            bool isHit = Physics.Raycast(shootRay, out RaycastHit hitInfo, WeaponSettings.HitDistance, _shootRayLayerMask, QueryTriggerInteraction.Collide);
            if (isHit)
            {
                var damageable = hitInfo.collider.GetComponent<IDamageable>();
                if (damageable is not Hitbox)
                {
                    var bulletHole = Instantiate(_bulletHolePrefab);
                    bulletHole.SetPositionAndRotation(hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    bulletHole.Translate(transform.forward * 0.02f);
                    bulletHole.SetParent(hitInfo.transform);
                    Destroy(bulletHole.gameObject, 5);
                }

                damageable?.TakeDamage(WeaponSettings.Damage);
                hitInfo.collider.GetComponent<IMoveable>()?.Move();
            }
            //Debug.DrawRay(shootRay.origin, isHit ? hitInfo.point - shootRay.origin : shootRay.direction * WeaponSettings.HitDistance, isHit ? Color.green : Color.red, 2);
            CurrentMagazineAmmo--;
            _muzzleFlash.Play();
            OnShootInvoke();
        }
    }
}
