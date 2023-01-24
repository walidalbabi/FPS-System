using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmAimToCamera : FirearmAimComponent
{


    /// <summary>
    /// ///Unfinished Code
    /// </summary>

    [SerializeField] private float preferredDistance = 2f;

    private Vector3 _defaultLocalPosAim;
    private Vector3 _defaultLocalPosWeapon;

    private void Start()
    {
        _defaultLocalPosAim = _aimTarget.transform.localPosition;
        _defaultLocalPosWeapon = transform.localPosition;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Aim()
    {
        //var targetRot = Quaternion.identity;
        var targetPos = _defaultLocalPosAim;

      //  targetRot = _isAiming == true ? _aimTarget.rotation : targetRot;
        targetPos = _isAiming == true ? _playerCamera.transform.position : targetPos;

        if (!_isAiming)
        {
            if (_aimTarget.localPosition != targetPos)
                _aimTarget.localPosition = Vector3.Lerp(_aimTarget.localPosition, targetPos, Time.deltaTime * _aimSpeed);

         //   _playerCamera.transform.localRotation = Quaternion.Lerp(_playerCamera.transform.localRotation, targetRot, Time.deltaTime * _aimSpeed);
        }
        else
        {
            if (_aimTarget.position != targetPos)
                 _aimTarget.position = Vector3.Lerp(_aimTarget.position, targetPos, Time.deltaTime * _aimSpeed);

         //   if (_playerCamera.transform.rotation != targetRot)
         //       _playerCamera.transform.rotation = Quaternion.Lerp(_playerCamera.transform.rotation, targetRot, Time.deltaTime * _aimSpeed);
         //   else _playerCamera.transform.rotation = targetRot;
        }

        MoveWeaponToTargetAim();
    }

    void MoveWeaponToTargetAim()
    {
        Vector3 distanceVector = transform.position - _aimTarget.position;
        Vector3 distanceVectorNormalized = distanceVector.normalized;
        Vector3 targetPosition = (distanceVectorNormalized * preferredDistance);
        transform.position = targetPosition;
    }

}
