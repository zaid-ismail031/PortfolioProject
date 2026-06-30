using UnityEngine;

namespace Models.BehaviourTree.Conditions
{
  public class IsPlayerNearCondition : Task
  {
    private readonly Transform npcTransform;
    private readonly Transform playerTransform;
    private readonly float detectionRange;

    public IsPlayerNearCondition( Transform npcTransform, Transform playerTransform, float detectionRange )
    {
      this.npcTransform = npcTransform;
      this.playerTransform = playerTransform;
      this.detectionRange = detectionRange;
    }

    public override Status Run()
    {
      if( playerTransform == null )
      {
        return Status.Failure;
      }

      float distance = Vector3.Distance( npcTransform.position, playerTransform.position );
      return distance <= detectionRange ? Status.Success : Status.Failure;
    }
  }
}
