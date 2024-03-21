using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoContainer : LootContainer
{
    [SerializeField]
    private List<WeaponAmmoDropCount> _weaponsDropCounts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            var currentWeapon = player.WeaponChanger.CurrentWeapon;
            if (Enum.TryParse<WeaponType>(currentWeapon.GetType().ToString(), out WeaponType weaponType))
            {
                if (weaponType == WeaponType.Knife)
                {
                    return;
                }
                var dropCount = _weaponsDropCounts.Find(drop => drop.WeaponType == weaponType);
                int ammoCountToAdd = dropCount.MultiplyMagazineCount ?
                    (int)Math.Ceiling(currentWeapon.WeaponSettings.MagazineAmmo * dropCount.MagazineMultiplier) :
                    dropCount.AmmoCount;

                currentWeapon.AddAmmo(ammoCountToAdd);
            }
            Destroy(gameObject);
        }
    }
}

[Serializable]
public sealed class WeaponAmmoDropCount
{
    public WeaponType WeaponType;
    public bool MultiplyMagazineCount;
    public float MagazineMultiplier;
    public int AmmoCount;
}

public enum WeaponType
{
    Knife,
    Revolver,
    AssaultRifle,
    GrenadeLauncher
}
