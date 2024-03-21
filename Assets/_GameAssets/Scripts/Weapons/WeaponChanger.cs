using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponChanger
{
    public event Action<Weapon> OnWeaponChanged;

    [SerializeField]
    private List<Weapon> _weapons;

    public Weapon CurrentWeapon { get; private set; }

    public Weapon GetWeapon(int weaponIndex) => _weapons[weaponIndex];

    public void ChangeWeapon(int weaponIndex)
    {
        if (weaponIndex >= _weapons.Count)
        {
            Debug.LogWarning($"No weapon with index {weaponIndex}!");
            return;
        }

        Weapon newWeapon = GetWeapon(weaponIndex);
        if (CurrentWeapon == newWeapon)
        {
            return;
        }
        if (CurrentWeapon != null)
        {
            CurrentWeapon.gameObject.SetActive(false);
        }
        CurrentWeapon = newWeapon;
        CurrentWeapon.gameObject.SetActive(true);
        OnWeaponChanged?.Invoke(CurrentWeapon);
    }

    public void DisableAllWeapons()
    {
        _weapons.ForEach(weapon => weapon.gameObject.SetActive(false));
    }
}
