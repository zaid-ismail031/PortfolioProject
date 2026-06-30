using UnityEngine;

namespace Models.BehaviourTree.Actions
{
  public class RushAndPunchAction : Task
  {
    private readonly NPCController npc;
    private readonly SquadBlackboard blackboard;
    private readonly float punchRange;

    public RushAndPunchAction( NPCController npc, SquadBlackboard blackboard, float punchRange = 2f )
    {
      this.npc = npc;
      this.blackboard = blackboard;
      this.punchRange = punchRange;
    }

    public override Status Run()
    {
      Vector3 targetPos = blackboard.PlayerLastKnownPosition;
      float distance = Vector3.Distance( npc.transform.position, targetPos );

      if( distance <= punchRange )
      {
        npc.Punch();
      }
      else
      {
        npc.GoTo( targetPos );
      }

      return Status.Running;
    }
  }
}
