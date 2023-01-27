using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSItemHolderOffsetModifier : MonoBehaviour
{

    private PlayerInventoryHandler _playerInventory;


    private void Awake()
    {
        _playerInventory = GetComponentInParent<PlayerInventoryHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInventory == null) return;
        if (_playerInventory.currentSelectedPlayerItem == null) return;

        transform.localPosition = _playerInventory.currentSelectedPlayerItem.weaponOffset;
    }
}
