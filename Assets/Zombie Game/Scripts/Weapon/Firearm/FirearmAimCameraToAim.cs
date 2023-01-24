using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmAimCameraToAim : FirearmAimComponent
{
	/// <summary>
	/// For preventing aiming when the gun is running
	/// </summary>
	private WeaponRunComponent _fireArmRunTransform;

    private void Awake()
    {
		_fireArmRunTransform = GetComponentInChildren<WeaponRunComponent>();

	}

    public override void Update()
    {
        base.Update();
    }

    public override void Aim()
    {
		var targetRot = Quaternion.identity;
		var targetPos = Vector3.zero;

		targetRot = _isAiming == true ? _aimTarget.rotation : targetRot;
		targetPos = _isAiming == true ? _aimTarget.position : targetPos;

		if (!_isAiming)
		{
			if (_playerCamera.transform.position != targetPos)
				_playerCamera.transform.localPosition = Vector3.Lerp(_playerCamera.transform.localPosition, targetPos, Time.deltaTime * _aimSpeed);

			_playerCamera.transform.localRotation = Quaternion.Lerp(_playerCamera.transform.localRotation, targetRot, Time.deltaTime * _aimSpeed);
		}
		else
		{
			if (_fireArmRunTransform._isRunning) return;

			if (_playerCamera.transform.position != targetPos)
				_playerCamera.transform.position = Vector3.Lerp(_playerCamera.transform.position, targetPos, Time.deltaTime * _aimSpeed);

			if (_playerCamera.transform.rotation != targetRot)
				_playerCamera.transform.rotation = Quaternion.Lerp(_playerCamera.transform.rotation, targetRot, Time.deltaTime * _aimSpeed);
			else _playerCamera.transform.rotation = targetRot;
		}
	}

    public override void AimFOVHandler()
    {
		if (_fireArmRunTransform._isRunning) return;
		base.AimFOVHandler();
    }
}
