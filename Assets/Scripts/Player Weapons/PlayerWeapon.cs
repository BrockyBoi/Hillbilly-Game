using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponModifiers
{
    Speed, Number, Size, FireRate
}

public abstract class PlayerWeapon : MonoBehaviour
{
    [Title("Weapon Id")]
    protected string Id = string.Empty;
    public string WeaponId { get { return Id; } }

    [Title("Weapon Properties")]
    [SerializeField]
    protected float _defaultWeaponSpeed = 5;
    protected float _weaponSpeedModifier = 1f;

    [SerializeField]
    protected int _defaultNumberOfProjectiles = 1;
    protected int _weaponProjectileNumberModifier = 1;

    [SerializeField]
    protected float _defaultWeaponSize = 1f;
    protected float _weaponSizeModifier = 1f;

    [SerializeField]
    protected float _defaultFireRate = 1f;
    [SerializeField]
    protected float _maxFireRate = .2f;
    protected float _weaponFireRateModifier = 1f;

    [Title("Prefab")]
    [SerializeField]
    protected GameObject _projectilePrefab;

    private WaitForSeconds _waitForNextFire;

    public void Initialize()
    {
        _waitForNextFire = new WaitForSeconds(_defaultFireRate);
        StartCoroutine(FireWeaponCoroutine());
    }

    IEnumerator FireWeaponCoroutine()
    {
        while (MainPlayer.Instance.IsAlive())
        {
            yield return _waitForNextFire;

            FireWeapon();
        }
    }

    protected abstract void FireWeapon();

    public void ModifyWeapon(WeaponModifiers modifier, float changeAmount)
    {
        switch (modifier)
        {
            case WeaponModifiers.Number:
                {
                    _weaponSpeedModifier += changeAmount;
                    break;
                }

            case WeaponModifiers.Size:
                {
                    _weaponSizeModifier += changeAmount;
                    break;
                }

            case WeaponModifiers.Speed:
                {
                    _weaponSpeedModifier += changeAmount;
                    break;
                }

            case WeaponModifiers.FireRate:
                {
                    _weaponFireRateModifier = Mathf.Clamp(_weaponFireRateModifier - changeAmount, _maxFireRate, _defaultFireRate);
                    break;
                }

            default:
                {
                    break;
                }
        }
    }
}
