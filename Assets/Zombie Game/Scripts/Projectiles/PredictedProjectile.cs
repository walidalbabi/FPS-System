using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictedProjectile : Projectile
{

    [SerializeField] private int _bulletDamage;



    /// <summary>
    /// Direction to travel.
    /// </summary>
    private Vector3 _direction;
    /// <summary>
    /// Distance remaining to catch up. This is calculated from a passed time and move rate.
    /// </summary>
    private float _passedTime = 0f;



    public override void AddProjectile(Vector3 postion, Vector3 direction, float passedTime)
    {
        base.AddProjectile(postion, direction, passedTime);
        transform.position = postion;
        _direction = direction;
        _passedTime = passedTime;
    }


    public override void Update()
    {
        Move();
    }

    /// <summary>
    /// Move the projectile each frame. This would be called from Update.
    /// </summary>
    private void Move()
    {
        //Frame delta, nothing unusual here.
        float delta = Time.deltaTime;

        //See if to add on additional delta to consume passed time.
        float passedTimeDelta = 0f;
        if (_passedTime > 0f)
        {
            /* Rather than use a flat catch up rate the
             * extra delta will be based on how much passed time
             * remains. This means the projectile will accelerate
             * faster at the beginning and slower at the end.
             * If a flat rate was used then the projectile
             * would accelerate at a constant rate, then abruptly
             * change to normal move rate. This is similar to using
             * a smooth damp. */

            /* Apply 8% of the step per frame. You can adjust
             * this number to whatever feels good. */
            float step = (_passedTime * 0.08f);
            _passedTime -= step;

            /* If the remaining time is less than half a delta then
             * just append it onto the step. The change won't be noticeable. */
            if (_passedTime <= (delta / 2f))
            {
                step += _passedTime;
                _passedTime = 0f;
            }
            passedTimeDelta = step;
        }

        //Move the projectile using moverate, delta, and passed time delta.
        transform.position += _direction * (_projectileSpeed * (delta + passedTimeDelta));
    }


    /// <summary>
    /// Handles collision events.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

        /* These projectiles are instantiated locally, as in,
         * they are not networked. Because of this there is a very
         * small chance the occasional projectile may not align with
         * 100% accuracy. But, the differences are generally
         * insignifcant and will not affect gameplay. */

        _surface = collision.gameObject.GetComponent<SurfaceIdentifier>();
        var hitBox = collision.gameObject.GetComponent<Hitbox>();
 

        //If client show visual effects, play impact audio.
        if (GameManager.instance.networkContext.Ownership.isClient)
        {
            if (_surface != null)
            {
                _objectPool.RetrievePoolBulletImpactObject(_surface);
            }
            //Show VFX.
            //Play Audio.
        }
        //If server check to damage hit objects.
        if (GameManager.instance.networkContext.Ownership.isServer)
        {
            //   PlayerShip ps = collision.gameObject.GetComponent<PlayerShip>();
            /* If a player ship was hit then remove 50 health.
             * The health value can be synchronized however you like,
             * such as a syncvar. */
            if(hitBox != null)
            {
                hitBox.Hit(_bulletDamage, null);
            }
        }

        //Destroy projectile (probably pool it instead).
        _objectPool.SendBackToPool(this);
    }
}
