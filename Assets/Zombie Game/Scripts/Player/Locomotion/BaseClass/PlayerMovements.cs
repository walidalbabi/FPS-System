using FishNet.Managing.Logging;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerMovements : NetworkBehaviour
{    
    
    //Serilized 
    [Header("Physics Settings")]
    [Tooltip("How much to dampen external forces by per fixed update. This is applied at the beginning of fixed update.")]
    [SerializeField] protected float _forceDampening = 5f;
    [SerializeField] protected float _jumpImpuls = 7f;
    [SerializeField] protected float _gravity = 7f;
    [SerializeField] protected float _minFallToTakeDamage = 2f;
    [SerializeField] protected int _fallDamageMultiplier = 5;
    [Space]
    [Header("View Settings")]
    [SerializeField] protected float _pitchSpeed = 100f;
    [SerializeField] protected float _yawSpeed = 100f;
    [Space]
    [Header("Speed Settings")]
    [SerializeField] protected float _walkStealthSpeed = 2f;
    [SerializeField] protected float _walkSpeed = 3.5f;
    [SerializeField] protected float _runSpeed = 5f;
    [SerializeField] protected float _crouchSpeed = 2f;
    [SerializeField] protected float _accelerationSpeed = 0.3f;
    [Space]
    [Header("Stamina Settings")]
    [SerializeField] protected float _maxStamina = 100f;
    [SerializeField] protected float _staminaDepletionRate = 5f;
    [SerializeField] protected float _staminaDepletionOnJump = 15f;
    [SerializeField] protected float _staminaRecoveryRate = 7f;
    [Space]
    [Header("Configurations")]
    [SerializeField] protected bool _canMoveInAllDirectionsWhileSprinting;
    [SerializeField] protected LayerMask _checkForCanJumpLayers;
    [Space]
    [Header("Sounds")]
    [SerializeField] private AudioClip[] _jumpSound;
    [SerializeField] private AudioSettings _jumpAudioSettings;
    [SerializeField] protected AudioSettings _landAudioSettings;

    //Private Members
    private Vector3 _lookDirection;
    private Vector3 _externalForces;
    private bool _wasFalling;
    private bool _wasGrounded;
    private bool _isGrounded;
    private float _startFallPos;
    private float _fallingTime;

    private bool _isFalling { get { return !_isGrounded && _characterController.velocity.y < 0; } }

    /// <summary>
    /// Starting step offset for the controller.
    /// </summary>
    private float _defaultStepOffset;

    protected ItemMotionAnimations _itemMotionAnimation;

    //Protected Members
    protected float _currentVerticalVelocity;
    protected float _pitch;
    protected float _yaw;
    protected float _currentMoveSpeed;
    protected float _maxAcceleration;
    protected float _currentStamina;
    protected bool  _canRunAfterStamina;



    //Public

    public event Action<int> OnFallDamageTaken;
    /// <summary>
    /// Used for Locking the player from controlling the character
    /// </summary>
    [HideInInspector]
    public bool isLockController;

    //Components
    protected CharacterController _characterController;
    protected PlayerHealth _playerHealth;
    protected LocalPlayerData _localPlayerActionData;
    protected PlayerFootSteps _playerFootSteps;
    protected NetworkPlayerFullBodyAnimationHandler _fullBodyAnimatorHandler;
    protected PlayerInventoryHandler _playerInventoryHandler;
    protected InputHandler _inputHandler;
    protected InputData _networkInputs;


    //Propeties
    public Vector3 lookDirection { get { return _lookDirection; } }
    public float pithWithRecoil { get; set; }
    public float currentMoveSpeed { get { return _currentMoveSpeed; } }
    public float currentStamina { get { return _currentStamina; } }
    public float velocityY { get { return _characterController.velocity.y; } }
    public bool isGrounded { get { return _isGrounded; } }

    public LocalPlayerData localPlayerDataAction { get { return _localPlayerActionData; } }


    public virtual void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _localPlayerActionData = GetComponent<LocalPlayerData>();
        _playerHealth = GetComponent<PlayerHealth>();
        _playerFootSteps = GetComponent<PlayerFootSteps>();
        _characterController = GetComponent<CharacterController>();
        _fullBodyAnimatorHandler = GetComponent<NetworkPlayerFullBodyAnimationHandler>();
        _playerInventoryHandler = GetComponent<PlayerInventoryHandler>();

        _currentStamina = _maxStamina;
        _defaultStepOffset = _characterController.stepOffset;


        _playerInventoryHandler.OnNewITemEquipedAndReadyToUSeItem += OnNewItemIsSelected;
    }

    private void OnDestroy()
    {
        _playerInventoryHandler.OnNewITemEquipedAndReadyToUSeItem -= OnNewItemIsSelected;
    }


    private void OnNewItemIsSelected(int index, PlayerItem newItem)
    {
        _itemMotionAnimation = newItem.GetComponent<ItemMotionAnimations>();
    }


    public virtual void Update()
    {
        if (_localPlayerActionData.isRunning)
        {
            //Calcculate Stamina  Depletion
            if (_currentStamina > 0)
                _currentStamina -= Time.deltaTime * _staminaDepletionRate;
            else _canRunAfterStamina = false;
        }
        else
        {
            //Caclculate Stamina Recovery
            if (!_isGrounded) return;
            if (_currentStamina < _maxStamina) _currentStamina += Time.deltaTime * _staminaRecoveryRate;
            if (_currentStamina > _maxStamina / 3) _canRunAfterStamina = true; // Can run again after regenerate 3/4 of the stammina
        }

    }

    public virtual void Move(Vector3 moveDir, float time, bool isRunning)
    {
        if (!CanMove()) return;

        if (isRunning)
        {
            if (!_canMoveInAllDirectionsWhileSprinting)     //Can't move Left/right and backward while Running
            {
                moveDir.x = 0;
                moveDir.y = 1;
            }
        }

        Vector3 moveInputs = transform.forward * moveDir.y + transform.right * moveDir.x;


        if (moveInputs.magnitude > 0.1f)
        {
            if (_currentMoveSpeed < _maxAcceleration)
                _currentMoveSpeed += _accelerationSpeed * time;
            else _currentMoveSpeed -= _accelerationSpeed * 3f * time;
        }
        else if (moveInputs.magnitude <= 0.1f)
        {
            if (_currentMoveSpeed >= 0) _currentMoveSpeed = 0;
                //_currentMoveSpeed -= _accelerationSpeed * time;
        }

        _currentMoveSpeed = Mathf.Clamp(_currentMoveSpeed, 0f, _runSpeed);
        //  _currentMoveSpeed = Mathf.Clamp(_maxAcceleration, 0f, _runSpeed);

        moveInputs.Normalize();
        moveInputs *= _currentMoveSpeed;

        if (!_characterController.isGrounded)
            _currentVerticalVelocity += (Physics.gravity.y * time * _gravity);  //Subtract gravity from the vertical velocity.
       /// else _currentVerticalVelocity = 0; // Reset Gravity if we are not grounded

        //Perhaps prevent the value from getting too low.
        _currentVerticalVelocity = Mathf.Max(-20f, _currentVerticalVelocity);

        //Add vertical velocity to the movement after movement is normalized.
        //You don't want to normalize the vertical velocity.
        moveInputs += new Vector3(0f, _currentVerticalVelocity, 0f);
        //Move your character!
        _characterController.Move(moveInputs * time);
    }

    public virtual void UpdateRotation(Vector2 viewInputs, float time)
    {
        if (!CanLook()) return;

        float delta = time;
        _pitch += viewInputs.y * delta * _pitchSpeed;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);
   
        _yaw += viewInputs.x * delta * _yawSpeed;

        transform.Rotate(0f, viewInputs.x * delta * _yawSpeed, 0f);

        _lookDirection = new Vector3(_pitch, transform.eulerAngles.y, transform.eulerAngles.z);

    }

    public virtual void Jump(bool isJump, float jumpForce, bool isReplaying)
    {
        if (!CanJump()) return;

        if (isJump)
        {
            if (!isReplaying)
            {
                _currentStamina -= _staminaDepletionOnJump;
                OnPlayerJump();
            }

            _currentVerticalVelocity = jumpForce;
        }
    }


    public virtual void OnPlayerJump()
    {
        if (_itemMotionAnimation != null) _itemMotionAnimation.SetJump();        //Set Animation For the Current Equiped Item if it uses Animations

        _fullBodyAnimatorHandler.SetJump();
        PlayJumpSound();
    }


    /// <summary>
    /// Sets velocity to 0f when grounded.
    /// </summary>
    protected void SetGroundedVelocity(float deltaTime, bool asServer, bool replaying)
    {
        /* Rapidly reduce gravity when grounded so falls aren't sudden when moving
         * off edges. Also move towards this gravity amount over time so gravity
         * isn't immediately reset upon landing, but rather is gradually to
         * give the impression of losing momentum. */
        if (_characterController.isGrounded && _currentVerticalVelocity < -1f)
            _currentVerticalVelocity = Mathf.MoveTowards(_currentVerticalVelocity, -1f, (-Physics.gravity.y * _gravity * 2f) * deltaTime);
    }

    /// <summary>
    /// Received when the character controller hits an object during move.
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Cancel jump velocity if hitting something above.
        if (_currentVerticalVelocity > 0f && hit.moveDirection.y > 0f)
        {
            //If hit is above middle of character controller it's safe to assume it's above.
            if (hit.point.y > transform.position.y + (_characterController.height / 2f))
                _currentVerticalVelocity = 0f;
        }
    }


    /// <summary>
    /// Sets if the controller is grounded.
    /// </summary>
    /// <returns>Returns if grounded has changed.</returns>
    protected void SetIsGrounded(bool replaying, out bool changed)
    {
        //State before checking for ground.
        bool previousGrounded = _isGrounded;
        _isGrounded = CastForGround();
        changed = (previousGrounded != _isGrounded);
    }

    /// <summary>
    /// Checks for ground beneath the player.
    /// </summary>
    /// <param name="extraDistance"></param>
    /// <returns></returns>
    private bool CastForGround(float extraDistance = 0.05f)
    {
        float radius = _characterController.radius + (_characterController.skinWidth / 2f);
        //Start right in the center.
        Vector3 start = transform.position + new Vector3(0f, (_characterController.height / 2f), 0f);
        float distance = (_characterController.height / 2f) - (radius / 2f);

        //Check for ground.
        Ray ray = new Ray(start, Vector3.down);
        RaycastHit hit;

        //Disable colliders on self.
        bool isGrounded = Physics.SphereCast(ray, radius, out hit, distance + extraDistance, _checkForCanJumpLayers);
        return isGrounded;
    }

    /// <summary>
    /// Get the surface beneath the player.
    /// </summary>
    /// <param name="extraDistance"></param>
    /// <returns></returns>
    protected SurfaceIdentifier CastForSurface(float extraDistance = 0.05f)
    {
        float radius = _characterController.radius + (_characterController.skinWidth / 2f);
        //Start right in the center.
        Vector3 start = transform.position + new Vector3(0f, (_characterController.height / 2f), 0f);
        float distance = (_characterController.height / 2f) - (radius / 2f);

        //Check for ground.
        Ray ray = new Ray(start, Vector3.down);
        RaycastHit hit;

        //Disable colliders on self.
        bool isGrounded = Physics.SphereCast(ray, radius, out hit, distance + extraDistance, _checkForCanJumpLayers);

        if (hit.collider == null)
        {
            return null;
        }

        SurfaceIdentifier surface = hit.collider.gameObject.GetComponent<SurfaceIdentifier>();
        if (isGrounded && surface != null)
        {
            return surface;
        }
        return null;
    }

    /// <summary>
    /// Conditionally adjust steps height.
    /// </summary>
    protected void SetStepOffset()
    {
        /* Don't allow stepping when in the air. This is so the client cannot step up on cliffs when falling in front of them.
         * This is an issue with the unity character controller that would maybe be good for ledge grabbing, but not for
         * a FPS game. */
        _characterController.stepOffset = (_isGrounded && _currentVerticalVelocity <= 0f) ? _defaultStepOffset : 0f;
    }

    /// <summary>
    /// Applies gravity to verticalVelocity.
    /// </summary>
    protected void ApplyGravity(ref float verticalVelocity, float deltaTime)
    {
        //Multiply gravity by 2 for snappier jumps.
        verticalVelocity += (Physics.gravity.y * _gravity) * deltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, Physics.gravity.y * _gravity);
    }

    /// <summary>
    /// Dampens current external forces.
    /// </summary>
    protected void DampenExternalForces(float deltaTime)
    {
        _externalForces = Vector3.MoveTowards(_externalForces, Vector3.zero, _forceDampening * deltaTime);
    }

    public virtual void ToogleSpeed(bool oldValue, bool newValue, bool asServer) { }

    public virtual void CheckIfWeAreFalling(bool replaying)
    {
        if (!_wasFalling && _isFalling) _startFallPos = transform.position.y;
        if (!_wasGrounded && _isGrounded)
        {
            TakeFallDamage();
        }

        if (_isFalling && !_isGrounded)
        {
            _fallingTime += Time.deltaTime * 20f;
        }
        else
        {
            _fallingTime = 0;
         
        }
        _fullBodyAnimatorHandler.SetVelocityY(_fallingTime);
        if (_itemMotionAnimation != null) _itemMotionAnimation.SetFall(_fallingTime > 2f);        //Set Animation For the Current Equiped Item if it uses Animations
        _wasGrounded = _isGrounded;
        _wasFalling = _isFalling;
    }

    public virtual void TakeFallDamage()
    {
        float fallDistance = _startFallPos - transform.position.y;
        if (fallDistance > _minFallToTakeDamage)
        {
            OnFallDamageTaken?.Invoke((int)fallDistance * _fallDamageMultiplier);
            PlayLandSound(true);
        }
        else
            PlayLandSound(false);

        _startFallPos = 0;
    }


    protected bool CanPressSprint()
    {
        if (isLockController) return false;
        if (!_isGrounded) return false;
        if (_localPlayerActionData.isAiming) return false;
        if (_localPlayerActionData.isFiring) return false;
        if (_localPlayerActionData.isReloading) return false;
        if (_localPlayerActionData.isDead) return false;
        if (!_canRunAfterStamina) return false;
        if (_currentStamina <= 0f) return false;

        return true;
    }

    protected bool CanMove()
    {
        if (isLockController) return false;
        if (_localPlayerActionData.isDead) return false;

        return true;
    }

    protected bool CanLook()
    {
        if (isLockController) return false;
        if (_localPlayerActionData.isDead) return false;

        return true;
    }

    protected bool CanJump()
    {
        /* Check for ground using a tiny bit of extra distance. This is for when going down slopes
         * where the player may break ground slightly. */
        if (isLockController) return false;
        if (!_isGrounded) return false;
        if (_localPlayerActionData.isDead) return false;
        if (_currentStamina <= _staminaDepletionOnJump) return false;

        return true;
    }


    public virtual void PlayJumpSound()
    {
        var audioObj = GameManager.instance.objPool.RetrievePoolAudio();
        if (audioObj == null) return;

        audioObj.transform.position = transform.position;
        audioObj.SetAudioSettings(_jumpAudioSettings,_jumpSound[UnityEngine.Random.Range(0, _jumpSound.Length)]);
        audioObj.Play();
    }


    public virtual void PlayLandSound(bool isFallWithDamage)
    {
        SurfaceIdentifier surface = CastForSurface();
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
}
