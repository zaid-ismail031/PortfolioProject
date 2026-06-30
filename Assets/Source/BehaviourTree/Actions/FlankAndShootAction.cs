using UnityEngine;

namespace Models.BehaviourTree.Actions
{
  public class FlankAndShootAction : Task
  {
    private readonly NPCController npc;
    private readonly SquadBlackboard blackboard;
    private readonly float shootDistance;

    public FlankAndShootAction( NPCController npc, SquadBlackboard blackboard, float shootDistance = 3f )
    {
      this.npc = npc;
      this.blackboard = blackboard;
      this.shootDistance = shootDistance;
    }

    public override Status Run()
    {
      Vector3 flankPos = blackboard.FlankPosition;
      float distanceToFlank = Vector3.Distance( npc.transform.position, flankPos );

      if( distanceToFlank > 1f )
      {
        npc.GoTo( flankPos );
      }
      else
      {
        npc.Shoot();
      }

      return Status.Running;
    }
  }
}
