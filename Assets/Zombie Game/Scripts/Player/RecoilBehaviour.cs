using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RecoilBehaviour : MonoBehaviour
{

    [SerializeField] private Vector3 _recoil;
    [SerializeField] private Vector3 _recoilAim;
    [SerializeField] private float _snappines;
    [SerializeField] private float _returnSpeed;

    protected Vector3 currentRotation;
    protected Vector3 targetRotation;
    protected bool _isAiming;

    protected Vector3 recoil => _recoil;
    protected Vector3 recoilAim => _recoilAim;
    protected float snappines => _snappines;
    protected float returnSpeed => _returnSpeed;

    public abstract void OnUpdate();

    public abstract void OnFireRecoil(WeaponBehaviour fireArm ,NetworkOwnership networkOwnership);
}
