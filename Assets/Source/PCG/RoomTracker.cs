using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomTracker : MonoBehaviour
{
  [Header( "References" )]
  [SerializeField]
  private DungeonGenerator DungeonGen;
  [SerializeField]
  private Transform Player;

  [Header( "HUD" )]
  [SerializeField]
  private Canvas HUDCanvas;
  [SerializeField]
  private int FontSize = 20;
  [SerializeField]
  private Color TextColor = Color.white;
  [SerializeField]
  private Color BackgroundColor = new Color( 0f, 0f, 0f, 0.6f );

  private Text roomText;
  private int currentRoomIndex = -1;

  private void Start()
  {
    CreateHUD();
  }

  private void Update()
  {
    if( DungeonGen == null || Player == null || roomText == null )
    {
      return;
    }

    int roomIndex = GetPlayerRoomIndex();
    if( roomIndex != currentRoomIndex )
    {
      currentRoomIndex = roomIndex;
      UpdateRoomText();
    }
  }

  private int GetPlayerRoomIndex()
  {
    List<RectInt> rooms = DungeonGen.GetRooms();
    if( rooms == null )
    {
      return -1;
    }

    float cellSize = DungeonGen.GetCellSize();
    Vector3 dungeonPos = DungeonGen.GetDungeonParent().transform.position;
    Vector3 playerPos = Player.position;

    int cellX = Mathf.FloorToInt( ( playerPos.x - dungeonPos.x ) / cellSize );
    int cellZ = Mathf.FloorToInt( ( playerPos.z - dungeonPos.z ) / cellSize );

    for( int i = 0; i < rooms.Count; i++ )
    {
      RectInt room = rooms[ i ];
      if( cellX >= room.x && cellX < room.x + room.width && cellZ >= room.y && cellZ < room.y + room.height )
      {
        return i;
      }
    }

    return -1;
  }

  private void UpdateRoomText()
  {
    if( currentRoomIndex == -1 )
    {
      roomText.text = "";
      return;
    }

    string label = "Room " + currentRoomIndex;

    if( currentRoomIndex == DungeonGen.GetStartRoomIndex() )
    {
      label += " (Start)";
    }
    else if( currentRoomIndex == DungeonGen.GetGoalRoomIndex() )
    {
      label += " (Goal)";
    }

    int difficulty = DungeonGen.GetRoomDifficulty( currentRoomIndex );
    label += "  |  Difficulty: " + difficulty;

    roomText.text = label;
  }

  private void CreateHUD()
  {
    GameObject panelObj = new GameObject( "RoomPanel" );
    panelObj.transform.SetParent( HUDCanvas.transform, false );

    Image panelImage = panelObj.AddComponent<Image>();
    panelImage.color = BackgroundColor;

    RectTransform panelRect = panelObj.GetComponent<RectTransform>();
    panelRect.anchorMin = new Vector2( 0.5f, 1f );
    panelRect.anchorMax = new Vector2( 0.5f, 1f );
    panelRect.pivot = new Vector2( 0.5f, 1f );
    panelRect.anchoredPosition = new Vector2( 0f, -10f );
    panelRect.sizeDelta = new Vector2( 300f, 40f );

    GameObject textObj = new GameObject( "RoomText" );
    textObj.transform.SetParent( panelObj.transform, false );

    roomText = textObj.AddComponent<Text>();
    roomText.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    roomText.fontSize = FontSize;
    roomText.color = TextColor;
    roomText.alignment = TextAnchor.MiddleCenter;
    roomText.horizontalOverflow = HorizontalWrapMode.Overflow;

    RectTransform textRect = textObj.GetComponent<RectTransform>();
    textRect.anchorMin = Vector2.zero;
    textRect.anchorMax = Vector2.one;
    textRect.offsetMin = Vector2.zero;
    textRect.offsetMax = Vector2.zero;
  }
}
