using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThirdPersonModule : MonoBehaviour
{
    [SerializeField] private Transform _muzzleTip;
    [SerializeField] private ParticleSystem _muzzlEffect;

    [SerializeField] private Animator _animator;

    public Transform muzzleTip { get { return _muzzleTip; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Shoot(string fireAnimatorString)
    {
        _muzzlEffect.Play();
        if (_animator != null) _animator.SetTrigger(fireAnimatorString);
    }
}
