using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerFootSteps : NetworkBehaviour
{
    [Header("Walkable Layers")]
    [SerializeField] private LayerMask _layer;
    [Space]
    [Header("Settings")]
    [Space]
    [Header("Time Settings")]
    [SerializeField] private float _walkIntervalWalk;
    [SerializeField] private float _walkIntervalStealthWalk;
    [SerializeField] private float _walkIntervalRun;
    [SerializeField] private float _walkIntervalCrouch;
    [Header("Sound Settings")]
    [SerializeField] private AudioSettings _walkStealthSettings;
    [SerializeField] private AudioSettings _walkSettings;
    [SerializeField] private AudioSettings _crouchSettings;
    [SerializeField] private AudioSettings _SprintSettings;

    private FullBodyAnimationEventsHandler _fullBodyAnimationEventsHandler;
    private LocalPlayerData _localPlayerData;
    private PlayerMovements _playerMovement;

    private bool _canPlaySound;
    private float _maxInterval;
    private float _timeToPlayAnotherSound;

    private void Awake()
    {
        _fullBodyAnimationEventsHandler = GetComponentInChildren<FullBodyAnimationEventsHandler>();
        _localPlayerData = GetComponentInParent<LocalPlayerData>();
        _playerMovement = GetComponentInParent<PlayerMovements>();

     //   if (_fullBodyAnimationEventsHandler != null) _fullBodyAnimationEventsHandler.OnPlayerFootSteps += PlayFootsteps;
    }

    private void OnDestroy()
    {
      //  if (_fullBodyAnimationEventsHandler != null) _fullBodyAnimationEventsHandler.OnPlayerFootSteps -= PlayFootsteps;
    }


    private void Start()
    {
        _maxInterval = _walkIntervalWalk;
        _canPlaySound = true;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.green);

        if(_playerMovement.currentMoveSpeed > 0)
        {
            if (!_canPlaySound)
            {
                if (_timeToPlayAnotherSound < _maxInterval)
                {
                    _timeToPlayAnotherSound += Time.deltaTime;
                }
                else _canPlaySound = true;
            }
            else
            {
                //   PlayFootsteps(BoolToState(_localPlayerData.isRunning, _localPlayerData.isCrouch, _localPlayerData.isStealthWalk));
                ObserversPlayFootsteps(BoolToState(_localPlayerData.isRunning, _localPlayerData.isCrouch, _localPlayerData.isStealthWalk));
                _timeToPlayAnotherSound = 0;
                _canPlaySound = false;
            }
        }
      
    }



    public void PlayFootsteps(string state)
    {
        AudioClip audioClip = GetCurrentSurfaceAudio();
        if (audioClip == null) return;

        var AudioObject = GameManager.instance.objPool.RetrievePoolAudio();
        if (AudioObject == null) return;

        AudioSettings settings = GetAudioSetting(state);


        AudioObject.transform.position = transform.position;
        AudioObject.SetAudioSettings(settings, audioClip);
        AudioObject.Play();
    }

    [ObserversRpc]
    public void ObserversPlayFootsteps(string state)
    {
        PlayFootsteps(state);
    }

    private AudioClip GetCurrentSurfaceAudio()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position ,Vector3.down, out hit, 1.5f, _layer))
        {
            var surface = hit.collider.gameObject.GetComponent<SurfaceIdentifier>();
            if (surface == null) return null;
            else
            {
               return surface.surfaceData.footSteps[Random.Range(0, surface.surfaceData.footSteps.Length)];
            }
        }

        return null;
    }


    private AudioSettings GetAudioSetting(string state)
    {
        switch (state)
        {
            case "WalkStealth":
                _maxInterval = _walkIntervalStealthWalk;
                return _walkStealthSettings;
            case "Walk":
                _maxInterval = _walkIntervalWalk;
                return _walkSettings;
            case "Crouch":
                _maxInterval = _walkIntervalCrouch;
                return _crouchSettings;
            case "Sprint":
                _maxInterval = _walkIntervalRun;
                return _SprintSettings;
            default:
                return default;
        }
    }

    private string BoolToState(bool isRun, bool isCrouch, bool isWalkStealth)
    {
        if (isRun) return "Sprint";
        else if (isCrouch) return "Crouch";
        else if (isWalkStealth) return "WalkStealth";
        else return "Walk";
    }
}
