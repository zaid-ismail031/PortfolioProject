using UnityEngine;

[CreateAssetMenu( fileName = "NewRoomObjectCollection", menuName = "PCG/Room Object Collection" )]
public class RoomObjectCollection : ScriptableObject
{
  [System.Serializable]
  public class RoomObject
  {
    [SerializeField]
    private GameObject Prefab;
    [SerializeField]
    private Vector3 PositionOffset;
    [SerializeField]
    private Vector3 RotationOffset;
    [SerializeField]
    private Vector3 Scale = Vector3.one;

    public GameObject GetPrefab()
    {
      return Prefab;
    }

    public Vector3 GetPositionOffset()
    {
      return PositionOffset;
    }

    public Vector3 GetRotationOffset()
    {
      return RotationOffset;
    }

    public Vector3 GetScale()
    {
      return Scale;
    }
  }

  [Header( "Objects" )]
  [SerializeField]
  private RoomObject[] Objects;

  public RoomObject[] GetObjects()
  {
    return Objects;
  }
}
