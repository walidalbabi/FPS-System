using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSwayMotion : MonoBehaviour
{
    [SerializeField] private bool _enableMotion;
    [SerializeField] private float _smooth = 8f;
    [SerializeField] private float _swayMultiplierX = 2f;
    [SerializeField] private float _swayMultiplierY = .5f;

    public void SetWeaponSway()
    {
        var sway = GetComponentInParent<PlayerItemsMotion>();
        if (_enableMotion)
        {
            if (sway != null)
                sway.SetSwaySettings(_smooth, _swayMultiplierX, _swayMultiplierY);
        }
        else
        {
            if (sway != null)
                sway.SetSwaySettings(0f, 0f, 0f);
        }
    }


}
