using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemsMotion : MonoBehaviour
{
    //Sway Settings
    private float _smooth = 8f;
    private float _swayMultiplierX = 2f;
    private float _swayMultiplierY = .5f;
    private Quaternion swayRot;

    //Bobbing
    private float speedCurve;
    private float curveSin { get => Mathf.Sign(speedCurve); }
    private float curveCos { get => Mathf.Cos(speedCurve); }

    private Vector3 _travelLimit = Vector3.one * 0.025f;
    private Vector3 _bobLimit = Vector3.one * 0.01f;
    private Vector3 _bobMultiplier;
    private Vector3 _bobPostion;
    private Vector3 _bobEulerRotation;

    private InputData _inputs;

    //Components
    private InputHandler _inputHandler;
    private PlayerInventoryHandler _playerInventory;
    private PlayerMovements _playerMovements;
    private LocalPlayerData _localPlayerData;

    private void Awake()
    {
        _inputHandler = GetComponentInParent<InputHandler>();
        _playerInventory = GetComponentInParent<PlayerInventoryHandler>();
        _playerMovements = GetComponentInParent<PlayerMovements>();
        _localPlayerData = GetComponentInParent<LocalPlayerData>();
    }



    public void LateUpdate()
    {
        if (_playerInventory == null) return;

        _inputs = _inputHandler.GetPlayerLocalInputs();

        //Getting Local Inputs
        float swayX = _inputHandler.viewInputs.x * _swayMultiplierX;
        float swayY = _inputHandler.viewInputs.y * _swayMultiplierY;

        //Calculating Rotation
        Quaternion rotX = Quaternion.AngleAxis(-swayY, Vector3.right);
        Quaternion rotY = Quaternion.AngleAxis(swayX, Vector3.up);

        swayRot = rotX * rotY;

    }

    private void Update()
    {
        if (_playerInventory == null) return;

        BobOffset();
        BobRotation();


        //Applying Sway
        if (_playerInventory.currentSelectedPlayerItem == null) return;

        _playerInventory.currentSelectedPlayerItem.transform.localPosition = Vector3.Lerp(_playerInventory.currentSelectedPlayerItem.transform.localPosition, _bobPostion, Time.deltaTime * _smooth);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, swayRot * Quaternion.Euler(_bobEulerRotation), _smooth * Time.deltaTime);
    }

    private void BobOffset()
    {
        speedCurve += Time.deltaTime * (_playerMovements.isGrounded ? _playerMovements.currentMoveSpeed : 1f) + 0.01f;

      _bobPostion.x = (curveCos * _bobLimit.x * ((_playerMovements.isGrounded ? 1: 0) * (_localPlayerData.isAiming ? 0 : 1) * (_localPlayerData.isRunning ? 2 : 1))) - (_inputs.moveInputs.x * _travelLimit.x);
      _bobPostion.y = (curveSin * _bobLimit.y) - (_playerMovements.velocityY * _travelLimit.x);
      _bobPostion.z = (_inputs.moveInputs.y * _travelLimit.z);
    }

    private void BobRotation()
    {
        _bobEulerRotation.x = (_inputs.moveInputs != Vector2.zero ? _bobMultiplier.x * (Mathf.Sin(2 * speedCurve)) :
            _bobMultiplier.x * (Mathf.Sin(2 * speedCurve) / 2f));

        _bobEulerRotation.y = (_inputs.moveInputs != Vector2.zero ? _bobMultiplier.y * curveCos : 0f);
        _bobEulerRotation.z = (_inputs.moveInputs != Vector2.zero ? _bobMultiplier.z * curveCos * _inputs.moveInputs.x : 0f);
    }

    /// <summary>
    /// Setting Sway Settings from each weapon On Equiping
    /// </summary>
    /// <param name="smooth"></param>
    /// <param name="swayXMultiplier"></param>
    /// <param name="swayYMultiplier"></param>
    public void SetSwaySettings(float smooth, float swayXMultiplier, float swayYMultiplier)
    {
        this._smooth = smooth;
        this._swayMultiplierX = swayXMultiplier;
        this._swayMultiplierY = swayYMultiplier;
    }

    public void SetBobSettings(Vector3 travelLimit, Vector3 bobLimit, Vector3 bobMultiplier)
    {
        this._travelLimit = travelLimit;
        this._bobLimit = bobLimit;
        this._bobMultiplier = bobMultiplier;
    }

}
