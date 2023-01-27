using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Serializing.Helping;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMovementHandler : PlayerMovements
{

    private float _nextSpectatorPitchTime = 0.03f;
    private bool _groundedChanged;


    [HideInInspector] [SyncVar(OnChange = nameof(ToogleSpeed))]
    private bool _isWalkStealth;
    [HideInInspector][SyncVar(OnChange = nameof(ToogleSpeed))]
    private bool _isRunning;
    [HideInInspector] [SyncVar(OnChange = nameof(ToogleSpeed))]
    private bool _isCrouch;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _maxAcceleration = _walkSpeed;
    }


    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (base.TimeManager != null)
            base.TimeManager.OnTick += TimeManager_OnTick;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        if (base.TimeManager != null)
            base.TimeManager.OnTick -= TimeManager_OnTick;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void Update()
    {
        base.Update();

        if (_localPlayerActionData.Ownership.isOwner)
        {
            UpdateRotation(_networkInputs.viewRotation, Time.deltaTime);
            CheckSendSpectatorPitch();
        }
    }


    private void TimeManager_OnTick()
    {
        if (_localPlayerActionData.Ownership.isOwner)
        {
            Reconcile(default, false);
            BuildActions();
            Replicate(_networkInputs, false);
        }
        else if (_localPlayerActionData.Ownership.isServer || GameManager.instance.networkContext.Ownership.isServer)
        {
            float rotY = transform.eulerAngles.y;

            Replicate(default, true);
            ReconcileData rd = new ReconcileData()
            {
                position = transform.position,
                verticalVelocity = _currentVerticalVelocity
            };
            Reconcile(rd, true);
        }
    }

    /// <summary>
    /// Setting the actions that the server and client will run
    /// </summary>
    [Client(Logging = LoggingType.Off)]
    private void BuildActions()
    {

        _networkInputs = default;
        _networkInputs = _inputHandler.GetPlayerLocalInputs();
        if (!CanPressSprint())
        {
            _networkInputs.sprintHold = false;
        }
        _inputHandler.NeededResetCashedInputsJump();


        float rotY = transform.eulerAngles.y;

        _networkInputs.Rotation = rotY;

        //Update Animator 
        _fullBodyAnimatorHandler.SetMovementsValues(_inputHandler.GetPlayerLocalInputs().moveInputs, _characterController.velocity.magnitude);
    }


    [Replicate]
    private void Replicate(InputData data, bool asServer, bool replaying = false)
    {
        float delta = (float)base.TimeManager.TickDelta;
        bool defaultData = Comparers.IsDefault(data);

        isLockController = _fullBodyAnimatorHandler.CheckForLockedMovementBool();

        //Update grounded, and set if grounded changed.
        SetIsGrounded(replaying, out _groundedChanged);
        CheckIfWeAreFalling(replaying);

   

        ApplyGravity(ref _currentVerticalVelocity, delta);
        DampenExternalForces(delta);
        //When grounded use a different velocity. This gives a better feel to the motor.
        SetGroundedVelocity(delta, asServer, replaying);
        SetStepOffset();

        Move(data.moveInputs, delta, _isRunning);
        Jump(data.jump, _jumpImpuls, replaying);


        if (!defaultData && (asServer || !replaying))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, data.Rotation, transform.eulerAngles.z);

        if(_isRunning != data.sprintHold) _isRunning = data.sprintHold;
        if(_isCrouch != data.sprintHold) _isCrouch = data.sprintHold;
        if(_isWalkStealth != data.stealthWalk) _isWalkStealth = data.stealthWalk;

        //Local Value
        _localPlayerActionData.isRunning = _isRunning;
        _localPlayerActionData.isCrouch = _isCrouch;
        _localPlayerActionData.isStealthWalk = _isWalkStealth;

        //Set Animation For the Current Equiped Item if it uses Animations
        if (_itemMotionAnimation != null) _itemMotionAnimation.SetRun(_isRunning);


        if (asServer)
        {
            //Update Animator Over Network
            ServerUpdateMovementsAnimations(data);
        }
          
    }

    [Reconcile]
    private void Reconcile(ReconcileData recData, bool asServer)
    {
        //Reset the client to the received position. It's okay to do this
        //even if there is no de-synchronization.
        transform.position = recData.position;
        _currentVerticalVelocity = recData.verticalVelocity;
    }


    public override void Jump(bool isJump, float jumpForce, bool isReplaying)
    {
        base.Jump(isJump, jumpForce, isReplaying);
    }

    public override void CheckIfWeAreFalling(bool replaying)
    {
        base.CheckIfWeAreFalling(replaying);
    }


    public override void OnPlayerJump()
    {
        if (_itemMotionAnimation != null) _itemMotionAnimation.SetJump();        //Set Animation For the Current Equiped Item if it uses Animations
        ObserverOnPlayerJump();
    }

    [ObserversRpc]
    private void ObserverOnPlayerJump()
    {
        _fullBodyAnimatorHandler.SetJump();
        PlayJumpSound();
    }

    public override void ToogleSpeed(bool oldValue, bool newValue, bool asServer)
    {
        if (_isRunning && CanPressSprint())
        {
            _maxAcceleration = _runSpeed;
        }
        else if (_isWalkStealth)
        {
            _maxAcceleration = _walkStealthSpeed;
        }
        else if (_isCrouch)
        {
            _maxAcceleration = _crouchSpeed;
        }
        else
        {
            _maxAcceleration = _walkSpeed;
        }

        _fullBodyAnimatorHandler.SetRun(_isRunning);
        _fullBodyAnimatorHandler.SetStealthWalk(_isWalkStealth);
    }

    /// <summary>
    /// Checks if owner should send pitch to the server.
    /// </summary>
    [Client(Logging = LoggingType.Off)]
    private void CheckSendSpectatorPitch()
    {
        if (Time.time < _nextSpectatorPitchTime)
            return;

        SendSpectatorPitch(lookDirection.x + pithWithRecoil);
    }

    [ServerRpc]
    private void SendSpectatorPitch(float pitch, Channel c = Channel.Unreliable)
    {
        _fullBodyAnimatorHandler.SetPitch(pitch);
    }

    [Server(Logging = LoggingType.Off)]
    private void ServerUpdateMovementsAnimations(InputData data)
    {
        _fullBodyAnimatorHandler.SetMovementsValues(data.moveInputs, _characterController.velocity.magnitude);
    }

    public override void TakeFallDamage()
    {
        base.TakeFallDamage();
    }

    public override void PlayLandSound(bool isFallWithDamage)
    {
        PooledLandSound(isFallWithDamage);
        ObserversPlayLandSound(isFallWithDamage);
    }

    private void PooledLandSound(bool isFallWithDamage)
    {
        SurfaceIdentifier surface = CastForSurface(0.5f);
        if (surface == null) return;
        var audioObj = GameManager.instance.objPool.RetrievePoolAudio();
        if (audioObj != null)
        {
            audioObj.transform.position = transform.position;
            if (!isFallWithDamage)
                audioObj.SetAudioSettings(_landAudioSettings, surface.surfaceData.landSounds[UnityEngine.Random.Range(0, surface.surfaceData.landSounds.Length)]);
            else audioObj.SetAudioSettings(_landAudioSettings, surface.surfaceData.fallImpactsSounds[UnityEngine.Random.Range(0, surface.surfaceData.fallImpactsSounds.Length)]);
            audioObj.Play();
        }
    }


    [ObserversRpc]
    private void ObserversPlayLandSound(bool isFallWithDamage)
    {
        if (base.IsOwner) return;
        PooledLandSound(isFallWithDamage);
    }
}
