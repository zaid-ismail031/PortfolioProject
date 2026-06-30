using System.Diagnostics;
using UnityEngine;

namespace DIGA7009A
{
  public class BulletProjectile : MonoBehaviour
  {
    private Rigidbody bulletRigidBody;
    private float bulletSpeed = 0.0f;
    private bool destroyOnCollision = false;

    private DIGA7009A.Tools.Stopwatch bulletLifeTimer = null;

    void Awake()
    {
      bulletRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
      bulletRigidBody.linearVelocity = transform.forward * bulletSpeed;
      bulletLifeTimer = new DIGA7009A.Tools.Stopwatch( 3.0f );
    }

    private void OnCollisionEnter( Collision other )
    {
      if( other.gameObject.GetComponent<BulletTarget>() != null )
      {
        if( other.gameObject.GetComponent<IDamageable>() != null && other.gameObject.GetComponent<CapsuleCollider>() )
        {
          other.gameObject.GetComponent<IDamageable>().TakeDamage( 25.0f );

          Destroy( this.gameObject );
        }
      }
      else
      {
        int layerMask = LayerMask.GetMask( "World" );
        int enemyMask = LayerMask.GetMask( "Enemy" );
        if( other.gameObject.layer == layerMask || other.gameObject.layer == enemyMask )
        {
          Destroy( this.gameObject );
        }
      }
    }

    public void SetBulletSpeed( float speed )
    {
      bulletSpeed = speed;
    }

    public void ToggleDestroyOnCollision( bool toggle )
    {
      destroyOnCollision = toggle;
    }


    private void FixedUpdate()
    {
      if( !gameObject.name.Contains( "Clone" ) )
      {
        return;
      }
      bulletLifeTimer.Start();
      bulletLifeTimer.DoLogic( Time.fixedDeltaTime );

      if( bulletLifeTimer.HasElapsed() )
      {
        bulletLifeTimer.Stop();
        Destroy( this.gameObject );
      }
    }
  }
}

