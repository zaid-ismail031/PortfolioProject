using UnityEngine;
using UnityEngine.UI;

public class NPCBackstoryLabel : MonoBehaviour
{
  [SerializeField]
  private Vector3 Offset = new Vector3( 0f, 2.8f, 0f );
  [SerializeField]
  private int FontSize = 20;
  [SerializeField]
  private Color BackgroundColor = new Color( 0f, 0f, 0f, 0.7f );

  public string Backstory { get; private set; }
  public string NPCName { get; private set; }
  public string RoleName { get; private set; }

  private Camera mainCamera;
  private Transform canvasTransform;

  public void Initialize( string backstory, string npcName, string roleName, Color roleColor )
  {
    Backstory = backstory;
    NPCName = npcName;
    RoleName = roleName;
    mainCamera = Camera.main;

    GameObject canvasObj = new GameObject( "NameLabelCanvas" );
    canvasObj.transform.SetParent( transform );
    canvasObj.transform.localPosition = Offset;

    Canvas canvas = canvasObj.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.WorldSpace;
    canvas.sortingOrder = 3;

    CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
    scaler.dynamicPixelsPerUnit = 10f;

    RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
    canvasRect.sizeDelta = new Vector2( 300f, 60f );
    canvasRect.localScale = Vector3.one * 0.01f;

    GameObject bgObj = new GameObject( "Background" );
    bgObj.transform.SetParent( canvasObj.transform, false );
    Image bgImage = bgObj.AddComponent<Image>();
    bgImage.color = BackgroundColor;
    RectTransform bgRect = bgObj.GetComponent<RectTransform>();
    bgRect.anchorMin = Vector2.zero;
    bgRect.anchorMax = Vector2.one;
    bgRect.offsetMin = Vector2.zero;
    bgRect.offsetMax = Vector2.zero;

    GameObject roleObj = new GameObject( "RoleText" );
    roleObj.transform.SetParent( canvasObj.transform, false );
    Text roleText = roleObj.AddComponent<Text>();
    roleText.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    roleText.fontSize = FontSize + 4;
    roleText.color = roleColor;
    roleText.alignment = TextAnchor.UpperCenter;
    roleText.horizontalOverflow = HorizontalWrapMode.Overflow;
    roleText.verticalOverflow = VerticalWrapMode.Overflow;
    roleText.text = roleName.ToUpper();

    RectTransform roleRect = roleObj.GetComponent<RectTransform>();
    roleRect.anchorMin = new Vector2( 0f, 0.5f );
    roleRect.anchorMax = Vector2.one;
    roleRect.offsetMin = Vector2.zero;
    roleRect.offsetMax = Vector2.zero;

    GameObject nameObj = new GameObject( "NameText" );
    nameObj.transform.SetParent( canvasObj.transform, false );
    Text nameText = nameObj.AddComponent<Text>();
    nameText.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    nameText.fontSize = FontSize;
    nameText.color = Color.white;
    nameText.alignment = TextAnchor.LowerCenter;
    nameText.horizontalOverflow = HorizontalWrapMode.Overflow;
    nameText.verticalOverflow = VerticalWrapMode.Overflow;
    nameText.text = npcName;

    RectTransform nameRect = nameObj.GetComponent<RectTransform>();
    nameRect.anchorMin = Vector2.zero;
    nameRect.anchorMax = new Vector2( 1f, 0.5f );
    nameRect.offsetMin = Vector2.zero;
    nameRect.offsetMax = Vector2.zero;

    canvasTransform = canvasObj.transform;
  }

  private void LateUpdate()
  {
    if( mainCamera == null || canvasTransform == null )
    {
      return;
    }

    canvasTransform.forward = mainCamera.transform.forward;
  }

  public Color GetRoleColor( PCGNPCRole role )
  {
    switch( role )
    {
      case PCGNPCRole.Attacker:
        return new Color( 1f, 0.3f, 0.3f );
      case PCGNPCRole.Passive:
        return new Color( 0.3f, 0.6f, 1f );
      case PCGNPCRole.Coward:
        return new Color( 1f, 0.8f, 0.2f );
      case PCGNPCRole.QuestGiver:
        return new Color( 0.3f, 1f, 0.5f );
      default:
        return Color.white;
    }
  }
}
