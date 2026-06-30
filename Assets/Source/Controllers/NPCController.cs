using DIGA7009A;
using DIGA7009A.Tools;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof( NavMeshAgent ) )]
public class NPCController : MonoBehaviour, IDamageable
{
  public enum PatrolState
  {
    TravellingToA,
    TravellingToB
  }

  public float NPCHealthPoints { get; private set; }

  [Header( "Navigation" )]
  [SerializeField]
  private Transform PlayerCharacterTarget;
  [SerializeField]
  private Transform PatrolPointA;
  [SerializeField] 
  private Transform PatrolPointB;
  [SerializeField]
  private float PatrolThreshold = 0.3f;
  [SerializeField]
  private float WanderRadius = 10f;
  [SerializeField]
  private float WanderWaitTime = 2f;

  private Vector3 wanderOrigin;
  private Vector3 wanderTarget;
  private bool hasWanderTarget;
  private float wanderWaitTimer;

  [Header( "Combat" )]
  [SerializeField]
  private float PunchRange = 2.0f;
  [SerializeField]
  private float PunchDamage = 10.0f;
  [SerializeField]
  private float PunchCooldown = 1.5f;

  [Header( "Shooting" )]
  [SerializeField]
  private GameObject BulletProjectile;
  [SerializeField]
  private Transform SpawnBulletPosition;
  [SerializeField]
  private float ShootCooldown = 1.0f;
  [SerializeField]
  private float BulletSpeed = 10.0f;
  [SerializeField]
  private float ShootRange = 15.0f;

  [Header( "Animation" )]
  [SerializeField]
  private float AnimRunSpeed = 5.335f;

  [Header( "Audio" )]
  [SerializeField]
  private AudioClip LandingAudioClip;
  [SerializeField]
  private AudioClip[] FootstepAudioClips;
  [Range( 0, 1 )] public float FootstepAudioVolume = 0.5f;
  [SerializeField]
  private float AnimSmoothTime = 10f;

  [Header( "Healing" )]
  [SerializeField]
  private float HealAmount = 5.0f;
  [SerializeField]
  private float HealRange = 2.0f;
  [SerializeField]
  private float HealCooldown = 2.0f;
  private float healCooldownTimer;

  private NavMeshAgent agent;
  private Animator animator;
  private int animIDSpeed;
  private int animIDMotionSpeed;
  private int animIDPunch;
  private int animIDCower;
  private int animIDHeal;
  private float animationBlend;
  private float punchCooldownTimer;
  private float shootCooldownTimer;
  private PatrolState patrolState;

  private OnValueChangeCallback<bool> patrolStateChecker;
  
  
  //Hack to heal for FSM
  private bool isRunToHeal = false;
  private float runAwayTimer;

  private void Start()
  {
    agent = GetComponent<NavMeshAgent>();
    animator = GetComponent<Animator>();

    animIDSpeed = Animator.StringToHash( "Speed" );
    animIDMotionSpeed = Animator.StringToHash( "MotionSpeed" );
    animIDPunch = Animator.StringToHash( "Punch" );
    animIDCower = Animator.StringToHash( "Cower" );
    animIDHeal = Animator.StringToHash( "Heal" );

    patrolState = PatrolState.TravellingToA;
    wanderOrigin = transform.position;

    patrolStateChecker = new OnValueChangeCallback<bool>( false, FlipPatrolState );
    NPCHealthPoints = 100.0f;
  }

  public bool IsCowering { get; private set; }
  public bool IsHealing { get; private set; }
  public bool IsDead { get; private set; }

  public void Chase()
  {
    if( IsCowering || PlayerCharacterTarget == null )
    {
      return;
    }

    if( !IsNotAiming() )
    {
      StopAiming();
      return;
    }

    //let punch cooldown handle isStopped fix
    //agent.isStopped = false;
    agent.SetDestination( PlayerCharacterTarget.position );
  }

  public void RunAway()
  {
    if( IsCowering || PlayerCharacterTarget == null )
    {
      return;
    }

    if( !IsNotAiming() )
    {
      StopAiming();
      return;
    }

    Vector3 directionAway = transform.position - PlayerCharacterTarget.position;
    Vector3 runTarget = transform.position + directionAway.normalized * 10f;

    if( NavMesh.SamplePosition( runTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas ) )
    {
      agent.SetDestination( hit.position );
    }
  }
  
  public void RunAwayHeal(float time)
  {
    runAwayTimer = time;
    isRunToHeal = true;
    
  }
  
  public void CloseRunAwayHeal()
  {
    isRunToHeal = false;
    Heal(100f);
  }

  

  public void Punch()
  {
    if( punchCooldownTimer > 0f || animator == null )
    {
      return;
    }

    if( !IsNotAiming() )
    {
      StopAiming();
      return;
    }

    agent.isStopped = true;
    animator.SetTrigger( animIDPunch );
    punchCooldownTimer = PunchCooldown;
  }

