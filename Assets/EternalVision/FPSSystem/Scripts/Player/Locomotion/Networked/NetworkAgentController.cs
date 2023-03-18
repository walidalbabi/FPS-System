using FishNet.Managing.Logging;
using FishNet.Object;
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
            CheckForInteractions();
            CheckForExitCurrentLadder();
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

    [Client(Logging = LoggingType.Off)]
    public override void CheckForInteractions()
    {
        if (!CanInteract()) return;

        if (_networkInputs.interact)
        {
            Debug.Log("interact pressed");
            Ray ray = Camera.main.ScreenPointToRay(_screenRay);
            ServerInteract(ray.origin, ray.direction);
            RaycastForInteraction(ray.origin, ray.direction);
        }
    }

    [Client(Logging = LoggingType.Off)]
    public override void CheckForExitCurrentLadder()
    {
        base.CheckForExitCurrentLadder();
    }

    [ServerRpc]
    private void ServerInteract(Vector3 pos, Vector3 dir)
    {
        if (base.IsServer)
        {
            RaycastForInteraction(pos, dir);
            ObserversInteract(pos, dir);
        }
    }

    [ObserversRpc]
    private void ObserversInteract(Vector3 pos, Vector3 dir)
    {
        if (base.IsOwner || base.IsServer) return;
        RaycastForInteraction(pos, dir);
    }

    private void RaycastForInteraction(Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray(origin,direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _interactionDistance, _interactionLayer))
        {
            if (hit.collider != null)
            {
                var interactive = hit.collider.GetComponent<I_Interactable>();
                if (interactive != null)
                {
                    Debug.Log("Found a Interactor" + base.IsServer + base.IsOwner);
                    //interact
                    interactive.Interact(this.gameObject);
                }

            }
        }
    }
}
