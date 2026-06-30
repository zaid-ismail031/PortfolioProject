using UnityEngine;

namespace Models.BehaviourTree.Actions
{
  public class GoToHealthPickupAction : Task
  {
    private readonly NPCController npc;
    private readonly Transform npcTransform;
    private readonly float searchRadius;
    private readonly float pickupDistance;
    private HealthPickup targetPickup;

    public GoToHealthPickupAction( NPCController npc, Transform npcTransform, float searchRadius, float pickupDistance = 1.5f )
    {
      this.npc = npc;
      this.npcTransform = npcTransform;
      this.searchRadius = searchRadius;
      this.pickupDistance = pickupDistance;
    }

    public override Status Run()
    {
      if( targetPickup == null || !targetPickup.GetIsActiveBT() )
      {
        targetPickup = FindNearestPickup();
        if( targetPickup == null )
        {
          return Status.Failure;
        }
      }

      Vector3 pickupPosition = targetPickup.GetPickupPositionBT();
      float distance = Vector3.Distance( npcTransform.position, pickupPosition );

      if( distance <= pickupDistance )
      {
        targetPickup.CollectBT( npc );
        targetPickup = null;
        return Status.Success;
      }

      npc.GoTo( pickupPosition );
      return Status.Running;
    }

    private HealthPickup FindNearestPickup()
    {
      HealthPickup[] pickups = Object.FindObjectsByType<HealthPickup>( FindObjectsSortMode.None );
      HealthPickup nearest = null;
      float nearestDistance = float.MaxValue;

      foreach( HealthPickup pickup in pickups )
      {
        if( !pickup.GetIsActiveBT() )
        {
          continue;
        }

        float distance = Vector3.Distance( npcTransform.position, pickup.GetPickupPositionBT() );
        if( distance < nearestDistance && distance <= searchRadius )
        {
          nearest = pickup;
          nearestDistance = distance;
        }
      }

      return nearest;
    }
  }
}
