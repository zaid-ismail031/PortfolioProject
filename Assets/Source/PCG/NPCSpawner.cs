using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
  [Header( "References" )]
  [SerializeField]
  private DungeonGenerator DungeonGen;
  [SerializeField]
  private NarrativeRules Rules;
  [SerializeField]
  private Transform PlayerTarget;

  [Header( "NPC Prefab" )]
  [SerializeField]
  private GameObject NPCPrefab;

  [Header( "Spawn Settings" )]
  [SerializeField]
  [Range( 1, 5 )]
  private int NPCsPerRoom = 1;
  [SerializeField]
  private bool SkipStartRoom = true;
  [SerializeField]
  private bool SkipGoalRoom;

  [Header( "Combat Ranges" )]
  [SerializeField]
  private float DetectionRange = 15f;
  [SerializeField]
  private float PunchRange = 2f;
  [SerializeField]
  private float ShootRange = 10f;

  [Header( "Patrol" )]
  [SerializeField]
  private float PatrolRadius = 5f;

  [Header( "Generation" )]
  [SerializeField]
  private int Seed;
  [SerializeField]
  private bool RandomizeSeed = true;

  private List<GameObject> spawnedNPCs;
  private Transform npcParent;
  private System.Random random;

  public void SpawnAll()
  {
    ClearSpawnedNPCs();

    if( DungeonGen == null || NPCPrefab == null )
    {
      return;
    }

    if( RandomizeSeed )
    {
      Seed = Random.Range( 0, 100000 );
    }

    random = new System.Random( Seed );
    spawnedNPCs = new List<GameObject>();
    npcParent = new GameObject( "SpawnedNPCs" ).transform;

    List<RectInt> rooms = DungeonGen.GetRooms();
    if( rooms == null || rooms.Count == 0 )
    {
      return;
    }

    PCGNPCRole[] combatRoles = new PCGNPCRole[]
    {
      PCGNPCRole.Attacker,
      PCGNPCRole.Passive,
      PCGNPCRole.Coward
    };

    List<int> eligibleRooms = new List<int>();

    for( int roomIndex = 0; roomIndex < rooms.Count; roomIndex++ )
    {
      if( SkipStartRoom && roomIndex == DungeonGen.GetStartRoomIndex() )
      {
        continue;
      }

      if( SkipGoalRoom && roomIndex == DungeonGen.GetGoalRoomIndex() )
      {
        continue;
      }

      eligibleRooms.Add( roomIndex );
    }

    if( rooms.Count > 0 )
    {
      SpawnNPC( rooms[ 0 ], 0, PCGNPCRole.QuestGiver );
    }

    foreach( int roomIndex in eligibleRooms )
    {
      RectInt room = rooms[ roomIndex ];
      int difficulty = DungeonGen.GetRoomDifficulty( roomIndex );
      int spawnCount = NPCsPerRoom + difficulty;

      for( int i = 0; i < spawnCount; i++ )
      {
        PCGNPCRole role = combatRoles[ random.Next( combatRoles.Length ) ];
        SpawnNPC( room, roomIndex, role );
      }
    }
  }

  private void SpawnNPC( RectInt room, int roomIndex, PCGNPCRole role )
  {
    float cellSize = DungeonGen.GetCellSize();
    float spawnX = ( room.x + random.Next( 1, room.width - 1 ) ) * cellSize;
    float spawnZ = ( room.y + random.Next( 1, room.height - 1 ) ) * cellSize;
    float spawnYOffset = DungeonGen.GetDungeonParent().transform.position.y;
    float spawnXOffset = DungeonGen.GetDungeonParent().transform.position.x;
    float spawnZOffset = DungeonGen.GetDungeonParent().transform.position.z;
    Vector3 spawnPosition = new Vector3( spawnX + spawnXOffset, spawnYOffset + 0.5f, spawnZ + spawnZOffset );

    GameObject npcObj = Instantiate( NPCPrefab, spawnPosition, Quaternion.identity, npcParent );
    npcObj.name = "NPC_" + role + "_Room" + roomIndex;
    npcObj.tag = "Enemy";

    NPCController npcController = npcObj.GetComponent<NPCController>();
    if( npcController == null )
    {
      return;
    }

    NavMeshAgent agent = npcObj.GetComponent<NavMeshAgent>();
    if( agent != null )
    {
      agent.Warp( spawnPosition );
    }

    npcController.SetPlayerTarget( PlayerTarget );
    SetupPatrolPoints( npcObj, npcController, spawnPosition );

    PCGNPCBehaviourTree behaviourTree = npcObj.AddComponent<PCGNPCBehaviourTree>();
    behaviourTree.Initialize( npcController, PlayerTarget, role, DetectionRange, PunchRange, ShootRange );

    string npcName = Rules.GetRandomTitle( random ) + " " + Rules.GetRandomFirstName( random );
    string backstory = GenerateBackstory( npcName, role );
    NPCBackstoryLabel label = npcObj.AddComponent<NPCBackstoryLabel>();
    Color roleColor = label.GetRoleColor( role );
    label.Initialize( backstory, npcName, role.ToString(), roleColor );

    npcObj.GetComponent<NPCController>().enabled = true;
    npcObj.GetComponent<NPCHealthBar>().enabled = true;

    spawnedNPCs.Add( npcObj );
  }

  private void SetupPatrolPoints( GameObject npcObj, NPCController npcController, Vector3 center )
  {
    GameObject pointA = new GameObject( "PatrolPointA" );
    pointA.transform.SetParent( npcObj.transform );
    pointA.transform.position = center + new Vector3( PatrolRadius, 0f, 0f );

    GameObject pointB = new GameObject( "PatrolPointB" );
    pointB.transform.SetParent( npcObj.transform );
    pointB.transform.position = center + new Vector3( -PatrolRadius, 0f, 0f );

    npcController.SetPatrolPoints( pointA.transform, pointB.transform );
  }

  private string GenerateBackstory( string npcName, PCGNPCRole role )
  {
    if( Rules == null )
    {
      return role.ToString();
    }

    string template = Rules.GetRandomBackstoryTemplate( random );

    string result = template;
    result = result.Replace( "{name}", npcName );
    result = result.Replace( "{role}", role.ToString() );
    result = result.Replace( "{enemy}", Rules.GetRandomEnemyName( random ) );
    result = result.Replace( "{item}", Rules.GetRandomItemName( random ) );
    result = result.Replace( "{location}", Rules.GetRandomLocationName( random ) );

    return result;
  }

  public void ClearSpawnedNPCs()
  {
    if( npcParent != null )
    {
      Destroy( npcParent.gameObject );
    }

    if( spawnedNPCs != null )
    {
      spawnedNPCs.Clear();
    }
  }

  public List<GameObject> GetSpawnedNPCs()
  {
    return spawnedNPCs;
  }

  public void SetRules( NarrativeRules rules )
  {
    Rules = rules;
  }
}
