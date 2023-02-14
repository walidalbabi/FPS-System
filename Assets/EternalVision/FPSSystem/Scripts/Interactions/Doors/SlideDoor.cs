using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDoor : DoorClass
{

    [SerializeField] private Vector3 _slideDirection;
    [SerializeField] private float _slideAmount = 1.9f;

    private Vector3 _startPos;


    public override void Awake()
    {
        base.Awake();
        _startPos = transform.position;
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
        animationCoroutine = StartCoroutine(DoSlidingOpen());
    }

    private IEnumerator DoSlidingOpen()
    {
        Vector3 endPos = _startPos + _slideAmount * _slideDirection;
        Vector3 startPos = transform.position;


        float time = 0;
        _isOpen = true;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, time);
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

        animationCoroutine = StartCoroutine(DoSlidingClose());
    }


    private IEnumerator DoSlidingClose()
    {
        Vector3 endPos = _startPos;
        Vector3 startPos = transform.position;


        float time = 0;
        _isOpen = false;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, time);
            yield return null;
            time += Time.deltaTime * _speed;
        }
    }

}
