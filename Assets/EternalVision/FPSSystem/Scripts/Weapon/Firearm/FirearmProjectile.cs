using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmProjectile : FirearmShootCompoment
{

    /// <summary>
    /// Projectile to spawn.
    /// </summary>
    [Tooltip("Projectile to spawn.")]
    [SerializeField]
    private PredictedProjectile _projectile;
    /// <summary>
    /// Maximum amount of passed time a projectile may have.
    /// This ensures really laggy players won't be able to disrupt
    /// other players by having the projectile speed up beyond
    /// reason on their screens.
    /// </summary>
    private const float MAX_PASSED_TIME = 0.3f;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        if (!_currentWeapon.weaponAmmoComponent.isEmpty)
        {
            Fire(muzzleTipFirstPerson.position, muzzleTipFirstPerson.forward + CalculateSpread());
        }
        else
        {
            FireEmpty();
            ObserversFireEmpty();
        }

    }


    private void Fire(Vector3 pos, Vector3 dir)
    {
        if (base.IsOwner)
        {
            PlayerEffects();
        }
        else if (!base.IsOwner)
            _weaponFireArm.weaponModule.Shoot("Fire");

        SpawnProjectile(pos, dir, 0f);
        //Ask server to also fire passing in current Tick.
        ServerFire(pos, dir, base.TimeManager.Tick);
        Event_OnFire();
        isFire = true;
        _delaytoRecalculateSpread = 0f;
        _lastShootTime = Time.time;

    }

    private void SpawnProjectile(Vector3 position, Vector3 direction, float passedTime)
    {
        var bullet = _objectPool.RetrievePoolBulletObject(_projectile);
        if (bullet != null)
            bullet.AddProjectile(position, direction, passedTime);
    }


    [ServerRpc]
    private void ServerFire(Vector3 position, Vector3 direction, uint tick)
    {
        /* You may want to validate position and direction here.
         * How this is done depends largely upon your game so it
         * won't be covered in this guide. */

        //Get passed time. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        /* Cap passed time at half of constant value for the server.
         * In our example max passed time is 300ms, so server value
         * would be max 150ms. This means if it took a client longer
         * than 150ms to send the rpc to the server, the time would
         * be capped to 150ms. This might sound restrictive, but that would
         * mean the client would have roughly a 300ms ping; we do not want
         * to punish other players because a laggy client is firing. */
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        //Spawn on the server.
        if (!base.IsServer) return;
        if (!base.IsOwner)
            SpawnProjectile(position, direction, passedTime);
        //Tell other clients to spawn the projectile.
        ObserversFire(position, direction, tick);
    }

    /// <summary>
    /// Fires on all clients but owner.
    /// </summary>
    [ObserversRpc(IncludeOwner = false)]
    private void ObserversFire(Vector3 position, Vector3 direction, uint tick)
    {
        //Like on server get the time passed and cap it. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        //Spawn the projectile locally.
        SpawnProjectile(position, direction, passedTime);
    }


    //[ServerRpc]
    //private void ServerFire(Vector3 pos, Vector3 dir)
    //{
    //    if (!base.IsServer) return;
    //    ObserversFire(pos, dir);
    //    if (base.IsOwner) return;
    //    Fire(pos, dir);
    //}


    private void FireEmpty()
    {
        Event_OnFireEmpty();
        _lastShootTime = Time.time;
    }

    [ObserversRpc]
    private void ObserversFireEmpty()
    {
        if (base.IsOwner) return;
        FireEmpty();
    }

    private void PlayerEffects()
    {
        _muzzlEffect.Play(true);
    }



    public override void GetPooledObjectReference(ObjectPool component)
    {
        _objectPool = component;
    }

}
