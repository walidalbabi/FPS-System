using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAgentController : PlayerController
{

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Event_CallOnPlayerAvailbleInScene();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }


    private void Update()
    {
        if (_localPlayerActionData.Ownership.isOwner)
        {
            GetInputs();
            CheckForChangingItem();
            if (_playerInventoryHandler.currentSelectedPlayerItem == null) return;


            CheckForLeftClick();
            CheckForRightClick();
            CheckForReload();
            
        }
    }
    /// <summary>
    /// Checks if the client wants to and can fire.
    /// </summary>
    [Client(Logging = LoggingType.Off)]
    public override void CheckForLeftClick()
    {
        base.CheckForLeftClick();
    }

    /// <summary>
    /// Checks if the client wants to and can Aim.
    /// </summary>
    [Client(Logging = LoggingType.Off)]
    public override void CheckForRightClick()
    {
        base.CheckForRightClick();
    }


    [Client(Logging = LoggingType.Off)]
    public override void CheckForReload()
    {
        base.CheckForReload();
    }

    [Client(Logging = LoggingType.Off)]
    public override void CheckForChangingItem()
    {
        base.CheckForChangingItem();
    }

}
