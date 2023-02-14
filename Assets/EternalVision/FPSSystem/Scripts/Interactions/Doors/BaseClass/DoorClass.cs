using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorClass : NetworkBehaviour, I_Interactable
{
    protected enum DoorForward
    {
        X,
        Y,
        Z,
        negativeX,
        negativeY,
        negativeZ
    }

    [SerializeField] protected float _speed = 2f;
    [SerializeField] protected float forwardDirection = 0f;


    protected bool _isOpen = false;
    protected Vector3 _forward;
    protected Coroutine animationCoroutine;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public virtual void Awake()
    {


    }




    public void Interact(GameObject playerObj)
    {
        DoorInteract(playerObj.transform.position);
    }

    public abstract void DoorInteract(Vector3 userPos);

}
