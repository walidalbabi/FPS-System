using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemBobbingMotion : MonoBehaviour
{
    [SerializeField] private bool _enableMotion;
    [SerializeField] private Vector3 _travelLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 _bobLimit = Vector3.one * 0.01f;
    [SerializeField] private Vector3 _bobMultiplier;

    public void SetBobSettings()
    {
        var sway = GetComponentInParent<PlayerItemsMotion>();
        if (_enableMotion)
        {
            if (sway != null)
                sway.SetBobSettings(_travelLimit, _bobLimit, _bobMultiplier);
        }
        else
        {
            if (sway != null)
                sway.SetBobSettings(Vector3.zero, Vector3.zero, Vector3.zero);
        }
         
    }
}
