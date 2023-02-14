using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInventoryHandler : NetworkBehaviour
{

    [Header("Refferences")]
    [Space]
    [SerializeField] protected Transform _playerCamAnchor;
    [SerializeField] protected Transform _fpsItemHolder;
    [SerializeField] protected Transform _weaponThirdPersonHolder;
    [Space]
    [Header("Loadout Settings")]
    [SerializeField] protected MeleeWeapon _defaultArm;
    [SerializeField] protected Loadout _playerLoadout;

    protected int _currentItemIndex;
    protected SwipeableItemClass _currentSwipeable;
    protected SwipeableItemClass _previousSwipeable;
    protected PlayerItem _currentSelectedPlayerItem;

    protected NetworkPlayerFullBodyAnimationHandler _playerFullBodyAnimatoprHandler;
    protected PlayerMovements _playerMovements;
    protected LocalPlayerData _localPlayerData;


    public List<SwipeableItemClass> _allSwipeableInInventory = new List<SwipeableItemClass>();
    public List<WeaponBehaviour> _weaponsInInventory = new List<WeaponBehaviour>();


    /// <summary>
    /// On Init a new weapon to our inventory with the index if it in the list and its info
    /// </summary>
    public event Action<int,SwipeableItemClass> OnNewWeapon;
    /// <summary>
    /// On Init a new Item Tool to our inventory with the index if it in the list and its info
    /// </summary>
    public event Action<int,SwipeableItemClass> OnNewItemTool;
    /// <summary>
    /// When Changing the current Selected ITem with its index in the ui and the item refference
    /// </summary>
    public event Action<int, SwipeableItemClass> OnSelectNewItem;

    /// <summary>
    /// When Changing the current Selected ITem and equiping the new item we call that
    /// </summary>
    public event Action<int, PlayerItem> OnNewITemEquipedAndReadyToUSeItem;

    public PlayerItem currentSelectedPlayerItem => _currentSelectedPlayerItem;
    public Transform playerCamAnchor { get { return _playerCamAnchor; } }
    public Transform weaponThirdPersonHolder { get { return _weaponThirdPersonHolder; } }

    private void Awake()
    {
        _playerFullBodyAnimatoprHandler = GetComponent<NetworkPlayerFullBodyAnimationHandler>();
        _playerMovements = GetComponent<PlayerMovements>();
        _localPlayerData = GetComponent<LocalPlayerData>();
    }


    public virtual void SetupLoadout() { Init(); }


    public virtual void Init()
    {
        Initialise();
    }

    public virtual void ChangeItem(int index)
    {
        LocalChangingWeapon(index);
    }


    protected void GetWeaponsInInventory()
    {
        foreach (var item in _fpsItemHolder.GetComponentsInChildren<WeaponBehaviour>())
        {
            var playerItem = item.GetComponent<PlayerItem>();

            item.transform.localPosition = Vector3.zero;
            item.OnUnEquip(true);
            _weaponsInInventory.Add(item);
            _allSwipeableInInventory.Add(item.GetComponent<SwipeableItemClass>());

            SetItemStateOnGameStart(playerItem);

            OnNewWeapon?.Invoke(_weaponsInInventory.IndexOf(item), item.GetComponent<SwipeableItemClass>());
        }
    }

    protected void GetAllItemsInInventory()
    {
        foreach (var item in _fpsItemHolder.GetComponentsInChildren<SwipeableItemClass>())
        {

            if (item.GetComponent<WeaponBehaviour>()) return;

            item.transform.localPosition = Vector3.zero;
            item.OnUnEquip(true);
            _allSwipeableInInventory.Add(item);

            var playerItem = item.GetComponent<PlayerItem>();
            SetItemStateOnGameStart(playerItem);

            OnNewItemTool?.Invoke(_allSwipeableInInventory.IndexOf(item), item.GetComponent<SwipeableItemClass>());
        }
    }


    private void SetItemStateOnGameStart(PlayerItem playerItem)
    {
        playerItem.Set_FullBodyAnimatorHandler(_playerFullBodyAnimatoprHandler);
        playerItem.Set_PlayerInventoryHandler(this);
    }

    protected void Initialise()
    {
        _currentSelectedPlayerItem = _weaponsInInventory[0];
        _currentSwipeable = _allSwipeableInInventory[0];
        _previousSwipeable = _allSwipeableInInventory[0];
        _currentSwipeable.gameObject.SetActive(true);
        _currentSwipeable.OnEquip();
        OnSelectNewItem?.Invoke(0, _currentSwipeable);
        SetItemNeeds();
    }


    protected void LocalChangingWeapon(int index)
    {
        var item = GetItemByIndex(index);
        //Getting next item
        if (item == null) return;
        //Found Item
        _currentSwipeable = item;
        _currentItemIndex = index;

        if (_previousSwipeable == _currentSwipeable) return;
        //Not The same Previouse Item
        if (_previousSwipeable != null)
        {
            _previousSwipeable.OnUnEquip(true);
            Debug.Log("OnUnEquip");
        }
        //Update UI
        OnSelectNewItem?.Invoke(index, _currentSwipeable);
        //Update Local Data
        _localPlayerData.isSwitchingItem = true;
    }


    /// <summary>
    /// This is Getting Called After previouse ITem Unequiped
    /// </summary>
    public void EnableNewItem()
    {
        _previousSwipeable.gameObject.SetActive(false);
        _previousSwipeable = _currentSwipeable;
        _previousSwipeable.gameObject.SetActive(true);
        _previousSwipeable.OnEquip();
        SetItemNeeds();

        if (_currentSwipeable.gameObject.GetComponent<PlayerItem>()) // Just a Check
            _currentSelectedPlayerItem = _currentSwipeable.gameObject.GetComponent<PlayerItem>();
        else _currentSelectedPlayerItem = null;

        OnNewITemEquipedAndReadyToUSeItem?.Invoke(_currentItemIndex, _currentSelectedPlayerItem);
        _localPlayerData.isSwitchingItem = false;
    }

    private void SetItemNeeds()
    {
        if (GameManager.instance.networkContext.Ownership.isOwner)
        {
                if (GetCurrentEquiped() != null)
                    GetCurrentEquiped().SetWeaponComponentsNeeds(_localPlayerData.Ownership, _playerMovements);

        }
        else Debug.Log("failed to set weapon aim");
    }

    public virtual void HostlerItem()
    {
        _currentSwipeable.OnUnEquip(false);
    }

    public virtual void UnhostlerItem()
    {
        _currentSwipeable.gameObject.SetActive(true);
        _currentSwipeable.OnEquip();
    }


    #region Getters

    public SwipeableItemClass GetItemByIndex(int Index)
    {
        var item = Index <= _allSwipeableInInventory.Count -1 && Index >= 0;

        if (item)
            return _allSwipeableInInventory[Index];
        else return null;
    }

    public WeaponBehaviour GetCurrentEquipedWeapon()
    {
        WeaponBehaviour weapon =_weaponsInInventory[_currentItemIndex];

        if (weapon != null)
            return weapon;
        else return null;
    }

    public PlayerItem GetCurrentEquiped()
    {
        PlayerItem item = _allSwipeableInInventory[_currentItemIndex].GetComponent<PlayerItem>();

        if (item != null)
            return item;
        else return null;
    }

    public int GetCurrentItemIndex()
    {
        return _currentItemIndex;
    }

    #endregion Getters

}

[System.Serializable]
public class Loadout
{
    public WeaponBehaviour _primaryWeapon;
    public WeaponBehaviour _seconderyWeapon;
}
