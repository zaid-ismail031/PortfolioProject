using UnityEngine;

[CreateAssetMenu( fileName = "NewLevelTheme", menuName = "PCG/Level Theme" )]
public class LevelTheme : ScriptableObject
{
  [Header( "Theme Identity" )]
  [SerializeField]
  private string ThemeName;
  [SerializeField]
  private NarrativeTheme NarrativeTheme;
  [SerializeField]
  private NarrativeRules NarrativeRules;

  [Header( "Terrain Settings" )]
  [SerializeField]
  private TerrainLayer[] TerrainLayers;
  [SerializeField]
  private float TerrainFrequency = 0.005f;
  [SerializeField]
  [Range( 0f, 1f )]
  private float TerrainAmplitude = 1.0f;
  [SerializeField]
  [Range( 1, 8 )]
  private int TerrainOctaves = 4;
  [SerializeField]
  private float TerrainHeight = 50f;

  [Header( "Dungeon Prefabs" )]
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

  [Header( "Props" )]
  [SerializeField]
  private GameObject[] PropPrefabs;
  [SerializeField]
  [Range( 0, 10 )]
  private int PropsPerRoom = 3;
  [SerializeField]
  private float PropYOffset = 0.5f;

  [Header( "Lighting" )]
  [SerializeField]
  private Color AmbientColor = new Color( 0.5f, 0.5f, 0.5f );
  [SerializeField]
  private Color DirectionalLightColor = Color.white;
  [SerializeField]
  private float DirectionalLightIntensity = 1f;

  [Header( "Fog" )]
  [SerializeField]
  private bool EnableFog;
  [SerializeField]
  private Color FogColor = Color.gray;
  [SerializeField]
  private float FogDensity = 0.02f;

  [Header( "Skybox" )]
  [SerializeField]
  private Material SkyboxMaterial;

  public string GetThemeName()
  {
    return ThemeName;
  }

  public NarrativeTheme GetNarrativeTheme()
  {
    return NarrativeTheme;
  }

  public NarrativeRules GetNarrativeRules()
  {
    return NarrativeRules;
  }

  public TerrainLayer[] GetTerrainLayers()
  {
    return TerrainLayers;
  }

  public float GetTerrainFrequency()
  {
    return TerrainFrequency;
  }

  public float GetTerrainAmplitude()
  {
    return TerrainAmplitude;
  }

  public int GetTerrainOctaves()
  {
    return TerrainOctaves;
  }

  public float GetTerrainHeight()
  {
    return TerrainHeight;
  }

  public GameObject GetFloorPrefab()
  {
    return FloorPrefab;
  }

  public GameObject GetWallPrefab()
  {
    return WallPrefab;
  }

  public GameObject GetCorridorPrefab()
  {
    return CorridorPrefab;
  }

  public GameObject GetStartMarkerPrefab()
  {
    return StartMarkerPrefab;
  }

  public GameObject GetGoalMarkerPrefab()
  {
    return GoalMarkerPrefab;
  }

  public GameObject[] GetPropPrefabs()
  {
    return PropPrefabs;
  }

  public int GetPropsPerRoom()
  {
    return PropsPerRoom;
  }

  public float GetPropYOffset()
  {
    return PropYOffset;
  }

  public Color GetAmbientColor()
  {
    return AmbientColor;
  }

  public Color GetDirectionalLightColor()
  {
    return DirectionalLightColor;
  }

  public float GetDirectionalLightIntensity()
  {
    return DirectionalLightIntensity;
  }

  public bool GetEnableFog()
  {
    return EnableFog;
  }

  public Color GetFogColor()
  {
    return FogColor;
  }

  public float GetFogDensity()
  {
    return FogDensity;
  }

  public Material GetSkyboxMaterial()
  {
    return SkyboxMaterial;
  }
}
