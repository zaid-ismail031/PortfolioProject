using System.Collections.Generic;
using Models.BehaviourTree;
using Models.BehaviourTree.Actions;
using Models.BehaviourTree.Conditions;
using UnityEngine;

public class SquadMemberBehaviourTree : MonoBehaviour, IBehaviourTree
{
  [SerializeField] private NPCController npcController;
  [SerializeField] private SquadController squadController;

  [Header( "Ranges" )]
  [SerializeField] private float punchRange = 2f;
  [SerializeField] private float flankArrivalDistance = 3f;
  [SerializeField] private float vantageArrivalDistance = 2f;
  [SerializeField] private float retreatDistance = 15f;

  public Task RootTask { get; private set; }

  private void Start()
  {
    if( npcController == null )
    {
      npcController = GetComponent<NPCController>();
    }

    SquadBlackboard blackboard = squadController.Blackboard;

    Task deathBranch = new Sequence( new List<Task>
    {
      new IsDeadCondition( npcController ),
      new DieAction( npcController )
    } );

    Task resurrectBranch = new Sequence( new List<Task>
    {
      new HasDiedAndShouldResurrectCondition( npcController ),
      new ResurrectAction( npcController )
    } );

    Task retreatBranch = new Sequence( new List<Task>
    {
      new IsSquadRetreatingCondition( blackboard ),
      new Selector( new List<Task>
      {
        new Sequence( new List<Task>
        {
          new IsRoleCondition( npcController, blackboard, SquadRole.Healer ),
          new IsAllyHurtCondition( blackboard ),
          new HealAllyAction( npcController, blackboard )
        } ),
        new RetreatAction( npcController, blackboard, retreatDistance )
      } )
    } );

    Task rusherBranch = new Sequence( new List<Task>
    {
      new IsRoleCondition( npcController, blackboard, SquadRole.Rusher ),
      new RushAndPunchAction( npcController, blackboard, punchRange )
    } );

    Task flankerBranch = new Sequence( new List<Task>
    {
      new IsRoleCondition( npcController, blackboard, SquadRole.Flanker ),
      new FlankAndShootAction( npcController, blackboard, flankArrivalDistance )
    } );

    Task vantageBranch = new Sequence( new List<Task>
    {
      new IsRoleCondition( npcController, blackboard, SquadRole.Vantage ),
      new VantageShootAction( npcController, blackboard, vantageArrivalDistance )
    } );

    Task healerBranch = new Sequence( new List<Task>
    {
      new IsRoleCondition( npcController, blackboard, SquadRole.Healer ),
      new Selector( new List<Task>
      {
        new Sequence( new List<Task>
        {
          new IsAllyHurtCondition( blackboard ),
          new HealAllyAction( npcController, blackboard )
        } ),
        new MoveToHealerHidePositionAction( npcController, blackboard )
      } )
    } );

    Task engagedBranch = new Sequence( new List<Task>
    {
      new IsSquadEngagedCondition( blackboard ),
      new Selector( new List<Task>
      {
        rusherBranch,
        flankerBranch,
        vantageBranch,
        healerBranch,
        new MoveToLastKnownPositionAction( npcController, blackboard )
      } )
    } );

    Task searchBranch = new Sequence( new List<Task>
    {
      new Inverter( new IsPlayerVisibleCondition( blackboard ) ),
      new MoveToLastKnownPositionAction( npcController, blackboard )
    } );

    RootTask = new Selector( new List<Task>
    {
      deathBranch,
      resurrectBranch,
      retreatBranch,
      engagedBranch,
      searchBranch
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
