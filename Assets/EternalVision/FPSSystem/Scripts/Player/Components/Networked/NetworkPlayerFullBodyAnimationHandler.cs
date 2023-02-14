using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerFullBodyAnimationHandler : PlayerFullBodyAnimationHandler
{
  

    [HideInInspector][SyncVar(Channel = FishNet.Transporting.Channel.Unreliable , ReadPermissions = ReadPermission.ExcludeOwner, SendRate = 0.02f)]
    private Vector3 v_movements;
    [HideInInspector][SyncVar(Channel = FishNet.Transporting.Channel.Unreliable, ReadPermissions = ReadPermission.ExcludeOwner, SendRate = 0.02f)]
    private float v_velocity;
    [HideInInspector]
    [SyncVar(Channel = FishNet.Transporting.Channel.Unreliable, ReadPermissions = ReadPermission.ExcludeOwner, SendRate = 0.02f)]
    private float v_pitch;




    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
    }


    public override void SmoothAnimations()
    {
        //smoothedVar_movements = Vector3.Lerp(smoothedVar_movements, v_movements, _animationSmoothSpeed * Time.deltaTime);
        //smoothedVar_velocity = Mathf.Lerp(smoothedVar_velocity, v_velocity, _animationSmoothSpeed * Time.deltaTime);
        soothedVar_pitch = Mathf.Lerp(soothedVar_pitch, v_pitch, _animationSmoothSpeed * Time.deltaTime);

        smoothedVar_movements = v_movements;
        smoothedVar_velocity = v_velocity;
    }

    public void SetPitch(float pt)
    {
        v_pitch = pt;
    }

    public void SetMovementsValues(Vector2 movementsDir, float velocity)
    {
        v_movements = movementsDir;
        v_velocity = velocity;
    }

}
