using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRagdoll : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private Camera _deathCamera;
    [SerializeField] private Rigidbody[] _ragdollBonesRb;
    private BonesCashedLocalPos[] _ragdollBonesCashedLocalPos;

    private Animator _fullBodyAnimator;
    private LocalPlayerData _localPlayerData;

    private void Awake()
    {

        _fullBodyAnimator = GetComponent<Animator>();
        _localPlayerData = _playerHealth.gameObject.GetComponent<LocalPlayerData>();

        for (int i = 0; i < _ragdollBonesRb.Length; i++)
        {
            _ragdollBonesRb[i].isKinematic = true;
        }
        _ragdollBonesCashedLocalPos = new BonesCashedLocalPos[_ragdollBonesRb.Length];

        _playerHealth.OnDeath += OnPlayerDie;
        _playerHealth.OnRespawned += OnPlayerRespawned;
    }

    private void Start()
    {
        for (int i = 0; i < _ragdollBonesRb.Length; i++) 
        {
            _ragdollBonesCashedLocalPos[i].LocalPos = _ragdollBonesRb[i].transform.localPosition;
            _ragdollBonesCashedLocalPos[i].LocalRot = _ragdollBonesRb[i].transform.localEulerAngles;
        }

    }

    private void OnDestroy()
    {
        _playerHealth.OnDeath -= OnPlayerDie;
        _playerHealth.OnRespawned -= OnPlayerRespawned;
    }

    private void OnPlayerDie()
    {
        foreach (var bone in _ragdollBonesRb)
        {
            bone.isKinematic = false;
        }
        if (_localPlayerData.Ownership.isOwner)
            _deathCamera.enabled = true;
        _fullBodyAnimator.enabled = false;
    }

    private void OnPlayerRespawned()
    {
        for (int i = 0; i < _ragdollBonesRb.Length; i++)
        {
            _ragdollBonesRb[i].isKinematic = true;
            _ragdollBonesRb[i].transform.localPosition = _ragdollBonesCashedLocalPos[i].LocalPos;
            _ragdollBonesRb[i].transform.localEulerAngles = _ragdollBonesCashedLocalPos[i].LocalRot;
        }

        if (_localPlayerData.Ownership.isOwner)
            _deathCamera.enabled = false;
        _fullBodyAnimator.enabled = true;
    }
}


public struct BonesCashedLocalPos
{
    public Vector3 LocalPos;
    public Vector3 LocalRot;
}
