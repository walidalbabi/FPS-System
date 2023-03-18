using FishNet;
using FishNet.Managing.Timing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerCameraHander : MainCameraHandler
{

    private TimeManager _timeManager;              

    public override void Awake()
    {
        base.Awake();
        _timeManager = InstanceFinder.TimeManager;

        if (_timeManager != null)
            _timeManager.OnUpdate += TimeManager_OnLateUpdate;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (_timeManager != null)
            _timeManager.OnUpdate -= TimeManager_OnLateUpdate;
    }


    private void Update()
    {
       
    }

    private void TimeManager_OnLateUpdate()
    {
        UpdatePositionAndRotation(Time.deltaTime);
    }

}
