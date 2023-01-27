using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCrosshair : FirearmComponent
{

    [SerializeField] private RectTransform _crosshairTansform;
    [SerializeField] private float _snappiness;
    [SerializeField] private float _returnSpeed;
    [SerializeField] private Vector2 _amountPerShot;

    private Vector2 _currentScale;
    private Vector2 _targetScale;
    private Vector2 _defaultScale;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = _crosshairTansform.GetComponent<CanvasGroup>();

        _defaultScale = _crosshairTansform.sizeDelta;

        FirearmShootCompoment.OnFire += SetCrosshairTaget;
    }

    private void OnDestroy()
    {
        FirearmShootCompoment.OnFire -= SetCrosshairTaget;
    }

    private void LateUpdate()
    {
        _targetScale = Vector3.Lerp(_targetScale, _defaultScale, _returnSpeed * Time.deltaTime);
        _currentScale = Vector3.Slerp(_currentScale, _targetScale, _snappiness * Time.deltaTime);
        _crosshairTansform.sizeDelta = _currentScale;

        //Fade Cursor

        if (_currentWeapon == null) return;

        if (_currentWeapon.isAiming)
        {
            if(_canvasGroup.alpha != 0)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0f,Time.deltaTime * 10f);
            }
        }
        else
        {
            if (_canvasGroup.alpha != 1)
            {
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1f, Time.deltaTime * 0.6f);
            }
        }
    }

    public void SetCrosshairTaget(WeaponBehaviour weapon, NetworkOwnership ownership)
    {
        if (weapon == _currentWeapon)
            _targetScale += _amountPerShot;
    }
}
