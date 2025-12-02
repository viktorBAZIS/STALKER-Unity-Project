using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "STALKER/Weapon Data")]
public class StalkerWeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float fireRate;
    public float accuracy;
    public float range;
    public int maxAmmo;
    public float reloadTime;
}