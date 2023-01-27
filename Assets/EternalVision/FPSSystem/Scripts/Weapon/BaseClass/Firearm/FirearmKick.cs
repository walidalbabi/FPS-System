using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmKick : FirearmComponent
{

    [SerializeField] private float _weaponKick;
    [SerializeField] private float _returnSpeed;
    [SerializeField] private float _snappines;


    private Vector3 _targetRotation;
    private Vector3 _currentRotation;



    private void Awake()
    {
        FirearmShootCompoment.OnFire += OnFire;
    }

    private void OnDestroy()
    {
        FirearmShootCompoment.OnFire -= OnFire;
    }

    private void Update()
    {
        //Updating the weapon rotation with the recoil settings
        OnUpdate();
    }

    public void OnUpdate()
    {
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, _snappines * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(_currentRotation);
    }

    /// <summary>
    /// On fire Update the rot of the gun by the recoil settings, usually called On Fire
    /// </summary>
    public void OnFire(WeaponBehaviour fireArm, NetworkOwnership networkOwnership)
    {
        if (fireArm == _currentWeapon)
            _targetRotation += new Vector3(_weaponKick, 0f, 0f);
    }
}
