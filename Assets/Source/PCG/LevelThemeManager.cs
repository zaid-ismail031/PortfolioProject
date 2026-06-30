using System.Collections.Generic;
using UnityEngine;

public class LevelThemeManager : MonoBehaviour
{
  [Header( "Theme" )]
  [SerializeField]
  private LevelTheme ActiveTheme;

  [Header( "Generators" )]
  [SerializeField]
  private TerrainGenerator TerrainGen;
  [SerializeField]
  private DungeonGenerator DungeonGen;

  [Header( "Lighting" )]
  [SerializeField]
  private Light DirectionalLight;

  [Header( "Generation" )]
  [SerializeField]
  private bool ApplyOnStart = true;

  private Transform propsParent;

  private void Start()
  {
    if( ApplyOnStart && ActiveTheme != null )
    {
      ApplyTheme();
    }
  }

  [ContextMenu( "Apply Theme" )]
  public void ApplyTheme()
  {
    if( ActiveTheme == null )
    {
      return;
    }

    ApplyTerrainTheme();
    ApplyDungeonTheme();
    ApplyLighting();
    ApplyFog();
    ApplySkybox();
  }

  private void ApplyTerrainTheme()
  {
    if( TerrainGen == null )
    {
      return;
    }

    TerrainGen.SetTerrainLayers( ActiveTheme.GetTerrainLayers() );
    TerrainGen.SetNoiseParameters(
      ActiveTheme.GetTerrainFrequency(),
      ActiveTheme.GetTerrainAmplitude(),
      ActiveTheme.GetTerrainOctaves()
    );
    TerrainGen.SetTerrainHeight( ActiveTheme.GetTerrainHeight() );
    TerrainGen.Generate();
  }

  private void ApplyDungeonTheme()
  {
    if( DungeonGen == null )
    {
      return;
    }

    DungeonGen.SetPrefabs(
      ActiveTheme.GetFloorPrefab(),
      ActiveTheme.GetWallPrefab(),
      ActiveTheme.GetCorridorPrefab()
    );
    DungeonGen.SetMarkerPrefabs(
      ActiveTheme.GetStartMarkerPrefab(),
      ActiveTheme.GetGoalMarkerPrefab()
    );
    DungeonGen.Generate();

    SpawnProps();
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
            Vector3 position = new Vector3( x * DungeonGen.GetCellSize(), propYOffset, y * DungeonGen.GetCellSize() );
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

  public void SetTheme( LevelTheme theme )
  {
    ActiveTheme = theme;
  }

  public LevelTheme GetActiveTheme()
  {
    return ActiveTheme;
  }
}
