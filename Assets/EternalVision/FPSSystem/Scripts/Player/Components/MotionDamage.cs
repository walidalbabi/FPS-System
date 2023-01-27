using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionDamage : MonoBehaviour
{

    [SerializeField] private Vector3 _hitMotion;
    [SerializeField] private float _motionDamageModifier = 10f;

    private PlayerHealth _playerHealth;

    private Vector3 targetRotation;

    private void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned;
    }


    private void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawned;

        if (_playerHealth != null) _playerHealth.OnDamageTaken -= OnTakeDamage;
    }


    private void Update()
    {
        OnUpdate();
    }

    private void OnPlayerSpawned(GameObject objPlayer)
    {
        _playerHealth = objPlayer.GetComponent<PlayerHealth>();


        if (_playerHealth != null) _playerHealth.OnDamageTaken += OnTakeDamage;

    }


    public void OnTakeDamage(int damage)
    {
        var dmgmotion = damage / _motionDamageModifier;
        targetRotation += new Vector3(_hitMotion.x, Random.Range(-_hitMotion.y, _hitMotion.y), Random.Range(-_hitMotion.z, _hitMotion.z)) * dmgmotion;
    }

    public  void OnUpdate()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, 10 * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles+targetRotation);
    }

}
