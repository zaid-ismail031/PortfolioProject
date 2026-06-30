using UnityEngine;

namespace Models.BehaviourTree.Actions
{
  public class VantageShootAction : Task
  {
    private readonly NPCController npc;
    private readonly SquadBlackboard blackboard;
    private readonly float arrivalThreshold;

    public VantageShootAction( NPCController npc, SquadBlackboard blackboard, float arrivalThreshold = 2f )
    {
      this.npc = npc;
      this.blackboard = blackboard;
      this.arrivalThreshold = arrivalThreshold;
    }

    public override Status Run()
    {
      Vector3 vantagePos = blackboard.VantagePosition;
      float distanceToVantage = Vector3.Distance( npc.transform.position, vantagePos );

      if( distanceToVantage > arrivalThreshold )
      {
        npc.GoTo( vantagePos );
      }
      else
      {
        npc.Shoot();
      }

      return Status.Running;
    }
  }
}
