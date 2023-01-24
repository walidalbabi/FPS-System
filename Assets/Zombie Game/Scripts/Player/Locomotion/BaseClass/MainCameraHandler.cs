using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainCameraHandler : MonoBehaviour
{

    /// <summary>
    /// Camera used to view the world.
    /// </summary>
    [Header("Cameras")][Tooltip("Camera used to view the world.")]
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private Camera _weaponCamera;
    [Header("Motions")]
    [SerializeField]
    private Transform _fpsHandParent;

    [Header("Settings")]
    /// <summary>
    /// Teleport camera if it is more than this distance from target position.
    /// </summary>
    [Tooltip("Teleport camera if it is more than this distance from target position.")]
    [SerializeField]
    private float _teleportDistance = 1.5f;
    ///// <summary>
    ///// How quickly to smooth position to goal.
    ///// </summary>
    /// <summary>
    /// How long the camera must be moving until minimum smoothing rate is achieved.
    /// </summary>
    [Tooltip("How long the camera must be moving until minimum smoothing rate is achieved.")]
    [SerializeField]
    private float _smoothedPositionalTime = 0.75f;
    [SerializeField]
    private float _rotationFollowSpeed = 20f;

    [Range(0.1f, 1f)]
    public float _rotationFollowSpeedMultiplayer = 1f;

    [SerializeField] private float _positionalSmoothingRateMin;
    [SerializeField] private float _positionalSmoothingRateMax;

    /// <summary>
    /// Current Looking of the localPlayer.
    /// </summary>
    private PlayerMovements _looking;

    /// <summary>
    /// Arms from the spawned player which are attached to the camera.
    /// </summary>
    private GameObject _firstPersonArms;

    /// <summary>
    /// How long the camera has been trying to catch up to it's target.
    /// </summary>
    private float _movingTime = 0f;

    //Components
    private BodyConfiguration _bodiesConfigurations;
    private PlayerController _playerController;
    private PlayerHealth _playerHealth;


    public virtual void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += SetupPlayerHandsAndCameraTarget;
    }

    public virtual void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned += SetupPlayerHandsAndCameraTarget;
        if (_playerHealth != null)
        {
            _playerHealth.OnRespawned -= SnapTheCameraToPlayer;
        }
    }


    private void SetupPlayerHandsAndCameraTarget(GameObject agentObject)
    {
        if (_firstPersonArms != null) Destroy(_firstPersonArms);

        //Object of player exist.
        if (agentObject != null)
        {
            _looking = agentObject.GetComponent<PlayerMovements>();
            _bodiesConfigurations = agentObject.GetComponent<BodyConfiguration>();
            _playerController = agentObject.GetComponent<PlayerController>();
            _playerHealth = agentObject.GetComponent<PlayerHealth>();
            //Move arms and camera.
            _mainCamera.transform.localPosition = Vector3.zero;
            _mainCamera.transform.eulerAngles = Vector3.zero;
            _firstPersonArms = _bodiesConfigurations.fpHand.gameObject;
            _firstPersonArms.transform.parent = _fpsHandParent;
            _firstPersonArms.transform.localPosition = Vector3.zero;
            _firstPersonArms.transform.eulerAngles = Vector3.zero;
            _mainCamera.enabled = true;
            _weaponCamera.enabled = true;

            _playerController.Event_OnCameraTargetChanged(_mainCamera.gameObject);

            if (_playerHealth != null)
            {
                _playerHealth.OnRespawned += SnapTheCameraToPlayer;
            }

            //Snap to position and rotation.
            SnapTheCameraToPlayer();
        }
    }

    /// <summary>
    /// Updates the cameras position and rotation to the player.
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void UpdatePositionAndRotation(float deltaTime)
    {
        if (_looking != null)
        {
            /* Position. */
            Vector3 targetPosition = _bodiesConfigurations.fullBodyArmsMatcher.position;
            //Only update position if not currently at position.
            if (transform.position != targetPosition)
            {
                float distance = Mathf.Max(0.1f, Vector3.Distance(transform.position, targetPosition));

                if (distance >= _teleportDistance)
                {
                    transform.position = targetPosition;
                }
                else
                {
                  //  _movingTime += deltaTime;
                  //  float smoothingPercent = (_movingTime / _smoothedPositionalTime);
                  //  float smoothingRate = Mathf.Lerp(_positionalSmoothingRateMax, _positionalSmoothingRateMin, smoothingPercent);
                    transform.position = Vector3.Slerp(transform.position, targetPosition, _positionalSmoothingRateMin  * deltaTime);
                }
            }
            //At position.
            else
            {
                _movingTime = 0f;
            }
            /* Rotation. */
            var rot = Quaternion.Euler(_looking.lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * (_rotationFollowSpeed * _rotationFollowSpeedMultiplayer));
        }
    }


    private void SnapTheCameraToPlayer()
    {
        UpdatePositionAndRotation(float.MaxValue);
    }

}
