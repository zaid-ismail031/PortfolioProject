using UnityEngine;

namespace Models.BehaviourTree
{
  public class MoveToHealerHidePositionAction : Task
  {
    private readonly NPCController npc;
    private readonly SquadBlackboard blackboard;
    private readonly float arrivalThreshold;

    public MoveToHealerHidePositionAction( NPCController npc, SquadBlackboard blackboard, float arrivalThreshold = 2f )
    {
      this.npc = npc;
      this.blackboard = blackboard;
      this.arrivalThreshold = arrivalThreshold;
    }

    public override Status Run()
    {
      Vector3 lastKnown = blackboard.HealerHidePosition;
      float distance = Vector3.Distance( npc.transform.position, lastKnown );

      if( distance <= arrivalThreshold )
      {
        return Status.Success;
      }

      npc.GoTo( lastKnown );
      return Status.Running;
    }
  }
}

