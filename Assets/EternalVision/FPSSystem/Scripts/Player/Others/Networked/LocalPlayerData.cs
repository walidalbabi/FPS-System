using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerData : NetworkBehaviour
{

    [SyncVar]
    [SerializeField] private string _playerName;
    public string playerName { get { return _playerName; } }

    public event Action OnLocalPlayerDataAvailble;

    public bool isRunning;
    public bool isCrouch;
    public bool isStealthWalk;
    public bool isFiring;
    public bool isMeleeAttack;
    public bool isReloading;
    public bool isSwitchingItem;
    public bool isDead;
    public bool isAiming;
    public bool onLadder;

    /// <summary>
    /// Act only for this current local client, use GameManager instead for other viewrs
    /// </summary>
    public NetworkOwnership Ownership;


    private CharacterController _characterContoller;
    private PlayerHealth _playerHealth;
    private PlayerInventoryHandler _playerInventory;

    private void Awake()
    {
        _characterContoller = GetComponent<CharacterController>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerInventory = GetComponent<PlayerInventoryHandler>();

        FirearmAmmoComponent.OnReloadStateChange += SetReloadState;
        _playerHealth.OnDeath += OnPlayerDie;
        _playerHealth.OnRespawned += OnPlayerRespawned;
    }

    private void OnDestroy()
    {
        FirearmAmmoComponent.OnReloadStateChange += SetReloadState;
        _playerHealth.OnDeath -= OnPlayerDie;
        _playerHealth.OnRespawned -= OnPlayerRespawned;
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        Ownership.CreateNew(base.IsOwner, base.IsClient, base.IsServer, base.IsHost);
        OnLocalPlayerDataAvailble?.Invoke();
        if (base.IsOwner)
        {
            SetUpPlayer();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void SetUpPlayer()
    {
        if (base.IsOwner)
        {
            SetName(MenuPlayerData.instance.GetPlayerName());
            ServerSetName(MenuPlayerData.instance.GetPlayerName());
        }
    }

    private void SetName(string name)
    {
        _playerName = name;
        MenuPlayerData.instance.gameObject.SetActive(false);
    }

    [ServerRpc]
    private void ServerSetName(string name)
    {
        if (!base.IsServer) return;
        SetName(name);
        ObserversSetName(name);
    }

    [ObserversRpc]
    private void ObserversSetName(string name)
    {
        if (base.IsOwner || base.IsServer) return;

        SetName(name);
    }

    private void SetReloadState(WeaponBehaviour weapon, bool state)
    {
        if (_playerInventory.currentSelectedPlayerItem == weapon)
            isReloading = state;
    }

    private void OnPlayerDie()
    {
        _characterContoller.enabled = false;
        isDead = true;
    }

    private void OnPlayerRespawned()
    {
        _characterContoller.enabled = true;
        isDead = false;
    }
}
