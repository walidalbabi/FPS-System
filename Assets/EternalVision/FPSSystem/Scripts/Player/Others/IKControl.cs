using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using FishNet.Object;
using UnityEngine.Animations.Rigging;

public class IKControl : NetworkBehaviour
{

    [SerializeField] private Transform _aimTarget;
    [SerializeField] private Transform _head;
    [SerializeField] private Rig _playerRig;

    private Vector3 _screeCenterPoint;

    private LocalPlayerData _localPlayerData;

    private void Awake()
    {
        _localPlayerData = GetComponentInParent<LocalPlayerData>();

    }


    private void Start()
    {
        _screeCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void LateUpdate()
    {
        if (_localPlayerData != null)
            CheckForLocalActionsData();


        if (!base.IsOwner) return;

        Ray ray = Camera.main.ScreenPointToRay(_screeCenterPoint);
        _aimTarget.transform.position = ray.origin + ray.direction;
    }


    public void SetPlayerRigStrenght(float amount)
    {
        if (_playerRig.weight != amount)
            _playerRig.weight = amount;
    }


    private void CheckForLocalActionsData()
    {
        if (_localPlayerData.onLadder)
        {
            SetPlayerRigStrenght(0);
        }
        else
        {
            SetPlayerRigStrenght(1);
        }
    }
}