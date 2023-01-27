using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadBobbing : MonoBehaviour
{
    [SerializeField] private bool _active;

    [SerializeField] private MotionHeadBobbing _runningBob;
    [SerializeField] private MotionHeadBobbing _norrmalBob;
    
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;



    private Vector3 _startPos;
    private LocalPlayerData _locaPlayerData;



    private void Awake()
    {
        PlayerSpawner.OnPlayerSpawned += OnPlayerSpawn;
    }

    private void OnDestroy()
    {
        PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawn;
    }

    private void OnPlayerSpawn(GameObject objPlayer)
    {
        _locaPlayerData = objPlayer.GetComponent<LocalPlayerData>();
    }

    // Update is called once per frame
    void Update()
    {
        ResetPosition();

        if (_locaPlayerData == null) return;

        if (!_active) return;

        CheckRunningMotion();
        CheckMotion();
     //   _camera.LookAt(FocusTarget());
    }


    private void CheckRunningMotion()
    {
        if (_locaPlayerData.isRunning)
            PlayMotionRotation(FootStepMotionRotation());
    }

    private void CheckMotion()
    {
        if (!_locaPlayerData.isRunning)
            PlayMotionPosition(FootStepMotionPostion());
    }

    private void PlayMotionRotation(Vector3 motion)
    {
        //_camera.localPosition += motion;
        Quaternion rot = Quaternion.Euler(0f,0f,motion.z * 10f);
        _camera.localRotation = rot;
    }

    private void PlayMotionPosition(Vector3 motion)
    {
        _camera.localPosition += motion;
    }

    private Vector3 FootStepMotionPostion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sign(Time.time * _norrmalBob._frequency) * _norrmalBob._amplitude / 2f;
        pos.x += Mathf.Cos(Time.time * _norrmalBob._frequency / 2f) * _norrmalBob._amplitude * 2f;
        return pos;
    }

    private Vector3 FootStepMotionRotation()
    {
        Vector3 pos = Vector3.zero;
        pos.z += Mathf.Cos(Time.time * _runningBob._frequency / 2f) * _runningBob._amplitude * 20f;
        return pos;
    }

    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 5 * Time.deltaTime);
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localEulerAngles.y, transform.position.z);
        pos += _cameraHolder.forward * 15f;
        return pos;
    }
}


[System.Serializable]
public class MotionHeadBobbing
{
    [SerializeField, Range(0, 0.1f)] public float _amplitude = 0.015f;
    [SerializeField, Range(0, 30f)] public float _frequency = 10f;
}