
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FirearmShootCompoment : FirearmComponent
{
    [Header("Settings")]
    [Tooltip("How many Shots per minute")]
    [SerializeField] protected float _cadence = 600f;
    [SerializeField] protected float _maxSpread = 2.5f;
    [SerializeField] protected float _spreadMultiplier = 0.3f;
    [SerializeField] protected float _spreadRecoverSpeed = 4f;
    [Header("Components")]
    [SerializeField] protected Transform _muzzleTipFirstPerson;
    [SerializeField] protected ParticleSystem _muzzlEffect;

    protected float _lastShootTime;
    protected float _delaytoRecalculateSpread;
    protected float _spread;
    protected float _fireRate;
    protected NetworkOwnership _networkOwnership;
    protected ObjectPool _objectPool;
    protected ModularFirearm _weaponFireArm;


    public Transform muzzleTipFirstPerson { get { return _muzzleTipFirstPerson; } }

    public static event Action<WeaponBehaviour, NetworkOwnership> OnFire;
    public static event Action<WeaponBehaviour> OnFireEmpty;
    public static event Action<WeaponBehaviour> OnHitATarget;



    public bool isFire { get; protected set; }


    public virtual void Awake()
    {
        _weaponFireArm = (ModularFirearm)_currentWeapon;

        _objectPool = GameManager.instance.objPool;
        _fireRate = 60f / _cadence;
    }


    public virtual void OnDestroy()
    {
      //  ObjectPool.OnPooledObjectsAdded -= GetPooledObjectReference;
    }

    public virtual void LateUpdate()
    {

        //Do not reset the spread if we are still firing
        if (_delaytoRecalculateSpread < .5f && isFire)
        {
            _delaytoRecalculateSpread += Time.deltaTime;
        }
        else isFire = false;

        //Reset spread over time
        if (_spread > 0 && !isFire)
            _spread -= Time.deltaTime * _spreadRecoverSpeed;
    }

    public abstract void Shoot();

    public void Event_OnFire()
    {
        OnFire?.Invoke(_currentWeapon, _networkOwnership);
    }

    public void Event_OnFireEmpty()
    {
        OnFireEmpty?.Invoke(_currentWeapon);
    }

    public void Event_OnHitTarget(WeaponBehaviour weapon)
    {
        OnHitATarget?.Invoke(weapon);
    }

    public abstract void GetPooledObjectReference(ObjectPool component);


    protected void UpdateSpread()
    {
        if (_spread < _maxSpread)
            _spread += _spreadMultiplier;
    }

    protected Vector3 CalculateSpread()
    {
        //Calculating spread
        Vector3 spreadCast = Vector3.zero;
        if (_spread > 0f && !_currentWeapon.isAiming)
            spreadCast = UnityEngine.Random.insideUnitSphere * _spread;

        return spreadCast;
    }


    public bool CanShoot() { return Time.time > (_lastShootTime + _fireRate); }


}