  private void OnPunchHit()
  {
    if( PlayerCharacterTarget == null )
    {
      return;
    }

    float distance = Vector3.Distance( transform.position, PlayerCharacterTarget.position );
    if( distance <= PunchRange )
    {
      var playerHealth = PlayerCharacterTarget.GetComponent<IDamageable>();
      if( playerHealth != null )
      {
        playerHealth.TakeDamage( PunchDamage );
      }
    }

    agent.isStopped = false;
  }

  public void Shoot()
  {
    if( IsCowering || shootCooldownTimer > 0f || PlayerCharacterTarget == null || BulletProjectile == null )
    {
      return;
    }

    float distance = Vector3.Distance( transform.position, PlayerCharacterTarget.position );
    if( distance > ShootRange )
    {
      return;
    }

    agent.isStopped = true;
    agent.SetDestination( transform.position );
    if( !IsAiming() )
    {
      StartAiming();
      return;
    }

    Vector3 aimDirection = ( PlayerCharacterTarget.position - SpawnBulletPosition.position ).normalized;
    transform.forward = new Vector3( aimDirection.x, 0f, aimDirection.z ).normalized;

    GameObject newBullet = Object.Instantiate( BulletProjectile, SpawnBulletPosition.position, Quaternion.LookRotation( aimDirection, Vector3.up ) );
    BulletProjectile bulletProjectile = newBullet.GetComponent<DIGA7009A.BulletProjectile>();
    if( bulletProjectile != null )
    {
      bulletProjectile.SetBulletSpeed( BulletSpeed );
      bulletProjectile.ToggleDestroyOnCollision( true );
    }

    shootCooldownTimer = ShootCooldown;
    agent.isStopped = false;
  }

  public void Patrol()
  {
    if( IsCowering )
    {
      return;
    }

    if( !IsNotAiming() )
    {
      StopAiming();
      return;
    }

    Vector3 patrolTarget = Vector3.zero;

    switch( patrolState )
    {
      case PatrolState.TravellingToA:
        patrolTarget = PatrolPointA.position;
        break;
      case PatrolState.TravellingToB:
        patrolTarget = PatrolPointB.position;
        break;
    }

    agent.SetDestination( patrolTarget );
    patrolStateChecker.Value = HasNPCReachedDestination( patrolTarget );
  }

  public void Wander()
  {
    if( IsCowering )
    {
      return;
    }

    if( !IsNotAiming() )
    {
      StopAiming();
      return;
    }

    if( wanderWaitTimer > 0f )
    {
      wanderWaitTimer -= Time.deltaTime;
      return;
    }

    if( !hasWanderTarget || HasReachedDestination() )
    {
      Vector3 randomDirection = Random.insideUnitSphere * WanderRadius;
      randomDirection += wanderOrigin;
      randomDirection.y = transform.position.y;

      if( NavMesh.SamplePosition( randomDirection, out NavMeshHit hit, WanderRadius, NavMesh.AllAreas ) )
      {
        wanderTarget = hit.position;
        hasWanderTarget = true;
        agent.SetDestination( wanderTarget );
      }

      wanderWaitTimer = WanderWaitTime;
    }
  }

  private void FixedUpdate()
  {
    NPCHealthPoints = Mathf.Clamp( NPCHealthPoints, 0.0f, 100.0f );
    if( punchCooldownTimer > 0f )
    {
      punchCooldownTimer -= Time.fixedDeltaTime;
    }
    //stops punching while moving
    else if(punchCooldownTimer <= 0f && agent.isStopped)
    {
      agent.isStopped = false;
    }


    if( shootCooldownTimer > 0f )
    {
      shootCooldownTimer -= Time.fixedDeltaTime;
    }

    if( healCooldownTimer > 0f )
    {
      healCooldownTimer -= Time.fixedDeltaTime;
    }
    
    
    //heal hack
    if (runAwayTimer > 0f && isRunToHeal)
    {
      runAwayTimer -= Time.fixedDeltaTime;
    }
    else if (runAwayTimer <= 0 && isRunToHeal)
    {
      CloseRunAwayHeal();
    }


    UpdateAnimator();
  }

  private void UpdateAnimator()
  {
    if( animator == null )
    {
      return;
    }

    float agentSpeed = agent.velocity.magnitude;

    float targetBlend = Mathf.Clamp( agentSpeed, 0f, AnimRunSpeed );
    animationBlend = Mathf.Lerp( animationBlend, targetBlend, Time.deltaTime * AnimSmoothTime );
    if( animationBlend < 0.01f )
    {
      animationBlend = 0f;
    }

    animator.SetFloat( animIDSpeed, animationBlend );
    animator.SetFloat( animIDMotionSpeed, 1f );
  }

