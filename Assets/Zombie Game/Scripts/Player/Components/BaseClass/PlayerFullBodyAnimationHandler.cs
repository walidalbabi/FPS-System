using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFullBodyAnimationHandler : NetworkBehaviour
{
    [SerializeField] protected Animator _thirdPersonAnimator;

    [SerializeField] protected float _animationSmoothSpeed = 12f;

    [SerializeField] protected string _moveVelocity = "MoveVelocity";
    [SerializeField] protected string _velocityY = "VelocityY";
    [SerializeField] protected string _movement_Horizontal = "Horizontal";
    [SerializeField] protected string _movement_Vertical = "Vertical";
    [SerializeField] protected string _view_Pitch = "Pitch";
    [SerializeField] protected string _jump = "Jump";
    [SerializeField] protected string _run = "Run";
    [SerializeField] protected string _stealthWalk = "StealthWalk";
    [SerializeField] protected string _fire = "Fire";
    [SerializeField] protected string _reload = "Reload";

    protected int HASH_MoveVelocity;
    protected int HASH_VelocityY;
    protected int HASH_MovementHorizontal;
    protected int HASH_MovementVertical;
    protected int HASH_Jump;
    protected int HASH_Run;
    protected int HASH_Pitch;
    protected int HASH_StealthWalk;
    protected int HASH_Fire;
    protected int HASH_Reload;



    protected Vector3 smoothedVar_movements;
    protected float smoothedVar_velocity;
    protected float soothedVar_pitch;


    public virtual void Awake()
    {
        //Get Hashes
        HASH_MoveVelocity = Animator.StringToHash(_moveVelocity);
        HASH_VelocityY = Animator.StringToHash(_velocityY);
        HASH_MovementHorizontal = Animator.StringToHash(_movement_Horizontal);
        HASH_MovementVertical = Animator.StringToHash(_movement_Vertical);
        HASH_Pitch = Animator.StringToHash(_view_Pitch);
        HASH_Jump = Animator.StringToHash(_jump);
        HASH_Run = Animator.StringToHash(_run);
        HASH_StealthWalk = Animator.StringToHash(_stealthWalk);
        HASH_Fire = Animator.StringToHash(_fire);
        HASH_Reload = Animator.StringToHash(_reload);
    }


    public virtual void Update()
    {
        SmoothAnimations();
        SetMovementsAnimations();
        SetPitchAnimation();
    }



    private void SetMovementsAnimations()
    {
        _thirdPersonAnimator.SetFloat(HASH_MovementHorizontal, smoothedVar_movements.x);
        _thirdPersonAnimator.SetFloat(HASH_MovementVertical, smoothedVar_movements.y);
        _thirdPersonAnimator.SetFloat(HASH_MoveVelocity, smoothedVar_velocity);
    }

    private void SetPitchAnimation()
    {
        _thirdPersonAnimator.SetFloat(HASH_Pitch, soothedVar_pitch);
    }


    public virtual void SmoothAnimations() { }


    public void SetJump()
    {
        _thirdPersonAnimator.SetTrigger(HASH_Jump);
    }

    public void SetVelocityY(float velocityY)
    {
        _thirdPersonAnimator.SetFloat(HASH_VelocityY, velocityY);
    }

    public void SetRun(bool isRunning)
    {
        _thirdPersonAnimator.SetBool(HASH_Run, isRunning);
    }


    public void SetStealthWalk(bool isStealthWalk)
    {
        _thirdPersonAnimator.SetBool(HASH_StealthWalk, isStealthWalk);
    }


    public void SetFire()
    {
        _thirdPersonAnimator.SetTrigger(HASH_Fire);
    }

    public void SetReload()
    {
        _thirdPersonAnimator.SetTrigger(HASH_Reload);
    }


    public bool CheckForLockedMovementBool()
    {
        return _thirdPersonAnimator.GetBool("LockMovements");
    }
}
