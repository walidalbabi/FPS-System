using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponSlot : SlotClass
{

    public override void Update()
    {
        base.Update();
    }
    public override void SetSlot(int slotNumber, Sprite slotImage)
    {
        _txtSlotNumber.text = slotNumber.ToString();
        _slotImage.sprite = slotImage;
    }
}