  private void OnFootstep( AnimationEvent animationEvent )
  {
    if( animationEvent.animatorClipInfo.weight > 0.5f )
    {
      if( FootstepAudioClips.Length > 0 )
      {
        var index = Random.Range( 0, FootstepAudioClips.Length );
        AudioSource.PlayClipAtPoint( FootstepAudioClips[ index ], transform.position, FootstepAudioVolume );
      }
    }
  }

  private void OnLand( AnimationEvent animationEvent )
  {
    if( animationEvent.animatorClipInfo.weight > 0.5f )
    {
      if( LandingAudioClip != null )
      {
        AudioSource.PlayClipAtPoint( LandingAudioClip, transform.position, FootstepAudioVolume );
      }
    }
  }

  private bool HasNPCReachedDestination( Vector3 target )
  {
    if( (agent.gameObject.transform.position - target).sqrMagnitude < PatrolThreshold )
    {
      return true;
    }
    return false;
  }

  private void FlipPatrolState( bool value )
  {
    if( value )
    {
      switch( patrolState )
      {
        case PatrolState.TravellingToA:
          patrolState = PatrolState.TravellingToB;
          break;
        case PatrolState.TravellingToB:
          patrolState = PatrolState.TravellingToA;
          break;
      }
    }
  }

  public void Cower( bool isCowering )
  {
    if( animator == null )
    {
      return;
    }

    IsCowering = isCowering;
    animator.SetBool( animIDCower, isCowering );

    if( isCowering )
    {
      agent.isStopped = true;
      agent.ResetPath();
      agent.velocity = Vector3.zero;
    }
    else
    {
      agent.isStopped = false;
    }
  }

  public void TakeDamage( float damage )
  {
    NPCHealthPoints -= damage;
  }

  public void Heal( float amount )
  {
    NPCHealthPoints = Mathf.Min( NPCHealthPoints + amount, 100f );
  }

  public void GoTo( Vector3 position )
  {
    if( IsCowering )
    {
      return;
    }

    if( !IsNotAiming() )
    {
      StopAiming();
      return;
    }

    agent.isStopped = false;
    agent.SetDestination( position );
  }

  public void AndCheckPickUpDT(HealthPickup targetPickup, float pickupDistance)
  {
    var distance = Vector3.Distance( transform.position,
      targetPickup.GetPickupPositionDT() );

    if (distance <= pickupDistance)
    {
      targetPickup.CollectDT(this);
    }

  }

  public void AndCheckPickUpBT( HealthPickup targetPickup, float pickupDistance )
  {
    var distance = Vector3.Distance( transform.position,
      targetPickup.GetPickupPositionBT() );

    if( distance <= pickupDistance )
    {
      targetPickup.CollectBT( this );
    }

  }

  public bool HasReachedDestination()
  {
    if( !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance )
    {
      return true;
    }
    return false;
  }

  public void HealAlly( NPCController ally )
  {
    if( ally == null || healCooldownTimer > 0f )
    {
      IsHealing = false;
      return;
    }

    float distance = Vector3.Distance( transform.position, ally.transform.position );
    if( distance <= HealRange )
    {
      IsHealing = true;
      animator.SetBool( animIDHeal, IsHealing );
      ally.Heal( 100.0f );
      healCooldownTimer = HealCooldown;
    }
    else
    {
      GoTo( ally.transform.position );
    }
  }

  public Transform GetPlayerTarget()
  {
    return PlayerCharacterTarget;
  }

  public void SetPlayerTarget( Transform target )
  {
    PlayerCharacterTarget = target;
  }

  public void SetPatrolPoints( Transform pointA, Transform pointB )
  {
    PatrolPointA = pointA;
    PatrolPointB = pointB;
  }

  private void StartAiming()
  {
    animator.SetLayerWeight( 1, 1.0f );
  }


  private void StopAiming()
  {
    animator.SetLayerWeight( 1, 0.0f );
  }


  private bool IsAiming()
  {
    if( animator.GetLayerWeight( 1 ) == 1 )
    {
      return true;
    }

    return false;
  }



  private bool IsNotAiming()
  {
    if( animator.GetLayerWeight( 1 ) == 0 )
    {
      return true;
    }

    return false;
  }


  public void Die()
  {
    if( IsDead )
    {
      return;
    }

    IsDead = true;
    agent.isStopped = true;
    agent.ResetPath();
    agent.velocity = Vector3.zero;
    animator.SetLayerWeight( 2, 1.0f );

    Collider col = GetComponent<Collider>();
    if( col != null )
    {
      col.enabled = false;
    }
  }


  public void Revive()
  {
    IsDead = false;
    agent.isStopped = false;
    animator.SetLayerWeight( 2, 0.0f );
  }

  public void Idle()
  {
    if( animator == null )
    {
      return;
    }

    agent.isStopped = true;
    agent.ResetPath();
    agent.velocity = Vector3.zero;
    animator.SetFloat( animIDSpeed, 0f );
    animator.SetFloat( animIDMotionSpeed, 0f );
    animator.SetLayerWeight( 3, 1.0f );
  }
}
