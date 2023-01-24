using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryHandler : MonoBehaviour
{

    [SerializeField] private SlotClass _weaponSlotPefab;
    [SerializeField] private SlotClass _toolsSlotPrefab;
    [SerializeField] private SlotClass _othersSlotPrefab;

    [SerializeField] private Transform _weaponSlotTransform;
    [SerializeField] private Transform _toolsSlotTransform;
    [SerializeField] private Transform _othersSlotTransform;

    private PlayerInventoryHandler _playerInventory;
    private List<SlotClass> _allSlots = new List<SlotClass>();
    private List<SlotClass> _weaponsSlots = new List<SlotClass>();
    private List<SlotClass> _toolsSlots = new List<SlotClass>();
    private List<SlotClass> _othersSlots = new List<SlotClass>();

    [SerializeField]private SlotClass _currentSelectedSlot;

    private void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += Init;
    }

    private void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= Init;

        if (_playerInventory != null)
        {
            _playerInventory.OnNewWeapon -= SetNewWeaponSlot;
            _playerInventory.OnNewItemTool -= SetNewItemSlot;
            _playerInventory.OnSelectNewItem -= SelectSlot;
        }
   
    }


    public void Init(GameObject playerObj)
    {
        _playerInventory = playerObj.GetComponent<PlayerInventoryHandler>();

        if (_playerInventory != null)
        {
            _playerInventory.OnNewWeapon += SetNewWeaponSlot;
            _playerInventory.OnNewItemTool += SetNewItemSlot;
            _playerInventory.OnSelectNewItem += SelectSlot;

            _playerInventory.SetupLoadout();
        }
   
    }


    private void SetNewWeaponSlot(int slotNumb, SwipeableItemClass item)
    {
        var weapon = Instantiate(_weaponSlotPefab, _weaponSlotTransform);
        weapon.SetSlot(slotNumb + 1, item._itemImage);
        _weaponsSlots.Add(weapon);
        _allSlots.Add(weapon);
    }

    private void SetNewItemSlot(int slotNumb, SwipeableItemClass item)
    {
        var tool = Instantiate(_toolsSlotPrefab, _toolsSlotTransform);
        tool.SetSlot(slotNumb + 1, item._itemImage);
        _toolsSlots.Add(tool);
        _allSlots.Add(tool);
    }

    public void SelectSlot(int index, SwipeableItemClass item)
    {
        if (_currentSelectedSlot != null) _currentSelectedSlot.OnDeselect();
        _currentSelectedSlot = _allSlots[index];
        _currentSelectedSlot.OnSelected();
    }
}
