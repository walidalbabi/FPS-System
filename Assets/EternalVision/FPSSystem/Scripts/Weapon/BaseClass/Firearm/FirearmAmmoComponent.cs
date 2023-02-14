using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FirearmAmmoComponent : FirearmComponent
{
    [Tooltip("Max Ammo this Weapon Can Hold")]
    [SerializeField] protected int _maxAmmo;
    [Tooltip("Max Ammo this Weapon Can have in a single mag")]
    [SerializeField] protected int _magAmmo;
    [SerializeField] protected bool _autoReloadOnEmpty;
    [SerializeField] protected bool _resetAmmoAfterPlayerRespawn;

    protected int _currentAmmoCount;
    protected int _totalAmmo;
    protected bool _isEmpty { get { return _currentAmmoCount <= 0; } }


    public static event Action<WeaponBehaviour, WeaponSoundsState> OnReload;
    public static event Action<WeaponBehaviour,bool> OnReloadStateChange;
    /// <summary>
    /// USed when the magazine change with old , current and TotalAmmo
    /// </summary>
    public static event Action<WeaponBehaviour,int,int,int> OnAmmoChange;

    public bool isEmpty { get { return _isEmpty; } }
    public bool isReloading { get; set; }
    public int currentAmmoCount { get { return _currentAmmoCount; } }
    public int maxAmmoMag { get { return _magAmmo; } }
    public int totalAmmo { get { return _totalAmmo; } }


    public virtual void Awake()
    {
        FirearmShootCompoment.OnFire += OnFire;
    }


    public override void SetWeaponOwner(WeaponBehaviour component)
    {
        base.SetWeaponOwner(component);

        if (_resetAmmoAfterPlayerRespawn)
        {
        //    _currentWeapon.playerInventoryHandler.GetComponent<PlayerHealth>().OnRespawned += ResetAmmo;
        }
    }


    public virtual void OnDestroy()
    {
        FirearmShootCompoment.OnFire -= OnFire;

        if (_resetAmmoAfterPlayerRespawn)
        {
          //  _currentWeapon.playerInventoryHandler.GetComponent<PlayerHealth>().OnRespawned -= ResetAmmo;
        }
    }

    public virtual void Start()
    {
        Event_CallOnAmmoChange(_currentWeapon, _currentAmmoCount, _currentAmmoCount, _totalAmmo);
    }

    public abstract void Reload();
    public virtual void OnFire(WeaponBehaviour weapon, NetworkOwnership ownership)
    {
        if (weapon != _currentWeapon) return;


        if (_currentAmmoCount > 0)
        {
            int oldAmmot = _currentAmmoCount;
            _currentAmmoCount--;
            Event_CallOnAmmoChange(_currentWeapon, oldAmmot, _currentAmmoCount, _totalAmmo);
        }
        else if (_autoReloadOnEmpty) { 
            _currentWeapon.Reload();
        }
    }


    public void ResetAmmo()
    {
        _totalAmmo = _maxAmmo;
        _currentAmmoCount = _magAmmo;
    }

    public void Event_CallOnReload(WeaponBehaviour weapon, WeaponSoundsState state)
    {
        OnReload?.Invoke(weapon, state);
    }

    /// <summary>
    /// True if we start reloading, false if we finish reloading
    /// </summary>
    /// <param name="weapon"></param>
    /// <param name="state"></param>
    public void Event_CallOnReloadStateChange(WeaponBehaviour weapon , bool state)
    {
        OnReloadStateChange?.Invoke(weapon, state);
    }

    public void Event_CallOnAmmoChange(WeaponBehaviour weapon, int oldValue, int currentValue, int totalAmmo)
    {
        OnAmmoChange?.Invoke(weapon, oldValue, currentValue, totalAmmo);
    }

    public bool CanReload()
    {
        if (isReloading) return false;
        if (_totalAmmo <= 0) return false;

        return true;
    }
}
