using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBulletVisual : Projectile
{
    [SerializeField] private float _delayBeforeEnablingTrail;

    private TrailRenderer _trailRenderer;
    private float _currentTrailTime;

    public override void Awake()
    {
        base.Awake();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
    }


    public override void Update()
    {
        base.Update();
        EnableTrailRendererAfterTime();
    }

    public override void AddProjectile(Vector3 startPos, Vector3 endPos, SurfaceIdentifier surface)
    {
        _trailRenderer.Clear();
        _trailRenderer.emitting = false;
        _currentTrailTime = _delayBeforeEnablingTrail;
        base.AddProjectile(startPos, endPos, surface);
    }


    private void EnableTrailRendererAfterTime()
    {
        if (_currentTrailTime > 0f)
        {
            _currentTrailTime -= Time.deltaTime;
        }
        else if (!_trailRenderer.emitting) _trailRenderer.emitting = true;
    }
}
