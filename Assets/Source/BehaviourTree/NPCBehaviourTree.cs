using System.Collections.Generic;
using Models.BehaviourTree;
using Models.BehaviourTree.Actions;
using Models.BehaviourTree.Conditions;
using UnityEngine;

public class NPCBehaviourTree : MonoBehaviour, IBehaviourTree
{
  [SerializeField] private NPCController npcController;
  [SerializeField] private Transform playerTarget;

  [Header( "Ranges" )]
  [SerializeField] private float detectionRange = 10f;
  [SerializeField] private float shootRange = 10f;
  [SerializeField] private float punchRange = 2f;

  [Header( "Health" )]
  [SerializeField] private float lowHealthThreshold = 30f;
  [SerializeField] private float pickupSearchRadius = 50f;

  [Header( "Decorators" )]
  [SerializeField] private float searchTimeout = 8f;
  [SerializeField] private float cowerDuration = 3f;
  [SerializeField] private int searchRetries = 3;
  [SerializeField] private float retryDelay = 1f;
  [SerializeField] private float healSearchCooldown = 5f;

  public Task RootTask { get; private set; }

  private void Start()
  {
    if( npcController == null )
    {
      npcController = GetComponent<NPCController>();
    }

    

    Task searchOrCower = new Selector( new List<Task>
    {
      new Timeout(
        new GoToHealthPickupAction( npcController, transform, pickupSearchRadius ),
        searchTimeout
      ),
      new CowerAction( npcController, cowerDuration )
    } );

    Task healSearch = new Cooldown(
      new Retry( searchOrCower, searchRetries, retryDelay ),
      healSearchCooldown
    );

    Task healBranch = new Sequence( new List<Task>
    {
      new IsHealthLowCondition( npcController, lowHealthThreshold ),
      new RunUntilSuccess( healSearch )
    } );

    Task punchBranch = new Sequence( new List<Task>
    {
      new IsPlayerNearCondition( transform, playerTarget, punchRange ),
      new PunchAction( npcController )
    } );

    Task shootBranch = new Sequence( new List<Task>
    {
      new IsPlayerNearCondition( transform, playerTarget, shootRange ),
      new Inverter( new IsPlayerNearCondition( transform, playerTarget, punchRange ) ),
      new ShootAction( npcController )
    } );

    Task chaseBranch = new Sequence( new List<Task>
    {
      new IsPlayerNearCondition( transform, playerTarget, detectionRange ),
      new Inverter( new IsPlayerNearCondition( transform, playerTarget, shootRange ) ),
      new ChaseAction( npcController )
    } );

    Task patrolFallback = new PatrolAction( npcController );

    RootTask = new Selector( new List<Task>
    {
      healBranch,
      punchBranch,
      shootBranch,
      chaseBranch,
      patrolFallback
    } );
  }

  public void Tick()
  {
    if( RootTask != null )
    {
      RootTask.Run();
    }
  }

  private void Update()
  {
    Tick();
  }
}
