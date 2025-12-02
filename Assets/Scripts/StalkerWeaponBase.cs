using UnityEngine;

public class StalkerWeaponBase : MonoBehaviour
{
    [Header("STALKER Weapon Settings")]
    public StalkerWeaponData weaponData;
    public Transform muzzlePoint;
    public Animator weaponAnimator;

    // STALKER особенности:
    public WeaponCondition condition; // состояние оружия
    public float jamChance;           // вероятность заклинивания
    public AnomalyEffect anomalyEffect; // влияние аномалий на оружие
}

public enum WeaponCondition
{
    Perfect,    // 100-80%
    Good,       // 79-60%  
    Worn,       // 59-40%
    Poor,       // 39-20%
    Broken      // 19-0%
}

// Добавьте этот класс если он у вас отсутствует
public class AnomalyEffect : MonoBehaviour
{
    // Реализация эффектов аномалий на оружие
}