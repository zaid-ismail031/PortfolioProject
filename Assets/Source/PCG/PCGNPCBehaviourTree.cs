using System.Collections.Generic;
using Models.BehaviourTree;
using Models.BehaviourTree.Actions;
using Models.BehaviourTree.Conditions;
using UnityEngine;

public enum PCGNPCRole
{
  Attacker,
  Passive,
  Coward,
  QuestGiver
}

public class PCGNPCBehaviourTree : MonoBehaviour, IBehaviourTree
{
  public Task RootTask { get; private set; }
  public PCGNPCRole Role { get; private set; }

  private NPCController npcController;
  private Transform playerTarget;
  private float detectionRange;
  private float punchRange;
  private float shootRange;

  public void Initialize( NPCController npc, Transform player, PCGNPCRole assignedRole, float detection, float punch, float shoot )
  {
    npcController = npc;
    playerTarget = player;
    Role = assignedRole;
    detectionRange = detection;
    punchRange = punch;
    shootRange = shoot;

    BuildTree();
  }

  private void BuildTree()
  {
    switch( Role )
    {
      case PCGNPCRole.Attacker:
        BuildAttackerTree();
        break;
      case PCGNPCRole.Passive:
        BuildPassiveTree();
        break;
      case PCGNPCRole.Coward:
        BuildCowardTree();
        break;
      case PCGNPCRole.QuestGiver:
        BuildQuestGiverTree();
        break;
    }
  }

  private Task BuildDeathBranch()
  {
    return new Sequence( new List<Task>
    {
      new IsDeadCondition( npcController ),
      new DieAction( npcController )
    } );
  }

  private void BuildAttackerTree()
  {
    Task deathBranch = BuildDeathBranch();

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
      new ChaseAction( npcController )
    } );

    Task patrolFallback = new WanderAction( npcController );

    RootTask = new Selector( new List<Task>
    {
      deathBranch,
      punchBranch,
      shootBranch,
      chaseBranch,
      patrolFallback
    } );
  }

  private void BuildPassiveTree()
  {
    Task deathBranch = BuildDeathBranch();

    Task cowerBranch = new Sequence( new List<Task>
    {
      new IsHealthLowCondition( npcController, 50f ),
      new CowerAction( npcController, 3f )
    } );

    Task patrolFallback = new WanderAction( npcController );

    RootTask = new Selector( new List<Task>
    {
      deathBranch,
      cowerBranch,
      patrolFallback
    } );
  }

  private void BuildCowardTree()
  {
    Task deathBranch = BuildDeathBranch();

    Task fleeBranch = new Sequence( new List<Task>
    {
      new IsPlayerNearCondition( transform, playerTarget, detectionRange ),
      new RunAwayAction( npcController )
    } );

    Task cowerBranch = new Sequence( new List<Task>
    {
      new IsHealthLowCondition( npcController, 30f ),
      new CowerAction( npcController, 5f )
    } );

    Task patrolFallback = new WanderAction( npcController );

    RootTask = new Selector( new List<Task>
    {
      deathBranch,
      fleeBranch,
      cowerBranch,
      patrolFallback
    } );
  }

  private void BuildQuestGiverTree()
  {
    RootTask = new IdleAction( npcController );
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
