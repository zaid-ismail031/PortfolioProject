using UnityEngine;

public class PyramidGenerator : MonoBehaviour
{
  [Header( "References" )]
  [SerializeField]
  private DungeonGenerator DungeonGen;

  [Header( "Ramp Settings" )]
  [SerializeField]
  private float RampLength = 15f;
  [SerializeField]
  private Material PlatformMaterial;

  private GameObject platformParent;

  [ContextMenu( "Generate Pyramid" )]
  public void Generate()
  {
    if( DungeonGen == null )
    {
      return;
    }

    if( platformParent != null )
    {
      Destroy( platformParent );
    }

    platformParent = new GameObject( "DungeonPlatform" );

    float cellSize = DungeonGen.GetCellSize();
    int dungeonWidth = DungeonGen.GetDungeonWidth();
    int dungeonHeight = DungeonGen.GetDungeonHeight();

    float topWidth = dungeonWidth * cellSize;
    float topDepth = dungeonHeight * cellSize;
    Vector3 dungeonPos = DungeonGen.GetDungeonParent().transform.position;

    float platformHeight = dungeonPos.y;
    if( platformHeight <= 0f )
    {
      return;
    }

    float centerX = dungeonPos.x + topWidth * 0.5f;
    float centerZ = dungeonPos.z + topDepth * 0.5f;

    GameObject platform = GameObject.CreatePrimitive( PrimitiveType.Cube );
    platform.name = "Platform";
    platform.layer = LayerMask.NameToLayer( "Floor" );
    platform.transform.SetParent( platformParent.transform );
    platform.transform.position = new Vector3( centerX, platformHeight * 0.5f, centerZ );
    platform.transform.localScale = new Vector3( topWidth, platformHeight, topDepth );

    if( PlatformMaterial != null )
    {
      platform.GetComponent<Renderer>().material = PlatformMaterial;
    }

    float rampWidth = topWidth;
    float rampHypotenuse = Mathf.Sqrt( RampLength * RampLength + platformHeight * platformHeight );
    float rampAngle = Mathf.Atan2( platformHeight, RampLength ) * Mathf.Rad2Deg;

    GameObject ramp = GameObject.CreatePrimitive( PrimitiveType.Cube );
    ramp.name = "Ramp";
    ramp.layer = LayerMask.NameToLayer( "Floor" );
    ramp.transform.SetParent( platformParent.transform );

    float rampCenterX = centerX;
    float rampCenterY = platformHeight * 0.5f;
    float rampCenterZ = dungeonPos.z - RampLength * 0.5f;

    ramp.transform.position = new Vector3( rampCenterX, rampCenterY, rampCenterZ );
    ramp.transform.rotation = Quaternion.Euler( rampAngle, 180.0f, 0f );
    ramp.transform.localScale = new Vector3( rampWidth, 0.5f, rampHypotenuse );

    if( PlatformMaterial != null )
    {
      ramp.GetComponent<Renderer>().material = PlatformMaterial;
    }
  }
}
