using UnityEngine;
using UnityEngine.AI;

namespace Models.BehaviourTree.Actions
{
  public class RetreatAction : Task
  {
    private readonly NPCController npc;
    private readonly SquadBlackboard blackboard;
    private readonly float retreatDistance;

    public RetreatAction( NPCController npc, SquadBlackboard blackboard, float retreatDistance = 15f )
    {
      this.npc = npc;
      this.blackboard = blackboard;
      this.retreatDistance = retreatDistance;
    }

    public override Status Run()
    {
      Vector3 directionAway = npc.transform.position - blackboard.PlayerLastKnownPosition;
      Vector3 retreatTarget = npc.transform.position + directionAway.normalized * retreatDistance;

      if( NavMesh.SamplePosition( retreatTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas ) )
      {
        npc.GoTo( hit.position );
      }

      return Status.Running;
    }
  }
}
