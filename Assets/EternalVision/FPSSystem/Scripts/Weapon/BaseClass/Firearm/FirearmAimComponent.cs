using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FirearmAimComponent : FirearmComponent
{
	// PRIVATE MEMBERS
	[SerializeField] [Range(10, 100)] protected float _camFovOnAim = 40f;
	[SerializeField] protected float _aimSpeed = 5f;
	[Tooltip("The Delay Used to wait before Aiming")][SerializeField] protected float _maxDelay;
	[SerializeField] protected Transform _aimTarget = null;


	protected Camera _playerCamera;
	protected float _defaultFOV;
	protected float _delay;

	protected bool _isAiming { get; set; }


	public static event Action<WeaponBehaviour, bool> OnAimStateChange;

	public bool isAiming { get { return _isAiming; } }


    public virtual void Update()
	{
		if (_playerCamera == null) return;

		if (Time.time < _delay) return;

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

	public void ToggleISAiming(bool state)
	{

		if (state)
		{
			if (_isAiming != state)
				_delay = Time.time + _maxDelay;
		}

		_isAiming = state;
		OnAimStateChange?.Invoke(_currentWeapon, state);
	}

	public void SetPlayerCamera()
    {
        if (_playerCamera == null)
            _playerCamera = Camera.main;
	}

}


