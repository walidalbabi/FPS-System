using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class SwipeableItemClass : MonoBehaviour
{
    public string _itemName;
    public Sprite _itemImage;

    public abstract void OnEquip();

    public abstract void OnUnEquip();
}


