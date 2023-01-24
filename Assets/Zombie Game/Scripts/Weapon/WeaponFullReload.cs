using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFullReload : FirearmAmmoComponent 
{

    [SerializeField] private bool _infiniteAmmo;
    [SerializeField] private bool _fullOnStart;

    public override void Awake()
    {
        base.Awake();
        if (_fullOnStart)
        {
            _totalAmmo = _maxAmmo;
            _currentAmmoCount = _magAmmo;
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Reload()
    {
        var ammo = _magAmmo - _currentAmmoCount;
        var increasedAmmo = 0;
        var oldAmmo = _currentAmmoCount;
        _currentAmmoCount = _totalAmmo >= ammo ? (_currentAmmoCount + ammo) : _currentAmmoCount + _totalAmmo;
        increasedAmmo = _totalAmmo >= ammo ? ammo : _totalAmmo;

        if (!_infiniteAmmo)
            _totalAmmo = _totalAmmo - increasedAmmo;

        isReloading = false;
        Event_CallOnReloadStateChange(_currentWeapon, false);
        Event_CallOnAmmoChange(_currentWeapon, oldAmmo, _currentAmmoCount, _totalAmmo);
    }

    public override void OnFire(WeaponBehaviour weapon, NetworkOwnership ownership)
    {
        base.OnFire(weapon, ownership);
    }
}
