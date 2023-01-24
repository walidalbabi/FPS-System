using FishNet.Object;
using FishNet.Serializing.Helping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkContext : NetworkBehaviour
{

    public NetworkOwnership Ownership;


    public override void OnStartServer()
    {
        base.OnStartServer();
        Ownership.CreateNew(false, base.IsClient, base.IsServer, base.IsHost);
    }

    public void CreateOwnership(bool isOwner , bool isClient, bool isServer, bool isHost)
    {
         Ownership.CreateNew(isOwner, isClient, isServer, isHost);
    }

    public RaycastHit RaycastWithFishnet(Vector3 position, Vector3 forward, LayerMask layer)
    {
        var pt = base.TimeManager.GetPreciseTick(FishNet.Managing.Timing.TickType.Tick);
        var _rollbackManager = base.RollbackManager;


        //Rollback only if a rollback time
        bool rollingBack = !Comparers.IsDefault(pt);
        //If a rollbackTime exist then rollback colliders before firing.
        if (rollingBack && GameManager.instance.networkContext.Ownership.isServer)
            _rollbackManager.Rollback(pt, FishNet.Component.ColliderRollback.RollbackManager.PhysicsType.ThreeDimensional);

        Ray ray = new Ray(position, forward);
        RaycastHit hit;
        //If ray hits.
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layer))
        {
            Hitbox hitbox = hit.collider.GetComponent<Hitbox>();

            if (!GameManager.instance.networkContext.Ownership.isServer)
            {
                if (hitbox != null) Debug.DrawRay(position, forward * 100f, Color.red, 20f);
                else Debug.DrawRay(position, forward  * 100f, Color.green, 20f);
            }

            //Apply damage and other server things.
            if (GameManager.instance.networkContext.Ownership.isServer)
            {
                if (hitbox != null)
                {
                    Debug.DrawRay(position, (forward) * 100f, Color.blue, 20f);
                    DrawSphere(hit.point, 0.05f, Color.red, 20f);
                }
                else
                {
                    Debug.DrawRay(position, (forward) * 100f, Color.cyan, 20f);
                    DrawSphere(hit.point, 0.05f, Color.green, 20f);
                }
            }
        }

        if (rollingBack && GameManager.instance.networkContext.Ownership.isServer)
            _rollbackManager.Return();

        return hit;
    }

    public RaycastHit AdvancedRaycastWithFishnet(Vector3 position, Vector3 forward, LayerMask layer)
    {
        var pt = base.TimeManager.GetPreciseTick(FishNet.Managing.Timing.TickType.Tick);
        var _rollbackManager = base.RollbackManager;


        //Rollback only if a rollback time
        bool rollingBack = !Comparers.IsDefault(pt);
        //If a rollbackTime exist then rollback colliders before firing.
        if (rollingBack && GameManager.instance.networkContext.Ownership.isServer)
            _rollbackManager.Rollback(pt, FishNet.Component.ColliderRollback.RollbackManager.PhysicsType.ThreeDimensional);

        float distance = (forward - position).magnitude;
        var dir = forward - position;
        Ray ray = new Ray(position, dir);
        RaycastHit hit;
        //If ray hits.
        if (Physics.Raycast(ray, out hit, distance, layer))
        {
            Hitbox hitbox = hit.collider.GetComponent<Hitbox>();

            if (!GameManager.instance.networkContext.Ownership.isServer)
            {
                if (hitbox != null) Debug.DrawRay(position, dir * distance * 2f, Color.red, 20f);
                else Debug.DrawRay(position, dir * distance * 2f, Color.green, 20f);
            }

            //Apply damage and other server things.
            if (GameManager.instance.networkContext.Ownership.isServer)
            {
                if (hitbox != null)
                {
                    Debug.DrawRay(position, (dir) * distance * 2f, Color.blue, 20f);
                    DrawSphere(hit.point, 0.05f, Color.red, 20f);
                }
                else
                {
                    Debug.DrawRay(position, (dir) * distance * 2f, Color.cyan, 20f);
                    DrawSphere(hit.point, 0.05f, Color.green, 20f);
                }
            }
        }

        if (rollingBack && GameManager.instance.networkContext.Ownership.isServer)
            _rollbackManager.Return();

        return hit;
    }

    public Collider[] FishNetSphereRaycast(Vector3 position, float radius, LayerMask layer)
    {
        var pt = base.TimeManager.GetPreciseTick(FishNet.Managing.Timing.TickType.Tick);
        var _rollbackManager = base.RollbackManager;


        //Rollback only if a rollback time
        bool rollingBack = !Comparers.IsDefault(pt);
        //If a rollbackTime exist then rollback colliders before firing.
        if (rollingBack && GameManager.instance.networkContext.Ownership.isServer)
            _rollbackManager.Rollback(pt, FishNet.Component.ColliderRollback.RollbackManager.PhysicsType.ThreeDimensional);


        //If ray hits.
        Collider[] hits = Physics.OverlapSphere(position, radius, layer);
        {


            for (int i = 0; i < hits.Length; i++)
            {
              //  DrawSphere(position ,radius, Color.white, 10f);
            }
            if (rollingBack && GameManager.instance.networkContext.Ownership.isServer)
                _rollbackManager.Return();

            return hits;
        }
    }

    // Sphere with radius of 1
    private static readonly Vector4[] s_UnitSphere = MakeUnitSphere(16);

    private static Vector4[] MakeUnitSphere(int len)
    {
        Debug.Assert(len > 2);
        var v = new Vector4[len * 3];
        for (int i = 0; i < len; i++)
        {
            var f = i / (float)len;
            float c = Mathf.Cos(f * (float)(Math.PI * 2.0));
            float s = Mathf.Sin(f * (float)(Math.PI * 2.0));
            v[0 * len + i] = new Vector4(c, s, 0, 1);
            v[1 * len + i] = new Vector4(0, c, s, 1);
            v[2 * len + i] = new Vector4(s, 0, c, 1);
        }
        return v;
    }

    public static void DrawSphere(Vector4 pos, float radius, Color color, float timeHealth)
    {
        Vector4[] v = s_UnitSphere;
        int len = s_UnitSphere.Length / 3;
        for (int i = 0; i < len; i++)
        {
            var sX = pos + radius * v[0 * len + i];
            var eX = pos + radius * v[0 * len + (i + 1) % len];
            var sY = pos + radius * v[1 * len + i];
            var eY = pos + radius * v[1 * len + (i + 1) % len];
            var sZ = pos + radius * v[2 * len + i];
            var eZ = pos + radius * v[2 * len + (i + 1) % len];
            Debug.DrawLine(sX, eX, color, timeHealth);
            Debug.DrawLine(sY, eY, color, timeHealth);
            Debug.DrawLine(sZ, eZ, color, timeHealth);
        }
    }



}
