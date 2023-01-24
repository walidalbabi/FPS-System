using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FirearmAimComponent : FirearmComponent
{
	// PRIVATE MEMBERS
	[SerializeField] [Range(10, 100)] protected float _camFovOnAim;
	[SerializeField] protected float _aimSpeed = .7f;
	[SerializeField] protected Transform _aimTarget;


	protected Camera _playerCamera;
	protected float _defaultFOV;

	protected bool _isAiming { get; set; }


	public bool isAiming { get { return _isAiming; } }


    public virtual void Update()
	{
		if (_playerCamera == null) return;

		AimFOVHandler();
		Aim();
	}

	public abstract void Aim();


	public virtual void AimFOVHandler()
	{
		_defaultFOV = 60f;

		if (_isAiming)
		{
			if (_playerCamera.fieldOfView != _camFovOnAim)
				_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _camFovOnAim, Time.deltaTime * _aimSpeed * 2f);
		}
		else
		{
			if (_playerCamera.fieldOfView != _defaultFOV)
				_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _defaultFOV, Time.deltaTime * _aimSpeed * 2f);
		}
	}

	public void ForceUnAim()
	{
		_isAiming = false;
	}

	public void ToggleISAiming(bool state)
	{
		_isAiming = state;
	}

	public void SetPlayerCamera()
    {
        if (_playerCamera == null)
            _playerCamera = Camera.main;
	}

}


