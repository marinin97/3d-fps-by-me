using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public event Action<Weapon> OnShoot;
    public event Action<Weapon> OnReload;

    [field: SerializeField]
    public WeaponSettings WeaponSettings { get; protected set; }
    [SerializeField]
    protected LayerMask _shootRayLayerMask;

    public int CurrentMagazineAmmo { get; protected set; }
    public int TotalAmmo { get; protected set; }
    public bool IsReloading { get; protected set; }

    public abstract void Shoot(bool fireIsPressed, Transform cameraTransform);
    public abstract void Reload();

    public void AddAmmo(int ammoCount)
    {
        int newTotalAmmo = TotalAmmo + ammoCount;
        TotalAmmo = newTotalAmmo < WeaponSettings.TotalAmmo ? newTotalAmmo : WeaponSettings.TotalAmmo;
        OnReloadInvoke();
    }


    protected void OnShootInvoke()
    {
        OnShoot?.Invoke(this);
    }

    protected void OnReloadInvoke()
    {
        OnReload?.Invoke(this);
    }
}

