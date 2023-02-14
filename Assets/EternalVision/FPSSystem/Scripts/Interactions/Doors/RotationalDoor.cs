using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotationalDoor : DoorClass
{
    [SerializeField] private float _rotationAmount = 90f;
    [SerializeField] protected DoorForward _doorForward;


    protected Vector3 _startRotation;


    public override void Awake()
    {
        base.Awake();
        _startRotation = transform.rotation.eulerAngles;
        _forward = SetForwardAxe();
    }

    private  Vector3 SetForwardAxe()
    {
        switch (_doorForward)
        {
            case DoorForward.X:
                return transform.right;
            case DoorForward.Y:
                return transform.up;
            case DoorForward.Z:
                return transform.forward;
            case DoorForward.negativeX:
                return transform.right * -1f;
            case DoorForward.negativeY:
                return transform.up * -1f;
            case DoorForward.negativeZ:
                return transform.forward * -1f;
            default:
                return transform.forward;
        }
    }

    public override void DoorInteract(Vector3 userPos)
    {
        if (!base.IsServer) return;

        ObserversInteract(userPos);
        if (!_isOpen) OpenDoor(userPos);
        else if (_isOpen) CloseDoor();
    }

    [ObserversRpc]
    private void ObserversInteract(Vector3 userPos)
    {
        if (base.IsOwner || base.IsServer) return;

        if (!_isOpen) OpenDoor(userPos);
        else if (_isOpen) CloseDoor();
    }

    public void OpenDoor(Vector3 userPos)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        float dot = Vector3.Dot(_forward, (userPos - transform.position).normalized);
        animationCoroutine = StartCoroutine(DoRotationOpen(dot));
    }

    private IEnumerator DoRotationOpen(float forwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if(forwardAmount >= forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0f, startRotation.y - _rotationAmount, 0f));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0f, startRotation.y + _rotationAmount, 0f));
        }

        _isOpen = true;

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * _speed;
        }
    }


    public void CloseDoor()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(DoRotationClose());
    }


    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(_startRotation);

        _isOpen = false;

        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * _speed;
        }
    }
}
