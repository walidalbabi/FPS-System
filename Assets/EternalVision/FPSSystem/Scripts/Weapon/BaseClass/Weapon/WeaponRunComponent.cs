using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRunComponent : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _offsetPos;
    [SerializeField] private Vector3 _offsetRot;
    private NetworkMovementHandler _playerMovements;


    public bool _isRunning;

    Vector3 targetRot;

    void LateUpdate()
    {
        if (_playerMovements == null) return;

        if (_playerMovements.localPlayerDataAction.isRunning)
        {
            targetRot = Vector3.Slerp(targetRot, _offsetRot, Time.deltaTime * _speed);
            transform.localPosition = Vector3.Lerp(transform.localPosition, _offsetPos, Time.deltaTime * _speed);
            transform.localRotation = Quaternion.Euler(targetRot);
        }
        else
        {
            targetRot = Vector3.Lerp(targetRot, Vector3.zero, Time.deltaTime * _speed);
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * _speed);
            transform.localRotation = Quaternion.Euler(targetRot);
        }
    }

    private void Update()
    {
        if (targetRot.y > 5 || targetRot.y < -5) _isRunning = true;
        else _isRunning = false;
    }

    public void SetPlayermovementsComponent(PlayerMovements component)
    {
        _playerMovements = (NetworkMovementHandler)component;
    }
}
