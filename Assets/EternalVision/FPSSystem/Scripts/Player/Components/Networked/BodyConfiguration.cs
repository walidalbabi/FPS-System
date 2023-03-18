using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyConfiguration : NetworkBehaviour
{
    [SerializeField] private string _remotePlayerLayer;
    [SerializeField] private string _localPlayerLayer;
    [SerializeField] private string _playerController;

    public Transform fpHand;
    public SkinnedMeshRenderer fullBodyMeshRender;
    public Transform fullBodyWeaponHandler;
    public Transform fullBodyArmsMatcher;


    private bool isOwner;
    private LocalPlayerData _localPlayerData;

    private void Awake()
    {
        _localPlayerData = GetComponent<LocalPlayerData>();

    }

    private void Start()
    {
        SetIsOwner();
    }

    public void SetIsOwner()
    {
        this.isOwner = base.IsOwner || base.IsHost;
        SetBodyConfigurations();
        if (base.IsOwner) ServerSetBodyConfigurations();
    }

    [ServerRpc]
    private void ServerSetBodyConfigurations()
    {
        if (!GameManager.instance.networkContext.Ownership.isServer) return;

        SetBodyConfigurations();
        ObserversSetBodyConfigurations();
    }

    [ObserversRpc]
    private void ObserversSetBodyConfigurations()
    {
        if (GameManager.instance.networkContext.Ownership.isServer || _localPlayerData.Ownership.isServer) return;

        SetBodyConfigurations();
    }


    private void SetBodyConfigurations()
    {
        if (isOwner)
        {
            //foreach (var child in fpHand.GetComponentsInChildren<Transform>())
            //{
            //    child.gameObject.layer = LayerMask.NameToLayer(_localPlayerLayer);
            //}
            fullBodyWeaponHandler.gameObject.SetActive(false);
            fullBodyMeshRender.gameObject.layer = LayerMask.NameToLayer(_remotePlayerLayer);
            gameObject.layer = LayerMask.NameToLayer(_playerController);
        }
        else
        {
            //foreach (var child in fpHand.GetComponentsInChildren<Transform>())
            //{
            //    child.gameObject.layer = LayerMask.NameToLayer(_remotePlayerLayer);
            //}
            fullBodyWeaponHandler.gameObject.SetActive(true);
            fullBodyMeshRender.gameObject.layer = 0;
            gameObject.layer = LayerMask.NameToLayer(_playerController);
        }
    }

}
