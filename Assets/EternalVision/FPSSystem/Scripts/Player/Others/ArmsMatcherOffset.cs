using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ArmsMatcherOffset : NetworkBehaviour
{
    public ArmsOffset _armsOffset;

    [SerializeField] private float _smmothSpeed = 0.1f;
    [SerializeField] private MultiAimConstraint _aimContraintsOffset;


    private float _angle;
    [SyncVar] [HideInInspector] private Vector3 _offsetPos;
    [SyncVar] [HideInInspector] private Vector3 _offsetConstraints;
    [SyncVar] private Vector3 _lookDirection;


    private PlayerMovements _playerMovements;
    private LocalPlayerData _localPlayerData;
    private PlayerInventoryHandler _playerInventory;


    private void Awake()
    {
        _playerMovements = GetComponentInParent<PlayerMovements>();
        _localPlayerData = GetComponentInParent<LocalPlayerData>();
        _playerInventory = GetComponentInParent<PlayerInventoryHandler>();

        if (_playerInventory != null)
        {
            _playerInventory.OnSelectNewItem += OnSelectedItemChange;
        }
        else Debug.LogError("Player inventory reference is null");
    }

    private void OnDestroy()
    {
        if (_playerInventory != null)
        {
            _playerInventory.OnSelectNewItem -= OnSelectedItemChange;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!base.IsOwner) return;

        _angle = _playerMovements.lookDirection.x;
        _angle /= 90f * -1f;
        _lookDirection = _playerMovements.lookDirection;

        CalculateOffsetPosition(_angle);
        CalculateOffsetConstraints(_angle);

        if (base.TimeManager.Tick % 3 == 0)
        {
            ServerSyncOffsets(_offsetPos , _offsetConstraints, _lookDirection);
        }
    }

    private void LateUpdate()
    {
        SmoothCalculations();
    }

    private void SmoothCalculations()
    {
        Vector3 targetPos = _armsOffset.startLocalPos + _offsetPos;
        Vector3 targetOffsetConstraints = _armsOffset.startConstrainstValue + _offsetConstraints;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, _smmothSpeed);
        _aimContraintsOffset.data.offset = Vector3.Lerp(_aimContraintsOffset.data.offset, targetOffsetConstraints, _smmothSpeed);
        _lookDirection.x = Mathf.Clamp(_lookDirection.x, -89f, 89f); // clamping the values to avoid the angle rotation bug of the fps arms
        transform.rotation = Quaternion.Euler(_lookDirection);
    }


    private void CalculateOffsetPosition(float angle)
    {
        if (angle > 0)
        {
            _offsetPos = _armsOffset.upOffset * angle;
        }
        else if (angle < 0)
        {
            _offsetPos = _armsOffset.downOffset * angle;
        }
        else
        {
            _offsetPos = Vector3.zero;
        }

        if (_localPlayerData.isCrouch)
        {
            _offsetPos += _armsOffset.crouchOffset;
        }

        if (_localPlayerData.isRunning)
        {
            _offsetPos += _armsOffset.runOffset;
        }
    }

    private void CalculateOffsetConstraints(float angle)
    {
        if (angle > 0)
        {
            _offsetConstraints = _armsOffset.upOffsetConstraints * angle;
        }
        else if (angle < 0)
        {
            _offsetConstraints = _armsOffset.downOffsetConstraints * angle;
        }
        else
        {
            _offsetConstraints = Vector3.zero;
        }

        if (_localPlayerData.isCrouch)
        {
            _offsetConstraints += _armsOffset.crouchOffsetConstraints;
        }

        if (_localPlayerData.isRunning)
        {
            _offsetConstraints += _armsOffset.runOffsetConstraints;
        }
    }


    [ServerRpc]
    private void ServerSyncOffsets(Vector3 offsetPos , Vector3 offsetConstraints, Vector3 lookDir)
    {
        if (!base.IsServer) return;

        _offsetConstraints = offsetConstraints;
        _offsetPos = offsetPos;
        _lookDirection = lookDir;
        ObserverSyncOffsets(offsetPos ,offsetConstraints, lookDir);
    }

    [ObserversRpc]
    private void ObserverSyncOffsets(Vector3 offsetPos ,Vector3 offsetConstraints, Vector3 lookDir)
    {
        if (base.IsOwner || base.IsServer) return;

        _offsetPos = offsetPos;
        _offsetConstraints = offsetConstraints;
        _lookDirection = lookDir;
    }

    private void OnSelectedItemChange(int index, SwipeableItemClass item)
    {
        PlayerItem playerItem = item.GetComponent<PlayerItem>();

        if(playerItem != null)
        {
            _armsOffset = playerItem.armsOffset;
        }
    }
}


[System.Serializable]
public struct ArmsOffset
{
    [Header("Offset Pos")]
    public Vector3 startLocalPos;
    public Vector3 upOffset;
    public Vector3 downOffset;
    public Vector3 crouchOffset;
    public Vector3 runOffset;
    [Header("Offset IK Constraints")]
    public Vector3 startConstrainstValue;
    public Vector3 upOffsetConstraints;
    public Vector3 downOffsetConstraints;
    public Vector3 crouchOffsetConstraints;
    public Vector3 runOffsetConstraints;
}