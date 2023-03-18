using EternalVision.FPS;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Timing;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmAdvancedHitscan : FirearmShootCompoment
{

    [SerializeField] private float _bulletSpeed = 1000f;
    [SerializeField] private float _bulletDrop = 0f;
    [SerializeField] private float _maxLifeTime = 5f;
    [SerializeField] private int _damage = 25;


    [Tooltip("Projectile to spawn.")]
    [SerializeField] private AdvancedHitscanBulletProjectile _projectile;
    [SerializeField] private LayerMask _collisionLayer;
    /// <summary>
    /// Maximum amount of passed time a projectile may have.
    /// This ensures really laggy players won't be able to disrupt
    /// other players by having the projectile speed up beyond
    /// reason on their screens.
    /// </summary>
    private const float MAX_PASSED_TIME = 0.3f;

    private List<Bullet> _bullets = new List<Bullet>();

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * _bulletDrop;
        return (bullet.initialPos) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity, float passedTime)
    {
        Bullet bullet = new Bullet();
        bullet.initialPos = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0f;
        bullet.passedTime = passedTime;
        if (!base.IsServer)
        {
            bullet.bulletProjectile = _objectPool.RetrievePoolBulletObject(_projectile);
            bullet.bulletProjectile.AddProjectile(_muzzleTipFirstPerson.position, Vector3.zero, 0f);
        }
        return bullet;
    }

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
        UpdateBullet(Time.deltaTime);

    }

    public override void Shoot()
    {
        if (!CanShoot()) return;

        if (!_currentWeapon.weaponAmmoComponent.isEmpty)
        {
            var dir = muzzleTipFirstPerson.forward + CalculateSpread();
            Fire(muzzleTipFirstPerson.position, dir);
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
        Vector3 velocity =  (direction).normalized * _bulletSpeed;
        var bullet = CreateBullet(position, velocity, passedTime);
        if (bullet != null)
        {
            _bullets.Add(bullet);
        }
        UpdateSpread();
    }

    private void UpdateBullet(float deltaTime)
    {
        SimulateBullets(deltaTime);
        RemoveBullet();
    }

    private void SimulateBullets(float deltaTime)
    {
        if (_bullets.Count <= 0) return;

        for (int i = 0; i < _bullets.Count; i++)
        {
            if (_bullets == null) return;

            Vector3 p0 = GetPosition(_bullets[i]);
            //See if to add on additional delta to consume passed time.
            float passedTimeDelta = 0f;
            if (_bullets[i].passedTime > 0f)
            {
                /* Rather than use a flat catch up rate the
                 * extra delta will be based on how much passed time
                 * remains. This means the projectile will accelerate
                 * faster at the beginning and slower at the end.
                 * If a flat rate was used then the projectile
                 * would accelerate at a constant rate, then abruptly
                 * change to normal move rate. This is similar to using
                 * a smooth damp. */

                /* Apply 8% of the step per frame. You can adjust
                 * this number to whatever feels good. */
                float step = (_bullets[i].passedTime * 0.15f);
                _bullets[i].passedTime -= step;

                /* If the remaining time is less than half a delta then
                 * just append it onto the step. The change won't be noticeable. */
                if (_bullets[i].passedTime <= (deltaTime / 2f))
                {
                    step += _bullets[i].passedTime;
                    _bullets[i].passedTime = 0f;
                }
                passedTimeDelta = step;
            }
            _bullets[i].time += (deltaTime + passedTimeDelta);
            Vector3 p1 = GetPosition(_bullets[i]);
            RaycastSegments(p0, p1, _bullets[i]);
            if (!base.IsServer)
            {
                if (_bullets[i].bulletProjectile != null)
                    _bullets[i].bulletProjectile.transform.position = p0;
            }
          
        }
    }

    private void RaycastSegments(Vector3 start, Vector3 end, Bullet bullet)
    {
        RaycastHit hit = GameManager.instance.networkContext.AdvancedRaycastWithFishnet(start, end, _collisionLayer);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider);
            SetVisuals(hit, Vector3.zero);
            CheckForDamage(hit);
            if (!base.IsServer)
                bullet.bulletProjectile.SendBackToPool();

            bullet.time = _maxLifeTime;
        }
    }

    private void RemoveBullet()
    {
        _bullets.RemoveAll(bullet => bullet.time > _maxLifeTime);
    }

    private void SetVisuals(RaycastHit hit, Vector3 direction)
    {
        SurfaceIdentifier surface = null;

        //Show hit effect. only if we are client
        if (GameManager.instance.networkContext.Ownership.isClient)
        {
            if (hit.collider != null)
            {
                surface = hit.collider.GetComponent<SurfaceIdentifier>();
                if (surface != null)
                {
                    var obj = _objectPool.RetrievePoolBulletImpactObject(surface);
                    if (obj == null) return;
                    obj.transform.position = hit.point;
                    obj.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
            }
        }
    }

    private void CheckForDamage(RaycastHit hit)
    {

        if (hit.collider == null) return;

        I_Hitbox hitbox = hit.collider.GetComponent<I_Hitbox>();
        if (hitbox != null)
        {
          
            if (GameManager.instance.networkContext.Ownership.isServer)
            {
                hitbox.Hit(_damage, _currentWeapon.playerInventoryHandler.gameObject);
                TargetOnHitEnemy(base.Owner);
            }
        }
    }

    [TargetRpc]
    private void TargetOnHitEnemy(NetworkConnection target)
    {
        Event_OnHitTarget(_currentWeapon);
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
        {
            Event_OnFire();
            SpawnProjectile(position, direction, passedTime);
        }
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

        if (!base.IsOwner && !base.IsServer)
        {
            if (_weaponFireArm.weaponModule != null)
                _weaponFireArm.weaponModule.Shoot("Fire");
            Event_OnFire();
            //Spawn the projectile locally.
            SpawnProjectile(position, direction, passedTime);
        }
  
    }


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



public class Bullet
{
    public float time;
    public float passedTime;
    public Vector3 initialPos;
    public Vector3 initialVelocity;
    public Projectile bulletProjectile;
}

