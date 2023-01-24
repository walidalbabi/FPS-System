using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputHandler : MonoBehaviour
{
    //Local Cashed Inputs
    private InputData _cashedInputs = new InputData();


    private Vector2 _viewInputs;


    public Vector2 viewInputs =>_viewInputs;

    private bool isCursorLocked;


    private void Awake()
    {

    }

    private void Start()
    {
        ToggleCursor(true);
    }

    public void On_ToggleCursor(InputAction.CallbackContext context)
    {

        switch (context)
        {
            case { phase: InputActionPhase.Started }:
                ToggleCursor(!isCursorLocked);
                break;
        }
    }

    public void On_Move(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        _cashedInputs.moveInputs = context.ReadValue<Vector2>();
    }

    public void On_Look(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        _viewInputs = context.ReadValue<Vector2>();
        _viewInputs.y *= -1f;
        // _localCameraHandler.SetCameraViewInput(_viewInputs);
        _cashedInputs.viewRotation = _viewInputs;
    }

    public void On_Jump(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        _cashedInputs.jump = context.performed;
    }

    public void On_Sprint(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        switch (context)
        {
            case { phase: InputActionPhase.Started }:
                _cashedInputs.sprintHold = true;
                break;
            case { phase: InputActionPhase.Canceled}:
                _cashedInputs.sprintHold = false;
                break;
        }
    }

    public void On_StealthWalk(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        switch (context)
        {
            case { phase: InputActionPhase.Started }:
                _cashedInputs.stealthWalk = true;
                break;
            case { phase: InputActionPhase.Canceled }:
                _cashedInputs.stealthWalk = false;
                break;
        }
    }

    public void On_Fire(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;


        switch (context)
        {
            case { phase: InputActionPhase.Started }:
                _cashedInputs.LeftClick = true;
                break;
            case { phase: InputActionPhase.Canceled }:
                _cashedInputs.LeftClick = false;
                break;
        }
    }

    public void On_Aim(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        switch (context)
        {
            case { phase: InputActionPhase.Started }:
                _cashedInputs.RightClick = true;
                break;
            case { phase: InputActionPhase.Canceled }:
                _cashedInputs.RightClick = false;
                break;
        }
    }

    public void On_Reload(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        switch (context)
        {
            case { phase: InputActionPhase.Started }:
                _cashedInputs.reload = true;
                break;
        }
    }


    public void On_ScrollWheel(InputAction.CallbackContext context)
    {
        if (!CanGetInputs()) return;

        _cashedInputs.itemScroll = context.ReadValue<Vector2>();
    }


    private void ToggleCursor(bool state)
    {
        isCursorLocked = state;
        Cursor.lockState = state == false ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = !state;
    }

    public InputData GetPlayerLocalInputs()
    {
        //Return Local Inputs
        return _cashedInputs;
    }

    public void NeededResetCashedInputsJump()
    {
        //Reset Inputs
        _cashedInputs.jump = false;
    }

    public void NeededResetCashedInputsReload()
    {
        //Reset Inputs
        _cashedInputs.reload = false;
    }

    public void NeededResetCashedInputsFire()
    {
        //Reset Inputs
        _cashedInputs.LeftClick = false;
    }

    private bool CanGetInputs()
    {
        if (!isCursorLocked) return false;

        return true;
    }
}
