using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageTrigger : MeleeWeaponComponent
{
    [SerializeField] private Vector3 _sphereCastOffset;
    [SerializeField] private float _sphereCastRadius;
    private int _ownerHash;
    private int _damage;
    private CapsuleCollider _collider;
    private LayerMask _collLayer;

    public void SetSettings(int damage, LayerMask layers, int hash)
    {
        this._damage = damage;
        this._collLayer = layers;
        _collider = GetComponent<CapsuleCollider>();
        _ownerHash = hash;
    }

    public void EnableTrigger()
    {
        _collider.enabled = true;
    }

    public void DisableTrigger()
    {
        _collider.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!base.IsOwner) return;

        RaycastSphere(transform.position + _sphereCastOffset);
        ServerRaycastSphere(transform.position + _sphereCastOffset);
    }

    private void RaycastSphere(Vector3 pos)
    {
        var hits = GameManager.instance.networkContext.FishNetSphereRaycast(pos, _sphereCastRadius, _collLayer);
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Hitbox hitbox = hits[i].GetComponent<Hitbox>();
                SurfaceIdentifier surface = hits[i].GetComponent<SurfaceIdentifier>();
                Vector3 hitPos = hits[i].ClosestPointOnBounds(transform.position);

                if (hitbox != null) if (_ownerHash == hitbox.ownerHash) return;// check if we are not hitting ourselfs

                Debug.Log(hits[i].gameObject.name);

                ShowHitEffect(surface, hitPos);
                CheckForDamage(hitbox);


                if (hitbox != null)
                {
                    //Debugs
                    if (GameManager.instance.networkContext.Ownership.isServer)
                    {
                        NetworkContext.DrawSphere(pos, _collider.radius, Color.blue, 20f);
                  //      Debug.DrawRay(transform.position, hits[i].transform.position, Color.blue, 20f);
                    }
                    else
                    {
                        if (!GameManager.instance.networkContext.Ownership.isServer)
                        {
                            NetworkContext.DrawSphere(pos, _collider.radius, Color.red, 20f);
                       //     Debug.DrawRay(transform.position, hits[i].transform.position, Color.red, 20f);
                        }
                    }
                }
                else
                {
                    //Debugs
                    if (GameManager.instance.networkContext.Ownership.isServer)
                        NetworkContext.DrawSphere(pos, _collider.radius, Color.cyan, 20f);
                    else if (!GameManager.instance.networkContext.Ownership.isServer) NetworkContext.DrawSphere(pos, _collider.radius, Color.green, 20f);
                }

                DisableTrigger();
            }

        }
    }


    private void ShowHitEffect(SurfaceIdentifier surface, Vector3 hitPos)
    {
        //Show hit effect.
        if (base.IsClient)
        {
            if (surface != null)
            {
                var obj = GameManager.instance.objPool.RetrievePoolBulletImpactObject(surface);
                if (obj == null) return;
                obj.transform.position = hitPos;
                obj.transform.rotation = Quaternion.LookRotation(transform.position);
            }
        }
    }

    private void CheckForDamage(Hitbox hitbox)
    {
        //Apply damage and other server things.
        if (base.IsServer)
        {
            if (hitbox != null)
            {
                /* Melee abilities should do the same amount of damage every time,
                * and double damage it from behind. */
                int modifiedDamage = Mathf.CeilToInt((1f / hitbox.Multiplier) * _damage);

                //float angle = hitbox.TopmostParent.forward != null ? Vector3.Angle(hitbox.TopmostParent.forward, transform.forward) : -1f;
                //if (angle >= 0f && angle <= 45f)
                //    modifiedDamage *= 2;
                hitbox.Hit(modifiedDamage, _currentWeapon.playerInventoryHandler.gameObject);
            }
        }
    }

    [ServerRpc]
    private void ServerRaycastSphere(Vector3 pos)
    {
        if (!base.IsServer) return;

        if (!base.IsOwner)
            RaycastSphere(pos);

        ObserversRaycastSphere(pos);
    }

    [ObserversRpc]
    private void ObserversRaycastSphere(Vector3 pos)
    {
        if (base.IsOwner || base.IsServer) return;

        RaycastSphere(pos);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + _sphereCastOffset, _sphereCastRadius);
    }
}
