using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerInventoryHandler : PlayerInventoryHandler
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            InstantiateItems();
        }
    }

    public override void SetupLoadout()
    {
        ServerSetLoadout();
    }

    public override void Init()
    {
        base.Init();
        ServerInit();
    }

    public override void ChangeItem(int index)
    {
        base.ChangeItem(index);
        ServerChangingWeapon(index);
    }

    [ServerRpc]
    private void ServerSetLoadout()
    {
        if (!base.IsServer) return;
        GetWeaponsInInventory();
        GetAllItemsInInventory();
        Init();
        ObserversSetLoadout();
    }

    [ObserversRpc]
    private void ObserversSetLoadout()
    {
        if (base.IsServer) return;
        GetWeaponsInInventory();
        GetAllItemsInInventory();
        Init();
    }

    [ServerRpc]
    private void InstantiateItems()
    {
        if (_defaultArm != null)
        {
            var obj = Instantiate(_defaultArm, _fpsItemHolder);
            base.Spawn(obj.gameObject, base.Owner);
            SetItemsParent(obj);
        }
        if (_playerLoadout._primaryWeapon != null)
        {
            var obj = Instantiate(_playerLoadout._primaryWeapon, _fpsItemHolder);
            base.Spawn(obj.gameObject, base.Owner);
            SetItemsParent(obj);
        }
        if (_playerLoadout._seconderyWeapon != null)
        {
            var obj = Instantiate(_playerLoadout._seconderyWeapon, _fpsItemHolder);
            base.Spawn(obj.gameObject, base.Owner);
            SetItemsParent(obj);
        }
    }

    [ObserversRpc]
    private void SetItemsParent(PlayerItem item)
    {
        if (base.IsServer) return;
        item.transform.parent = _fpsItemHolder;
        item.transform.localPosition = Vector3.zero;
    }

    [ServerRpc]
    private void ServerInit()
    {
        if (GameManager.instance.networkContext.Ownership.isServer && !_localPlayerData.Ownership.isServer)
        {
            Initialise();
        }
        ObserversInit();
    }

    [ObserversRpc]
    private void ObserversInit()
    {
        if (_localPlayerData.Ownership.isOwner || _localPlayerData.Ownership.isServer) return;

        Initialise();
    }

    [ServerRpc]
    private void ServerChangingWeapon(int index)
    {
        if (!base.IsServer) return;
        LocalChangingWeapon(index);
        ObserversChangingWeapon(index);
    }

    [ObserversRpc]
    private void ObserversChangingWeapon(int index)
    {
        if (base.IsOwner || base.IsServer) return;
        LocalChangingWeapon(index);
    }
}
