using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTarget : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private int _waypointsIndex;

    Vector3 target;
    float distance;

    private Hitbox _currentHitbox;
    private MeshRenderer _renderer;

    private bool isHit;
    private float _timer;

    private void Awake()
    {
        _currentHitbox = GetComponentInChildren<Hitbox>();
        _renderer = GetComponentInChildren<MeshRenderer>();

        if (_currentHitbox != null)
            _currentHitbox.OnHit += OnHit;
    }

    private void OnDestroy()
    {
        if (_currentHitbox != null)
            _currentHitbox.OnHit -= OnHit;
    }

    // Update is called once per frame
    void Update()
    {

        if (isHit)
        {
            _renderer.material.color = Color.red;
            if (_timer <= 0.3f)
            {
                _timer += Time.deltaTime * 2f;
            }
            else
            {
                isHit = false;
                _timer = 0;
                _renderer.material.color = Color.white;
            }
        }

        if (base.IsOwner || base.IsServer)
        {


            distance = Vector3.Distance(transform.position, _waypoints[_waypointsIndex].position);

            if (distance >= 3f)
            {
                target = Vector3.Lerp(transform.position, _waypoints[_waypointsIndex].position, Time.deltaTime * _moveSpeed);
                transform.position = target;
            }
            else
            {
                _waypointsIndex = _waypointsIndex < (_waypoints.Length - 1) ? _waypointsIndex = _waypointsIndex + 1 : 0;
            }
        }

    }

    private void OnHit(Hitbox hit, int damage, GameObject objectHitter)
    {
        if (!GameManager.instance.networkContext.Ownership.isServer) return;
        isHit = true;
        _timer = 0;
        ObserversSetHit();
    }

    [ObserversRpc]
    private void ObserversSetHit()
    {
        isHit = true;
        _timer = 0;
    }

}
