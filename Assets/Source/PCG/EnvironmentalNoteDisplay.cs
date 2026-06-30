using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalNoteDisplay : MonoBehaviour
{
  [Header( "Appearance" )]
  [SerializeField]
  private int FontSize = 14;
  [SerializeField]
  private Color TextColor = new Color( 0.85f, 0.8f, 0.65f );
  [SerializeField]
  private Color BackgroundColor = new Color( 0.15f, 0.12f, 0.08f, 0.85f );
  [SerializeField]
  private float CanvasWidth = 180f;
  [SerializeField]
  private float CanvasHeight = 100f;
  [SerializeField]
  private float CanvasScale = 0.008f;
  
  private float WallOffset = 0.7f;

  public void Initialize( string noteText, Vector3 position, Quaternion rotation )
  {
    transform.position = position;
    transform.rotation = rotation;

    Vector3 offsetPosition = transform.forward * WallOffset;
    transform.position += offsetPosition;

    GameObject canvasObj = new GameObject( "NoteCanvas" );
    canvasObj.transform.SetParent( transform );
    canvasObj.transform.localPosition = Vector3.zero;
    canvasObj.transform.rotation = rotation;
    canvasObj.transform.localEulerAngles = new Vector3( 0.0f, 180.0f, 0.0f);

    Canvas canvas = canvasObj.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.WorldSpace;
    canvas.sortingOrder = 1;

    CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
    scaler.dynamicPixelsPerUnit = 10f;

    RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
    canvasRect.sizeDelta = new Vector2( CanvasWidth, CanvasHeight );
    canvasRect.localScale = Vector3.one * CanvasScale;

    GameObject bgObj = new GameObject( "Background" );
    bgObj.transform.SetParent( canvasObj.transform, false );
    Image bgImage = bgObj.AddComponent<Image>();
    bgImage.color = BackgroundColor;
    RectTransform bgRect = bgObj.GetComponent<RectTransform>();
    bgRect.anchorMin = Vector2.zero;
    bgRect.anchorMax = Vector2.one;
    bgRect.offsetMin = Vector2.zero;
    bgRect.offsetMax = Vector2.zero;

    GameObject textObj = new GameObject( "NoteText" );
    textObj.transform.SetParent( canvasObj.transform, false );
    Text text = textObj.AddComponent<Text>();
    text.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    text.fontSize = FontSize;
    text.color = TextColor;
    text.fontStyle = FontStyle.Italic;
    text.alignment = TextAnchor.MiddleCenter;
    text.horizontalOverflow = HorizontalWrapMode.Wrap;
    text.verticalOverflow = VerticalWrapMode.Overflow;
    text.text = noteText;

    RectTransform textRect = textObj.GetComponent<RectTransform>();
    textRect.anchorMin = Vector2.zero;
    textRect.anchorMax = Vector2.one;
    textRect.offsetMin = new Vector2( 8f, 8f );
    textRect.offsetMax = new Vector2( -8f, -8f );
  }
}
