using UnityEngine;

[RequireComponent( typeof( Terrain ) )]
public class TerrainGenerator : MonoBehaviour
{
  public enum NoiseType
  {
    Perlin,
    Simplex
  }

  [Header( "Dimensions" )]
  [SerializeField]
  private int HeightmapResolution = 257;
  [SerializeField]
  private float TerrainWidth = 256f;
  [SerializeField]
  private float TerrainLength = 256f;
  [SerializeField]
  private float TerrainHeight = 50f;

  [Header( "Noise Settings" )]
  [SerializeField]
  private NoiseType SelectedNoise = NoiseType.Perlin;
  [SerializeField]
  private float Frequency = 0.005f;
  [SerializeField]
  [Range( 0f, 1f )]
  private float Amplitude = 1.0f;
  [SerializeField]
  [Range( 1, 8 )]
  private int Octaves = 4;
  [SerializeField]
  private float Lacunarity = 2.0f;
  [SerializeField]
  [Range( 0f, 1f )]
  private float Persistence = 0.5f;
  [SerializeField]
  private int Seed;
  [SerializeField]
  private bool RandomizeSeed = true;

  [Header( "Terrain Layers" )]
  [SerializeField]
  private TerrainLayer[] Layers;

  [Header( "Layer Height Thresholds" )]
  [SerializeField]
  [Range( 0f, 1f )]
  private float WaterThreshold = 0.3f;
  [SerializeField]
  [Range( 0f, 1f )]
  private float SandThreshold = 0.4f;
  [SerializeField]
  [Range( 0f, 1f )]
  private float GrassThreshold = 0.7f;
  [SerializeField]
  [Range( 0f, 1f )]
  private float RockThreshold = 0.85f;

  [Header( "Generation" )]
  [SerializeField]
  private bool GenerateOnStart = true;

  private Terrain terrain;

  private void Start()
  {
    terrain = GetComponent<Terrain>();

    if( RandomizeSeed )
    {
      Seed = Random.Range( 0, 100000 );
    }

    if( GenerateOnStart )
    {
      Generate();
    }
  }

  [ContextMenu( "Generate Terrain" )]
  public void Generate()
  {
    if( terrain == null )
    {
      terrain = GetComponent<Terrain>();
    }

    TerrainData data = terrain.terrainData;
    data.heightmapResolution = HeightmapResolution;
    data.size = new Vector3( TerrainWidth, TerrainHeight, TerrainLength );

    float[,] heights = GenerateHeightmap();
    data.SetHeights( 0, 0, heights );

    if( Layers != null && Layers.Length > 0 )
    {
      data.terrainLayers = Layers;
      PaintTerrainLayers( data, heights );
    }
  }

  public float[,] GenerateHeightmap()
  {
    int resolution = HeightmapResolution;
    float[,] heights = new float[resolution, resolution];

    System.Random prng = new System.Random( Seed );
    Vector2[] octaveOffsets = new Vector2[Octaves];
    for( int i = 0; i < Octaves; i++ )
    {
      float offsetX = prng.Next( -100000, 100000 );
      float offsetY = prng.Next( -100000, 100000 );
      octaveOffsets[ i ] = new Vector2( offsetX, offsetY );
    }

    float minHeight = float.MaxValue;
    float maxHeight = float.MinValue;

    for( int z = 0; z < resolution; z++ )
    {
      for( int x = 0; x < resolution; x++ )
      {
        float value = SampleFractalNoise( x, z, octaveOffsets );
        heights[ z, x ] = value;

        if( value < minHeight )
        {
          minHeight = value;
        }

        if( value > maxHeight )
        {
          maxHeight = value;
        }
      }
    }

    for( int z = 0; z < resolution; z++ )
    {
      for( int x = 0; x < resolution; x++ )
      {
        heights[ z, x ] = Mathf.InverseLerp( minHeight, maxHeight, heights[ z, x ] ) * Amplitude;
      }
    }

    return heights;
  }

  private float SampleFractalNoise( int x, int z, Vector2[] octaveOffsets )
  {
    float value = 0f;
    float currentAmplitude = 1f;
    float currentFrequency = Frequency;

    for( int i = 0; i < Octaves; i++ )
    {
      float sampleX = ( x + octaveOffsets[ i ].x ) * currentFrequency;
      float sampleZ = ( z + octaveOffsets[ i ].y ) * currentFrequency;

      float perlinValue = SampleNoise( sampleX, sampleZ );
      value += perlinValue * currentAmplitude;

      currentAmplitude *= Persistence;
      currentFrequency *= Lacunarity;
    }

    return value;
  }

  private float SampleNoise( float x, float y )
  {
    switch( SelectedNoise )
    {
      case NoiseType.Simplex:
        return SimplexNoise.Sample( x, y );
      case NoiseType.Perlin:
      default:
        return Mathf.PerlinNoise( x, y ) * 2f - 1f;
    }
  }

  private void PaintTerrainLayers( TerrainData data, float[,] heights )
  {
    int alphamapWidth = data.alphamapWidth;
    int alphamapHeight = data.alphamapHeight;
    int layerCount = data.terrainLayers.Length;

    float[,,] alphamaps = new float[alphamapHeight, alphamapWidth, layerCount];

    for( int z = 0; z < alphamapHeight; z++ )
    {
      for( int x = 0; x < alphamapWidth; x++ )
      {
        float heightX = (float)x / alphamapWidth * ( HeightmapResolution - 1 );
        float heightZ = (float)z / alphamapHeight * ( HeightmapResolution - 1 );

        int hx = Mathf.Clamp( Mathf.FloorToInt( heightX ), 0, HeightmapResolution - 1 );
        int hz = Mathf.Clamp( Mathf.FloorToInt( heightZ ), 0, HeightmapResolution - 1 );

        float height = heights[ hz, hx ] / Amplitude;
        int layerIndex = GetLayerIndex( height );

        for( int l = 0; l < layerCount; l++ )
        {
          alphamaps[ z, x, l ] = l == layerIndex ? 1f : 0f;
        }
      }
    }

    data.SetAlphamaps( 0, 0, alphamaps );
  }

  private int GetLayerIndex( float normalizedHeight )
  {
    if( normalizedHeight < WaterThreshold )
    {
      return 0;
    }

    if( normalizedHeight < SandThreshold )
    {
      return Mathf.Min( 1, Layers.Length - 1 );
    }

    if( normalizedHeight < GrassThreshold )
    {
      return Mathf.Min( 2, Layers.Length - 1 );
    }

    if( normalizedHeight < RockThreshold )
    {
      return Mathf.Min( 3, Layers.Length - 1 );
    }

    return Mathf.Min( 4, Layers.Length - 1 );
  }

  public int GetSeed()
  {
    return Seed;
  }

  public void SetSeed( int seed )
  {
    Seed = seed;
    RandomizeSeed = false;
  }

  public void SetTerrainLayers( TerrainLayer[] layers )
  {
    Layers = layers;
  }

  public void SetNoiseParameters( float frequency, float amplitude, int octaves )
  {
    Frequency = frequency;
    Amplitude = amplitude;
    Octaves = octaves;
  }

  public void SetTerrainHeight( float height )
  {
    TerrainHeight = height;
  }
}
