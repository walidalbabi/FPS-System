using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class SlotClass : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _txtSlotNumber;
    [SerializeField] protected Image _slotImage;
    [SerializeField] protected Image _slotBackgroundImage;
    [SerializeField, Range(0f ,1f)] protected float _maxFade = 1f;
    [SerializeField] protected float _fadeSpeed = 2f;



    [SerializeField]private bool _isSelected;

    public virtual void Update()
    {
        if (_isSelected)
        {
            if (_slotBackgroundImage.color.a != _maxFade)
            {
                var tmpColor = _slotBackgroundImage.color;
                tmpColor.a = Mathf.Lerp(_slotBackgroundImage.color.a, _maxFade, Time.deltaTime * _fadeSpeed);
                _slotBackgroundImage.color = tmpColor;
            }
        }
        else
        {
            if (_slotBackgroundImage.color.a != 0)
            {
                var tmpColor = _slotBackgroundImage.color;
                tmpColor.a = Mathf.Lerp(_slotBackgroundImage.color.a, 0f, Time.deltaTime * _fadeSpeed);
                _slotBackgroundImage.color = tmpColor;
            }

        }
    }

    public abstract void SetSlot(int slotNumber, Sprite slotImage);

    public void OnSelected()
    {
        _isSelected = true;
    }

    public void OnDeselect()
    {
        _isSelected = false;
    }
}
