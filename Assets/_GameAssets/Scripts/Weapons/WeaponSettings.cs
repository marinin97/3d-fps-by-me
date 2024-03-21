using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Settings", menuName = "Settings/Weapon")]
public class WeaponSettings : ScriptableObject
{

    [field: SerializeField]
    public int TotalAmmo { get; private set; }
    [field: SerializeField]
    public int MagazineAmmo { get; private set; }
    [field :SerializeField]
    public float HitDistance { get; private set; }
    [field: SerializeField]
    public float Damage { get; private set; }
    [field: SerializeField]
    public float ShootDelay { get; private set; }
    [field: SerializeField]
    public float ReloadTime { get; private set; }
}
