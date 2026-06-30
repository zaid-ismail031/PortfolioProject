using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
  [Header( "Generators" )]
  [SerializeField]
  private TerrainGenerator TerrainGen;
  [SerializeField]
  private DungeonGenerator DungeonGen;
  [SerializeField]
  private NarrativeGenerator NarrativeGen;
  [SerializeField]
  private NPCSpawner Spawner;
  [SerializeField]
  private QuestHUD QuestHud;
  [SerializeField]
  private PyramidGenerator PyramidGen;

  [Header( "Theme" )]
  [SerializeField]
  private LevelTheme[] Themes;
  [SerializeField]
  private Light DirectionalLight;

  private LevelTheme ActiveTheme;

  [Header( "Room Objects" )]
  [SerializeField]
  private RoomObjectCollection StartRoomObjects;
  [SerializeField]
  private RoomObjectCollection GoalRoomObjects;

  [Header( "Dungeon Placement" )]
  [SerializeField]
  private Vector3 DungeonOffset = new Vector3( 50f, 0f, 50f );

  [Header( "Player" )]
  [SerializeField]
  private GameObject PlayerArmature;

  [Header( "Generation" )]
  [SerializeField]
  private bool GenerateOnStart = true;
  [SerializeField]
  private int Seed;
  [SerializeField]
  private bool RandomizeSeed = true;

  private NavMeshSurface navMeshSurface;
  private Transform notesParent;
  private Transform roomObjectsParent;
  private Transform propsParent;

  private void Start()
  {
    if( RandomizeSeed )
    {
      Seed = Random.Range( 0, 100000 );
    }

    if( GenerateOnStart )
    {
      GenerateLevel();
    }
  }

  [ContextMenu( "Generate Level" )]
  public void GenerateLevel()
  {
    if( Themes != null && Themes.Length > 0 )
    {
      ActiveTheme = Themes[ Random.Range( 0, Themes.Length ) ];
      ApplyTheme();

      NarrativeRules rules = ActiveTheme.GetNarrativeRules();
      if( rules != null )
      {
        if( NarrativeGen != null )
        {
          NarrativeGen.SetRules( rules );
        }

        if( Spawner != null )
        {
          Spawner.SetRules( rules );
        }
      }
    }

    if( TerrainGen != null )
    {
      TerrainGen.SetSeed( Seed );
      TerrainGen.Generate();
    }

    if( DungeonGen != null )
    {
      DungeonGen.Generate();
      DungeonGen.GetDungeonParent().transform.position = DungeonOffset;

      if( PyramidGen != null )
      {
        PyramidGen.Generate();
      }

      SpawnProps();
      SpawnRoomObjects();
      BakeNavMesh();
    }

    if( NarrativeGen != null )
    {
      NarrativeGen.Generate();
    }

    if( NarrativeGen != null && DungeonGen != null )
    {
      SpawnEnvironmentalNotes();
    }

    if( Spawner != null )
    {
      Spawner.SpawnAll();
    }

    if( NarrativeGen != null && Spawner != null )
    {
      NarrativeGen.GenerateQuestFromSpawnedNPCs( Spawner.GetSpawnedNPCs() );
    }

    if( QuestHud != null && NarrativeGen != null )
    {
      QuestHud.BuildHUD();
    }

    Transform dungeonParent = DungeonGen.GetDungeonParent();
    Vector3 floorPosition = Vector3.zero;

    foreach( Transform dungeonChild in dungeonParent )
    {
      if( dungeonChild.name.ToLower().Contains( "floor" ) )
      {
        floorPosition = dungeonChild.position;
        break;
      }
    }

    //PlayerArmature.transform.position = new Vector3( floorPosition.x, floorPosition.y + 3.0f, floorPosition.z );
  }

  private void ApplyTheme()
  {
    //if( TerrainGen != null )
    //{
    //  TerrainGen.SetTerrainLayers( ActiveTheme.GetTerrainLayers() );
    //  TerrainGen.SetNoiseParameters(
    //    ActiveTheme.GetTerrainFrequency(),
    //    ActiveTheme.GetTerrainAmplitude(),
    //    ActiveTheme.GetTerrainOctaves()
    //  );
    //  TerrainGen.SetTerrainHeight( ActiveTheme.GetTerrainHeight() );
    //}

    if( DungeonGen != null )
    {
      DungeonGen.SetPrefabs(
        ActiveTheme.GetFloorPrefab(),
        ActiveTheme.GetWallPrefab(),
        ActiveTheme.GetCorridorPrefab()
      );
      DungeonGen.SetMarkerPrefabs(
        ActiveTheme.GetStartMarkerPrefab(),
        ActiveTheme.GetGoalMarkerPrefab()
      );
    }

    ApplyLighting();
    ApplyFog();
    ApplySkybox();
  }

  private void ApplyLighting()
  {
    RenderSettings.ambientLight = ActiveTheme.GetAmbientColor();

    if( DirectionalLight != null )
    {
      DirectionalLight.color = ActiveTheme.GetDirectionalLightColor();
      DirectionalLight.intensity = ActiveTheme.GetDirectionalLightIntensity();
    }
  }

  private void ApplyFog()
  {
    RenderSettings.fog = ActiveTheme.GetEnableFog();
    RenderSettings.fogColor = ActiveTheme.GetFogColor();
    RenderSettings.fogDensity = ActiveTheme.GetFogDensity();
  }

  private void ApplySkybox()
  {
    Material skybox = ActiveTheme.GetSkyboxMaterial();
    if( skybox != null )
    {
      RenderSettings.skybox = skybox;
    }
  }

  private void SpawnProps()
  {
    if( ActiveTheme == null )
    {
      return;
    }

    ClearProps();

    GameObject[] propPrefabs = ActiveTheme.GetPropPrefabs();
    if( propPrefabs == null || propPrefabs.Length == 0 )
    {
      return;
    }

    propsParent = new GameObject( "DungeonProps" ).transform;

    List<RectInt> rooms = DungeonGen.GetRooms();
    DungeonGenerator.CellType[,] grid = DungeonGen.GetGrid();
    int propsPerRoom = ActiveTheme.GetPropsPerRoom();
    float propYOffset = ActiveTheme.GetPropYOffset();
    Vector3 dungeonPos = DungeonGen.GetDungeonParent().transform.position;

    System.Random propRandom = new System.Random( DungeonGen.GetSeed() );

    for( int roomIndex = 0; roomIndex < rooms.Count; roomIndex++ )
    {
      RectInt room = rooms[ roomIndex ];

      for( int i = 0; i < propsPerRoom; i++ )
      {
        int attempts = 0;
        while( attempts < 10 )
        {
          int x = propRandom.Next( room.x, room.x + room.width );
          int y = propRandom.Next( room.y, room.y + room.height );

          if( grid[ x, y ] == DungeonGenerator.CellType.Floor )
          {
            GameObject prefab = propPrefabs[ propRandom.Next( propPrefabs.Length ) ];
            Vector3 position = new Vector3(
              dungeonPos.x + x * DungeonGen.GetCellSize(),
              dungeonPos.y + propYOffset,
              dungeonPos.z + y * DungeonGen.GetCellSize()
            );
            float rotation = (float)propRandom.NextDouble() * 360f;
            Instantiate( prefab, position, Quaternion.Euler( 0f, rotation, 0f ), propsParent );
            break;
          }

          attempts++;
        }
      }
    }
  }

  private void ClearProps()
  {
    if( propsParent != null )
    {
      Destroy( propsParent.gameObject );
    }
  }

  private void SpawnRoomObjects()
  {
    if( roomObjectsParent != null )
    {
      Destroy( roomObjectsParent.gameObject );
    }

    roomObjectsParent = new GameObject( "RoomObjects" ).transform;

    List<RectInt> rooms = DungeonGen.GetRooms();
    float cellSize = DungeonGen.GetCellSize();
    Vector3 dungeonPos = DungeonGen.GetDungeonParent().transform.position;

    if( StartRoomObjects != null )
    {
      RectInt startRoom = rooms[ DungeonGen.GetStartRoomIndex() ];
      Vector3 startCenter = new Vector3(
        dungeonPos.x + ( startRoom.x + startRoom.width * 0.5f ) * cellSize,
        dungeonPos.y,
        dungeonPos.z + ( startRoom.y + startRoom.height * 0.5f ) * cellSize
      );
      SpawnCollection( StartRoomObjects, startCenter );
    }

    if( GoalRoomObjects != null )
    {
      RectInt goalRoom = rooms[ DungeonGen.GetGoalRoomIndex() ];
      Vector3 goalCenter = new Vector3(
        dungeonPos.x + ( goalRoom.x + goalRoom.width * 0.5f ) * cellSize,
        dungeonPos.y,
        dungeonPos.z + ( goalRoom.y + goalRoom.height * 0.5f ) * cellSize
      );
      SpawnCollection( GoalRoomObjects, goalCenter );
    }
  }

  private void SpawnCollection( RoomObjectCollection collection, Vector3 roomCenter )
  {
    RoomObjectCollection.RoomObject[] objects = collection.GetObjects();
    if( objects == null )
    {
      return;
    }

    foreach( RoomObjectCollection.RoomObject roomObj in objects )
    {
      if( roomObj.GetPrefab() == null )
      {
        continue;
      }

      Vector3 position = roomCenter + roomObj.GetPositionOffset();
      Quaternion rotation = Quaternion.Euler( roomObj.GetRotationOffset() );
      GameObject spawned = Instantiate( roomObj.GetPrefab(), position, rotation, roomObjectsParent );
      spawned.transform.localScale = roomObj.GetScale();
    }
  }

  private void SpawnEnvironmentalNotes()
  {
    if( notesParent != null )
    {
      Destroy( notesParent.gameObject );
    }

    notesParent = new GameObject( "EnvironmentalNotes" ).transform;

    List<EnvironmentalNote> notes = NarrativeGen.GetEnvironmentalNotes();
    if( notes == null )
    {
      return;
    }

    List<RectInt> rooms = DungeonGen.GetRooms();
    DungeonGenerator.CellType[,] grid = DungeonGen.GetGrid();
    float cellSize = DungeonGen.GetCellSize();
    Vector3 dungeonPos = DungeonGen.GetDungeonParent().transform.position;

    foreach( EnvironmentalNote note in notes )
    {
      if( note.RoomIndex < 0 || note.RoomIndex >= rooms.Count )
      {
        continue;
      }

      RectInt room = rooms[ note.RoomIndex ];
      Vector3 wallPosition;
      Quaternion wallRotation;

      if( FindWallInRoom( room, grid, cellSize, out wallPosition, out wallRotation ) )
      {
        wallPosition += dungeonPos;

        GameObject noteObj = new GameObject( "Note_Room" + note.RoomIndex );
        noteObj.transform.SetParent( notesParent );

        EnvironmentalNoteDisplay display = noteObj.AddComponent<EnvironmentalNoteDisplay>();
        display.Initialize( note.Text, wallPosition, wallRotation );
      }
    }
  }

  private bool FindWallInRoom( RectInt room, DungeonGenerator.CellType[,] grid, float cellSize, out Vector3 position, out Quaternion rotation )
  {
    int gridWidth = grid.GetLength( 0 );
    int gridHeight = grid.GetLength( 1 );

    int midX = room.x + room.width / 2;
    int midY = room.y + room.height / 2;

    int northWallY = room.y + room.height;
    if( northWallY < gridHeight && midX >= 0 && midX < gridWidth )
    {
      if( grid[ midX, northWallY ] == DungeonGenerator.CellType.Wall )
      {
        position = new Vector3( midX * cellSize, cellSize * 0.5f, northWallY * cellSize );
        rotation = Quaternion.Euler( 0f, 180f, 0f );
        return true;
      }
    }

    int southWallY = room.y - 1;
    if( southWallY >= 0 && midX >= 0 && midX < gridWidth )
    {
      if( grid[ midX, southWallY ] == DungeonGenerator.CellType.Wall )
      {
        position = new Vector3( midX * cellSize, cellSize * 0.5f, southWallY * cellSize );
        rotation = Quaternion.Euler( 0f, 0f, 0f );
        return true;
      }
    }

    int eastWallX = room.x + room.width;
    if( eastWallX < gridWidth && midY >= 0 && midY < gridHeight )
    {
      if( grid[ eastWallX, midY ] == DungeonGenerator.CellType.Wall )
      {
        position = new Vector3( eastWallX * cellSize, cellSize * 0.5f, midY * cellSize );
        rotation = Quaternion.Euler( 0f, 270f, 0f );
        return true;
      }
    }

    int westWallX = room.x - 1;
    if( westWallX >= 0 && midY >= 0 && midY < gridHeight )
    {
      if( grid[ westWallX, midY ] == DungeonGenerator.CellType.Wall )
      {
        position = new Vector3( westWallX * cellSize, cellSize * 0.5f, midY * cellSize );
        rotation = Quaternion.Euler( 0f, 90f, 0f );
        return true;
      }
    }

    position = Vector3.zero;
    rotation = Quaternion.identity;
    return false;
  }

  private void BakeNavMesh()
  {
    Transform dungeonParent = DungeonGen.GetDungeonParent();
    if( dungeonParent == null )
    {
      return;
    }

    if( navMeshSurface != null )
    {
      Destroy( navMeshSurface );
    }

    navMeshSurface = dungeonParent.gameObject.AddComponent<NavMeshSurface>();
    navMeshSurface.collectObjects = CollectObjects.Children;
    navMeshSurface.BuildNavMesh();
  }
}
