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
        {
            base.TimeManager.OnTick += TimeManager_OnTick;
        }

    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        if (base.TimeManager != null)
        {
            base.TimeManager.OnTick -= TimeManager_OnTick;
        }

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
            Replicate(default, true);
            ReconcileData rd = new ReconcileData()
            {
                position = transform.position,
                moveSpeed = _currentMoveSpeed,
                verticalVelocity = _currentVerticalVelocity,
                stamina = _currentStamina,
                movementInputs = _movementInputs
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
       // _fullBodyAnimatorHandler.SetMovementsValues(_inputHandler.GetPlayerLocalInputs().moveInputs, _characterController.velocity.magnitude);
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



        ApplyGravity(ref _currentVerticalVelocity.y, delta);
        DampenExternalForces(delta);
        //When grounded use a different velocity. This gives a better feel to the motor.
        SetGroundedVelocity(delta, asServer, replaying);
        SetStepOffset();



        Move(data.moveInputs, delta, _isRunning);
        Jump(data.jump, _jumpImpuls, replaying);

        CalculateStamina((float)base.TimeManager.TickDelta);

        if (!defaultData && (asServer || !replaying))
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, data.Rotation, transform.eulerAngles.z);

        if (_isRunning != data.sprintHold)
        {
            _isRunning = data.sprintHold;
            _isCrouch = false;
        }
        if (_isCrouch != data.crouch)
        {
            _isCrouch = data.crouch;
            ToogleCrouch(_isCrouch);
        }
        if(_isWalkStealth != data.stealthWalk) _isWalkStealth = data.stealthWalk;

        //Local Value
        _localPlayerActionData.isRunning = _isRunning;
        _localPlayerActionData.isCrouch = _isCrouch;
        _localPlayerActionData.isStealthWalk = _isWalkStealth;

        //Set Animation For the Current Equiped Item if it uses Animations
        if (_itemMotionAnimation != null) _itemMotionAnimation.SetRun(_isRunning);


          
    }

    [Reconcile]
    private void Reconcile(ReconcileData recData, bool asServer)
    {
        //Reset the client to the received position. It's okay to do this
        //even if there is no de-synchronization.
        transform.position = recData.position;
        _currentVerticalVelocity = recData.verticalVelocity;
        _currentStamina = recData.stamina;
        _currentMoveSpeed = recData.moveSpeed;
        _movementInputs = recData.movementInputs;
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
        if (_isRunning)
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
        }else if (_localPlayerActionData.onLadder)
        {
            _maxAcceleration = _onLadderSpeed;
        }
        else
        {
            _maxAcceleration = _walkSpeed;
        }

        _fullBodyAnimatorHandler.SetCrouch(_isCrouch);
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



    public override void TakeFallDamage()
    {
        base.TakeFallDamage();
    }


    public override void SetPlayerLadder(Vector3 forwardDirection, Vector3 startPos, LadderScript ladder)
    {
        if (!CanClimbLadder()) return;
        _currentLadder = ladder;
        Debug.Log("On Ladder Now");
        ServerSetPlayerLadder(startPos, forwardDirection);
        SetLadderAction(startPos, forwardDirection);
    }


    private void SetLadderAction(Vector3 startPos, Vector3 rotation)
    {
        if (!CanClimbLadder()) return;

        _fullBodyAnimatorHandler.SetOnLadder(true);
        _localPlayerActionData.onLadder = true;
        _playerInventoryHandler.HostlerItem();

        StartCoroutine(LadderTransition(true, startPos, rotation));

        ToogleSpeed(true, true, base.IsServer);
    }

    [ServerRpc]
    private void ServerSetPlayerLadder(Vector3 startPos, Vector3 rotation)
    {
        if (base.IsServer)
        {
            ObserverSetPlayerLadder(startPos, rotation);
            SetLadderAction(startPos, rotation);
        }
    }

    [ObserversRpc]
    private void ObserverSetPlayerLadder(Vector3 startPos, Vector3 rotation)
    {
        if (base.IsOwner || base.IsServer) return;
        Debug.Log("Observers On Ladder Now");
        SetLadderAction(startPos, rotation);
    }

    public override void ForceExitPlayerLadder()
    {
        if (base.IsOwner)
            ServerForceExitLadder();
    }


    [ServerRpc]
    private void ServerForceExitLadder()
    {
        if (base.IsServer)
        {
            ObserverForceExitLadder();
            ForceExitLadderAction();
        }
    }

    [ObserversRpc]
    private void ObserverForceExitLadder()
    {
        if (base.IsServer) return;
        ForceExitLadderAction();
    }

    private void ForceExitLadderAction()
    {
        if (_currentLadder != null)
        {
            _currentLadder = null;
        }


        _playerInventoryHandler.UnhostlerItem();
        _fullBodyAnimatorHandler.SetOnLadder(false);
        _localPlayerActionData.onLadder = false;
        _characterController.enabled = true;
        _characterController.radius = _defaultCharacterControllerRadius;

        ToogleSpeed(true, true, base.IsServer);
    }


    public override void AnimatedExitPlayerLadder(Vector3 forwardDirection, Vector3 targetPos)
    {
        if (base.IsOwner)
            ServerAnimatedExitLadder(forwardDirection, targetPos);
    }

    [ServerRpc]
    private void ServerAnimatedExitLadder(Vector3 forwardDirection, Vector3 targetPos)
    {
        if (base.IsServer)
        {
            ObserverAnimatedExitLadder(forwardDirection, targetPos);
            AnimatedExitLadderAction(forwardDirection, targetPos);
        }
    }

    [ObserversRpc]
    private void ObserverAnimatedExitLadder(Vector3 forwardDirection, Vector3 targetPos)
    {
        if (base.IsServer) return;
        AnimatedExitLadderAction(forwardDirection, targetPos);
    }

    private void AnimatedExitLadderAction(Vector3 forwardDirection, Vector3 targetPos)
    {
        _playerInventoryHandler.UnhostlerItem();

        _fullBodyAnimatorHandler.SetOnLadder(false);
        _fullBodyAnimatorHandler.PlayLadderExit(true);

        StartCoroutine(LadderTransition(false, targetPos, forwardDirection));
    }

    public override void PlayLandSound(bool isFallWithDamage)
    {
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
        PooledLandSound(isFallWithDamage);
    }
}
