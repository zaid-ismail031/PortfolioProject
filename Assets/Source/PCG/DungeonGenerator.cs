using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
  public enum CellType
  {
    Empty,
    Floor,
    Wall,
    Corridor
  }

  [Header( "Dungeon Dimensions" )]
  [SerializeField]
  private int DungeonWidth = 50;
  [SerializeField]
  private int DungeonHeight = 50;

  [Header( "BSP Settings" )]
  [SerializeField]
  private int MinPartitionSize = 10;
  [SerializeField]
  private int MinRoomSize = 4;
  [SerializeField]
  private int RoomPadding = 2;
  [SerializeField]
  [Range( 1, 3 )]
  private int CorridorWidth = 2;
  [SerializeField]
  [Range( 1, 8 )]
  private int MaxSplitDepth = 5;

  [Header( "Prefabs" )]
  [SerializeField]
  private GameObject FloorPrefab;
  [SerializeField]
  private GameObject WallPrefab;
  [SerializeField]
  private GameObject CorridorPrefab;
  [SerializeField]
  private GameObject StartMarkerPrefab;
  [SerializeField]
  private GameObject GoalMarkerPrefab;
  [SerializeField]
  private float WallPrefabPositionForwardOffset = 2.0f;
  [SerializeField]
  private float WallPrefabPositionHeightOffset = 2.0f;

  [Header( "Entrances" )]
  [SerializeField]
  [Range( 1, 5 )]
  private int EntranceWidth = 3;

  [Header( "Generation" )]
  [SerializeField]
  private int Seed;
  [SerializeField]
  private bool RandomizeSeed = true;
  [SerializeField]
  private bool GenerateOnStart = true;
  [SerializeField]
  private float CellSize = 1f;

  [Header( "Difficulty" )]
  [SerializeField]
  [Range( 1, 10 )]
  private int DifficultyLevels = 5;

  private CellType[,] grid;
  private List<RectInt> rooms;
  private List<List<int>> roomAdjacency;
  private int startRoomIndex;
  private int goalRoomIndex;
  private int[] roomDifficulties;
  private System.Random random;
  private Transform dungeonParent;

  private void Start()
  {
    if( RandomizeSeed )
    {
      Seed = Random.Range( 0, 100000 );
    }

    if( GenerateOnStart )
    {
      Generate();
    }
  }

  [ContextMenu( "Generate Dungeon" )]
  public void Generate()
  {
    random = new System.Random( Seed );
    grid = new CellType[DungeonWidth, DungeonHeight];
    rooms = new List<RectInt>();

    BSPNode root = new BSPNode( new RectInt( 0, 0, DungeonWidth, DungeonHeight ) );
    SplitNode( root, 0 );
    CreateRooms( root );

    roomAdjacency = new List<List<int>>();
    for( int i = 0; i < rooms.Count; i++ )
    {
      roomAdjacency.Add( new List<int>() );
    }

    ConnectRooms( root );
    AddWalls();
    AssignStartGoalAndDifficulty();
    CarveEntrances();
    BuildDungeon();
  }

  private void SplitNode( BSPNode node, int depth )
  {
    if( depth >= MaxSplitDepth )
    {
      return;
    }

    if( node.Bounds.width < MinPartitionSize * 2 && node.Bounds.height < MinPartitionSize * 2 )
    {
      return;
    }

    bool splitHorizontal;
    if( node.Bounds.width > node.Bounds.height )
    {
      splitHorizontal = false;
    }
    else if( node.Bounds.height > node.Bounds.width )
    {
      splitHorizontal = true;
    }
    else
    {
      splitHorizontal = random.Next( 2 ) == 0;
    }

    if( splitHorizontal )
    {
      if( node.Bounds.height < MinPartitionSize * 2 )
      {
        return;
      }

      int splitPosition = random.Next( MinPartitionSize, node.Bounds.height - MinPartitionSize + 1 );
      node.Left = new BSPNode( new RectInt( node.Bounds.x, node.Bounds.y, node.Bounds.width, splitPosition ) );
      node.Right = new BSPNode( new RectInt( node.Bounds.x, node.Bounds.y + splitPosition, node.Bounds.width, node.Bounds.height - splitPosition ) );
    }
    else
    {
      if( node.Bounds.width < MinPartitionSize * 2 )
      {
        return;
      }

      int splitPosition = random.Next( MinPartitionSize, node.Bounds.width - MinPartitionSize + 1 );
      node.Left = new BSPNode( new RectInt( node.Bounds.x, node.Bounds.y, splitPosition, node.Bounds.height ) );
      node.Right = new BSPNode( new RectInt( node.Bounds.x + splitPosition, node.Bounds.y, node.Bounds.width - splitPosition, node.Bounds.height ) );
    }

    SplitNode( node.Left, depth + 1 );
    SplitNode( node.Right, depth + 1 );
  }

  private void CreateRooms( BSPNode node )
  {
    if( node.IsLeaf )
    {
      int maxRoomWidth = node.Bounds.width - RoomPadding * 2;
      int maxRoomHeight = node.Bounds.height - RoomPadding * 2;

      if( maxRoomWidth < MinRoomSize || maxRoomHeight < MinRoomSize )
      {
        return;
      }

      int roomWidth = random.Next( MinRoomSize, maxRoomWidth + 1 );
      int roomHeight = random.Next( MinRoomSize, maxRoomHeight + 1 );

      int roomX = node.Bounds.x + random.Next( RoomPadding, node.Bounds.width - roomWidth - RoomPadding + 1 );
      int roomY = node.Bounds.y + random.Next( RoomPadding, node.Bounds.height - roomHeight - RoomPadding + 1 );

      RectInt room = new RectInt( roomX, roomY, roomWidth, roomHeight );
      node.RoomIndex = rooms.Count;
      rooms.Add( room );
      CarveRoom( room );
      return;
    }

    if( node.Left != null )
    {
      CreateRooms( node.Left );
    }

    if( node.Right != null )
    {
      CreateRooms( node.Right );
    }
  }

  private void CarveRoom( RectInt room )
  {
    for( int x = room.x; x < room.x + room.width; x++ )
    {
      for( int y = room.y; y < room.y + room.height; y++ )
      {
        if( x >= 0 && x < DungeonWidth && y >= 0 && y < DungeonHeight )
        {
          grid[ x, y ] = CellType.Floor;
        }
      }
    }
  }

  private void ConnectRooms( BSPNode node )
  {
    if( node.IsLeaf )
    {
      return;
    }

    if( node.Left != null )
    {
      ConnectRooms( node.Left );
    }

    if( node.Right != null )
    {
      ConnectRooms( node.Right );
    }

    if( node.Left != null && node.Right != null )
    {
      int leftRoomIndex = GetRoomIndexFromNode( node.Left );
      int rightRoomIndex = GetRoomIndexFromNode( node.Right );

      if( leftRoomIndex < 0 || rightRoomIndex < 0 )
      {
        return;
      }

      RectInt leftRoom = rooms[ leftRoomIndex ];
      RectInt rightRoom = rooms[ rightRoomIndex ];

      Vector2Int leftCenter = new Vector2Int( leftRoom.x + leftRoom.width / 2, leftRoom.y + leftRoom.height / 2 );
      Vector2Int rightCenter = new Vector2Int( rightRoom.x + rightRoom.width / 2, rightRoom.y + rightRoom.height / 2 );

      CarveCorridor( leftCenter, rightCenter );

      roomAdjacency[ leftRoomIndex ].Add( rightRoomIndex );
      roomAdjacency[ rightRoomIndex ].Add( leftRoomIndex );
    }
  }

  private int GetRoomIndexFromNode( BSPNode node )
  {
    if( node.RoomIndex >= 0 )
    {
      return node.RoomIndex;
    }

    if( node.Left != null )
    {
      int leftResult = GetRoomIndexFromNode( node.Left );
      if( leftResult >= 0 )
      {
        return leftResult;
      }
    }

    if( node.Right != null )
    {
      return GetRoomIndexFromNode( node.Right );
    }

    return -1;
  }

  private void CarveCorridor( Vector2Int from, Vector2Int to )
  {
    int x = from.x;
    int y = from.y;

    while( x != to.x )
    {
      for( int w = 0; w < CorridorWidth; w++ )
      {
        int cellY = y + w;
        if( x >= 0 && x < DungeonWidth && cellY >= 0 && cellY < DungeonHeight )
        {
          if( grid[ x, cellY ] == CellType.Empty )
          {
            grid[ x, cellY ] = CellType.Corridor;
          }
        }
      }

      x += ( to.x > from.x ) ? 1 : -1;
    }

    while( y != to.y )
    {
      for( int w = 0; w < CorridorWidth; w++ )
      {
        int cellX = x + w;
        if( cellX >= 0 && cellX < DungeonWidth && y >= 0 && y < DungeonHeight )
        {
          if( grid[ cellX, y ] == CellType.Empty )
          {
            grid[ cellX, y ] = CellType.Corridor;
          }
        }
      }

      y += ( to.y > from.y ) ? 1 : -1;
    }
  }

  private void AddWalls()
  {
    CellType[,] wallGrid = (CellType[,])grid.Clone();

    for( int x = 0; x < DungeonWidth; x++ )
    {
      for( int y = 0; y < DungeonHeight; y++ )
      {
        if( grid[ x, y ] != CellType.Empty )
        {
          continue;
        }

        if( IsAdjacentToFloor( x, y ) )
        {
          wallGrid[ x, y ] = CellType.Wall;
        }
      }
    }

    grid = wallGrid;
  }

  private bool IsFloorOrCorridor( int x, int y )
  {
    if( x < 0 || x >= DungeonWidth || y < 0 || y >= DungeonHeight )
    {
      return false;
    }

    return grid[ x, y ] == CellType.Floor || grid[ x, y ] == CellType.Corridor;
  }

  private Quaternion GetWallRotation( int x, int y )
  {
    bool floorNorth = IsFloorOrCorridor( x, y + 1 );
    bool floorSouth = IsFloorOrCorridor( x, y - 1 );
    bool floorEast = IsFloorOrCorridor( x + 1, y );
    bool floorWest = IsFloorOrCorridor( x - 1, y );

    if( floorNorth && !floorSouth && !floorEast && !floorWest )
    {
      return Quaternion.Euler( 0f, 0f, 0f );
    }

    if( floorSouth && !floorNorth && !floorEast && !floorWest )
    {
      return Quaternion.Euler( 0f, 180f, 0f );
    }

    if( floorEast && !floorWest && !floorNorth && !floorSouth )
    {
      return Quaternion.Euler( 0f, 90f, 0f );
    }

    if( floorWest && !floorEast && !floorNorth && !floorSouth )
    {
      return Quaternion.Euler( 0f, 270f, 0f );
    }

    if( floorNorth || floorSouth )
    {
      return Quaternion.Euler( 0f, 0f, 0f );
    }

    if( floorEast || floorWest )
    {
      return Quaternion.Euler( 0f, 90f, 0f );
    }

    return Quaternion.identity;
  }

  private bool IsAdjacentToFloor( int x, int y )
  {
    for( int dx = -1; dx <= 1; dx++ )
    {
      for( int dy = -1; dy <= 1; dy++ )
      {
        if( dx == 0 && dy == 0 )
        {
          continue;
        }

        int neighborX = x + dx;
        int neighborY = y + dy;

        if( neighborX >= 0 && neighborX < DungeonWidth && neighborY >= 0 && neighborY < DungeonHeight )
        {
          if( grid[ neighborX, neighborY ] == CellType.Floor || grid[ neighborX, neighborY ] == CellType.Corridor )
          {
            return true;
          }
        }
      }
    }

    return false;
  }

  private void AssignStartGoalAndDifficulty()
  {
    if( rooms.Count == 0 )
    {
      return;
    }

    startRoomIndex = 0;

    int[] distances = new int[rooms.Count];
    for( int i = 0; i < distances.Length; i++ )
    {
      distances[ i ] = -1;
    }

    Queue<int> queue = new Queue<int>();
    queue.Enqueue( startRoomIndex );
    distances[ startRoomIndex ] = 0;

    int maxDistance = 0;
    int furthestRoom = startRoomIndex;

    while( queue.Count > 0 )
    {
      int current = queue.Dequeue();

      foreach( int neighbor in roomAdjacency[ current ] )
      {
        if( distances[ neighbor ] == -1 )
        {
          distances[ neighbor ] = distances[ current ] + 1;
          queue.Enqueue( neighbor );

          if( distances[ neighbor ] > maxDistance )
          {
            maxDistance = distances[ neighbor ];
            furthestRoom = neighbor;
          }
        }
      }
    }

    goalRoomIndex = furthestRoom;

    roomDifficulties = new int[rooms.Count];
    for( int i = 0; i < rooms.Count; i++ )
    {
      if( maxDistance > 0 )
      {
        roomDifficulties[ i ] = Mathf.CeilToInt( (float)distances[ i ] / maxDistance * DifficultyLevels );
      }
      else
      {
        roomDifficulties[ i ] = 1;
      }
    }
  }

  private void CarveEntrances()
  {
    if( rooms.Count == 0 )
    {
      return;
    }

    RectInt startRoom = rooms[ startRoomIndex ];
    int midX = startRoom.x + startRoom.width / 2;
    int halfWidth = EntranceWidth / 2;

    for( int offset = -halfWidth; offset <= halfWidth; offset++ )
    {
      int x = midX + offset;
      if( x < 0 || x >= DungeonWidth )
      {
        continue;
      }

      for( int y = startRoom.y - 1; y >= 0; y-- )
      {
        if( grid[ x, y ] == CellType.Wall )
        {
          grid[ x, y ] = CellType.Floor;
        }
        else if( grid[ x, y ] == CellType.Empty )
        {
          break;
        }
      }
    }

    RectInt goalRoom = rooms[ goalRoomIndex ];
    int goalMidX = goalRoom.x + goalRoom.width / 2;

    for( int offset = -halfWidth; offset <= halfWidth; offset++ )
    {
      int x = goalMidX + offset;
      if( x < 0 || x >= DungeonWidth )
      {
        continue;
      }

      for( int y = goalRoom.y + goalRoom.height; y < DungeonHeight; y++ )
      {
        if( grid[ x, y ] == CellType.Wall )
        {
          grid[ x, y ] = CellType.Floor;
        }
        else if( grid[ x, y ] == CellType.Empty )
        {
          break;
        }
      }
    }
  }

  private void BuildDungeon()
  {
    ClearDungeon();

    dungeonParent = new GameObject( "GeneratedDungeon" ).transform;

    for( int x = 0; x < DungeonWidth; x++ )
    {
      for( int y = 0; y < DungeonHeight; y++ )
      {
        Vector3 position = new Vector3( x * CellSize, 0f, y * CellSize );
        GameObject prefab = null;

        Quaternion rotation = Quaternion.identity;

        switch( grid[ x, y ] )
        {
          case CellType.Floor:
            prefab = FloorPrefab;
            break;
          case CellType.Wall:
            prefab = WallPrefab;
            rotation = GetWallRotation( x, y );
            break;
          case CellType.Corridor:
            prefab = CorridorPrefab != null ? CorridorPrefab : FloorPrefab;
            break;
        }

        if( prefab != null )
        {
          GameObject instantiatedPrefab = Instantiate( prefab, position, rotation, dungeonParent );

          if( prefab == WallPrefab )
          {
            instantiatedPrefab.transform.position += ( instantiatedPrefab.transform.forward * WallPrefabPositionForwardOffset );
            instantiatedPrefab.transform.position += ( instantiatedPrefab.transform.up * WallPrefabPositionHeightOffset );
          }
        }
      }
    }

    PlaceMarkers();
  }

  private void PlaceMarkers()
  {
    if( rooms.Count == 0 )
    {
      return;
    }

    if( StartMarkerPrefab != null )
    {
      RectInt startRoom = rooms[ startRoomIndex ];
      Vector3 startPosition = new Vector3(
        ( startRoom.x + startRoom.width / 2f ) * CellSize,
        CellSize,
        ( startRoom.y + startRoom.height / 2f ) * CellSize
      );
      Instantiate( StartMarkerPrefab, startPosition, Quaternion.identity, dungeonParent );
    }

    if( GoalMarkerPrefab != null )
    {
      RectInt goalRoom = rooms[ goalRoomIndex ];
      Vector3 goalPosition = new Vector3(
        ( goalRoom.x + goalRoom.width / 2f ) * CellSize,
        CellSize,
        ( goalRoom.y + goalRoom.height / 2f ) * CellSize
      );
      Instantiate( GoalMarkerPrefab, goalPosition, Quaternion.identity, dungeonParent );
    }
  }

  private void ClearDungeon()
  {
    if( dungeonParent != null )
    {
      Destroy( dungeonParent.gameObject );
    }
  }

  public Transform GetDungeonParent()
  {
    return dungeonParent;
  }

  public CellType[,] GetGrid()
  {
    return grid;
  }

  public List<RectInt> GetRooms()
  {
    return rooms;
  }

  public List<List<int>> GetAdjacency()
  {
    return roomAdjacency;
  }

  public int GetRoomDifficulty( int roomIndex )
  {
    if( roomDifficulties != null && roomIndex >= 0 && roomIndex < roomDifficulties.Length )
    {
      return roomDifficulties[ roomIndex ];
    }

    return 0;
  }

  public int GetStartRoomIndex()
  {
    return startRoomIndex;
  }

  public int GetGoalRoomIndex()
  {
    return goalRoomIndex;
  }

  public int GetSeed()
  {
    return Seed;
  }

  public float GetCellSize()
  {
    return CellSize;
  }

  public int GetDungeonWidth()
  {
    return DungeonWidth;
  }

  public int GetDungeonHeight()
  {
    return DungeonHeight;
  }

  public void SetPrefabs( GameObject floor, GameObject wall, GameObject corridor )
  {
    FloorPrefab = floor;
    WallPrefab = wall;
    CorridorPrefab = corridor;
  }

  public void SetMarkerPrefabs( GameObject start, GameObject goal )
  {
    StartMarkerPrefab = start;
    GoalMarkerPrefab = goal;
  }

  private class BSPNode
  {
    public RectInt Bounds;
    public BSPNode Left;
    public BSPNode Right;
    public int RoomIndex = -1;

    public bool IsLeaf
    {
      get { return Left == null && Right == null; }
    }

    public BSPNode( RectInt bounds )
    {
      Bounds = bounds;
    }
  }
}
