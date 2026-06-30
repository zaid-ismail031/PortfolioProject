using UnityEngine;
using UnityEngine.AI;

public class HealthPickup : MonoBehaviour
{
  [SerializeField]
  private Transform HealthPickUpObjectBT;
  [SerializeField]
  private Transform HealthPickUpObjectDT;
  [SerializeField] 
  private float HealAmount = 30f;
  [SerializeField] 
  private float RespawnTime = 15f;
  [SerializeField] 
  private float SpawnRadius = 5f;

  private MeshRenderer meshRendererBT;
  private Collider pickupColliderBT;

  private MeshRenderer meshRendererDT;
  private Collider pickUpColliderDT;
  private bool isActiveBT = true;
  private bool isActiveDT = true;

  private void Start()
  {
    meshRendererBT = HealthPickUpObjectBT.GetComponent<MeshRenderer>();
    pickupColliderBT = HealthPickUpObjectBT.GetComponent<Collider>();

    meshRendererDT = HealthPickUpObjectDT.GetComponent<MeshRenderer>();
    pickUpColliderDT = HealthPickUpObjectDT.GetComponent<Collider>();
    MoveToRandomNavMeshPosition( HealthPickUpObjectBT );
    MoveToRandomNavMeshPosition( HealthPickUpObjectDT );
  }

  public void CollectBT( NPCController npc )
  {
    if( !isActiveBT )
    {
      return;
    }

    npc.Heal( HealAmount );
    DeactivateBT();
  }

  public void CollectDT( NPCController npc )
  {
    if( !isActiveDT )
    {
      return;
    }

    npc.Heal( HealAmount );
    DeactivateDT();
  }

  private void Deactivate( MeshRenderer meshRenderer, Collider pickupCollider, ref bool isActive )
  {
    isActive = false;
    if( meshRenderer != null ) meshRenderer.enabled = false;
    if( pickupCollider != null ) pickupCollider.enabled = false;
  }

  private void DeactivateBT()
  {
    Deactivate( meshRendererBT, pickupColliderBT, ref isActiveBT );
    Invoke( nameof( ReactivateBT ), RespawnTime );
  }

  private void DeactivateDT()
  {
    Deactivate( meshRendererDT, pickUpColliderDT, ref isActiveDT );
    Invoke( nameof( ReactivateDT ), RespawnTime );
  }

  private void ReactivateBT()
  {
    MoveToRandomNavMeshPosition( HealthPickUpObjectBT );
    isActiveBT = true;
    if( meshRendererBT != null ) meshRendererBT.enabled = true;
    if( pickupColliderBT != null ) pickupColliderBT.enabled = true;
  }

  private void ReactivateDT()
  {
    MoveToRandomNavMeshPosition( HealthPickUpObjectDT );
    isActiveDT = true;
    if( meshRendererDT != null ) meshRendererDT.enabled = true;
    if( pickUpColliderDT != null ) pickUpColliderDT.enabled = true;
  }

  private void MoveToRandomNavMeshPosition( Transform HealthPickUpObject )
  {
    for( int i = 0; i < 30; i++ )
    {
      Vector3 randomPoint = HealthPickUpObject.position + Random.insideUnitSphere * SpawnRadius;
      randomPoint.y = transform.position.y;

      if( NavMesh.SamplePosition( randomPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas ) )
      {
        HealthPickUpObject.position = hit.position + Vector3.up * 0.5f;
        return;
      }
    }
  }

  public float GetHealAmount()
  {
    return HealAmount;
  }

  public bool GetIsActiveBT()
  {
    return isActiveBT;
  }


  public bool GetIsActiveDT()
  {
    return isActiveDT;
  }

  public Vector3 GetPickupPositionBT()
  {
    return HealthPickUpObjectBT.position;
  }
  

  public Vector3 GetPickupPositionDT()
  {
    return HealthPickUpObjectDT.position;
  }

  public Transform GetPickupTransformDT()
  {
    return HealthPickUpObjectDT;
  }
}
