using UnityEngine;

public static class SimplexNoise
{
  private static readonly int[] permutation =
  {
    151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,
    140,36,103,30,69,142,8,99,37,240,21,10,23,190,6,148,
    247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,
    57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,
    74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,122,
    60,211,133,230,220,105,92,41,55,46,245,40,244,102,143,54,
    65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,
    200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,
    52,217,226,250,124,123,5,202,38,147,118,126,255,82,85,212,
    207,206,59,227,47,16,58,17,182,189,28,42,223,183,170,213,
    119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,
    129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,
    218,246,97,228,251,34,242,193,238,210,144,12,191,179,162,241,
    81,51,145,235,249,14,239,107,49,192,214,31,181,199,106,157,
    184,84,204,176,115,121,50,45,127,4,150,254,138,236,205,93,
    222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
  };

  private static readonly int[] permutationMod12;

  private static readonly float skewFactor = 0.5f * ( 1.732050808f - 1.0f );
  private static readonly float unskewFactor = ( 3.0f - 1.732050808f ) / 6.0f;

  private static readonly Vector2[] gradients =
  {
    new Vector2( 1, 0 ), new Vector2( -1, 0 ), new Vector2( 1, 0 ), new Vector2( -1, 0 ),
    new Vector2( 1, 1 ), new Vector2( -1, 1 ), new Vector2( 1, -1 ), new Vector2( -1, -1 ),
    new Vector2( 0, 1 ), new Vector2( 0, -1 ), new Vector2( 0, 1 ), new Vector2( 0, -1 )
  };

  static SimplexNoise()
  {
    permutationMod12 = new int[512];
    for( int i = 0; i < 512; i++ )
    {
      permutationMod12[ i ] = permutation[ i & 255 ] % 12;
    }
  }

  public static float Sample( float x, float y )
  {
    float skew = ( x + y ) * skewFactor;
    int cellX = FastFloor( x + skew );
    int cellY = FastFloor( y + skew );

    float unskew = ( cellX + cellY ) * unskewFactor;
    float x0 = x - ( cellX - unskew );
    float y0 = y - ( cellY - unskew );

    int offsetX, offsetY;
    if( x0 > y0 )
    {
      offsetX = 1;
      offsetY = 0;
    }
    else
    {
      offsetX = 0;
      offsetY = 1;
    }

    Vector2 point0 = new Vector2( x0, y0 );
    Vector2 point1 = new Vector2( x0 - offsetX + unskewFactor, y0 - offsetY + unskewFactor );
    Vector2 point2 = new Vector2( x0 - 1.0f + 2.0f * unskewFactor, y0 - 1.0f + 2.0f * unskewFactor );

    int wrappedX = cellX & 255;
    int wrappedY = cellY & 255;

    float n0 = ContributionAt( point0, wrappedX, wrappedY );
    float n1 = ContributionAt( point1, wrappedX + offsetX, wrappedY + offsetY );
    float n2 = ContributionAt( point2, wrappedX + 1, wrappedY + 1 );

    return 70.0f * ( n0 + n1 + n2 );
  }

  private static float ContributionAt( Vector2 point, int hashX, int hashY )
  {
    float distance = 0.5f - point.sqrMagnitude;
    if( distance < 0 )
    {
      return 0f;
    }

    int gradientIndex = permutationMod12[ ( hashX + permutation[ hashY & 255 ] ) & 255 ];
    distance *= distance;
    return distance * distance * Vector2.Dot( gradients[ gradientIndex ], point );
  }

  private static int FastFloor( float value )
  {
    int integer = (int)value;
    if( value < integer )
    {
      return integer - 1;
    }

    return integer;
  }
}
