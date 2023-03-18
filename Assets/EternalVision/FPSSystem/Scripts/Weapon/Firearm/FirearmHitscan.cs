using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using EternalVision.FPS;

public class FirearmHitscan : FirearmShootCompoment
{
    [SerializeField] private int _weaponDamage = 20;
    [SerializeField] private Projectile _bulletProjectileTrace;
    [SerializeField] private LayerMask _collisionLayer;


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
            ServerFire(muzzleTipFirstPerson.position, muzzleTipFirstPerson.forward + CalculateSpread());
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

            RaycastShoot(pos, dir);
            Event_OnFire();
            isFire = true;
            _delaytoRecalculateSpread = 0f;
            _lastShootTime = Time.time;
        
    }

    [ServerRpc]
    private void ServerFire(Vector3 pos, Vector3 dir)
    {
        if (!base.IsServer) return;
        ObserversFire(pos, dir);
        if (base.IsOwner) return;
        Fire(pos, dir);
    }

    [ServerRpc]
    private void ObserversFire(Vector3 pos, Vector3 dir)
    {
        if (base.IsOwner || base.IsServer) return;
        Fire(pos, dir);
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



    private void RaycastShoot(Vector3 position ,Vector3 direction)
    {
        var hit = GameManager.instance.networkContext.RaycastWithFishnet(position, direction, _collisionLayer);

        UpdateSpread();
        CheckForDamage(hit);
        SetVisuals(hit, direction);
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

        //Only for visuals thirdperson/firstperson
        Vector3 visualExitPoint = _currentWeapon.ownership.isOwner ? muzzleTipFirstPerson.position : _weaponFireArm.weaponModule.muzzleTip.position;

        //Pooling the projectile
        Projectile pj = _objectPool.RetrievePoolBulletObject(_bulletProjectileTrace);
        if (pj == null) return;
        //Setting projectile Position
        pj.transform.position = visualExitPoint;
        //adding the projectile
        Vector3 visualBulletDirection = hit.collider != null ? hit.point : direction * 100f;
        //  visualBulletDirection = networkOwnership.isOwner ? visualBulletDirection : (_weaponFireArm.weaponModule.muzzleTip.forward + spreadCast) * 100f;
        pj.AddProjectile(visualExitPoint, visualBulletDirection, surface);
    }

    private void CheckForDamage(RaycastHit hit)
    {
  
        if (hit.collider == null) return;

        I_Hitbox hitbox = hit.collider.GetComponent<I_Hitbox>();
        if (hitbox != null)
        {
            Event_OnHitTarget(_currentWeapon);
            if (GameManager.instance.networkContext.Ownership.isServer)
                hitbox.Hit(_weaponDamage, _currentWeapon.playerInventoryHandler.gameObject);
        }
    }

    public override void GetPooledObjectReference(ObjectPool component)
    {
        Debug.Log("getting Pool " + component != null ? "availble" : "Null");
        _objectPool = component;
    }
}
