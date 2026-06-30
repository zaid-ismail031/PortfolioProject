using Models.BehaviourTree;
using UnityEngine;
using UnityEngine.UI;

public class NPCRoleLabel : MonoBehaviour
{
  [SerializeField]
  private NPCController npcController;

  [SerializeField]
  private SquadController squadController;

  [SerializeField]
  private Vector3 offset = new Vector3( 0f, 2.6f, 0f );

  [SerializeField]
  private int fontSize = 32;

  [SerializeField]
  private Color textColor = Color.white;

  [SerializeField]
  private Color bgColor = new Color( 0f, 0f, 0f, 0.6f );

  private Camera mainCamera;
  private Transform canvasTransform;
  private Text roleText;
  private SquadRole lastRole;

  private void Start()
  {
    mainCamera = Camera.main;

    GameObject canvasObj = new GameObject( "RoleLabelCanvas" );
    canvasObj.transform.SetParent( transform );
    canvasObj.transform.localPosition = offset;

    Canvas canvas = canvasObj.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.WorldSpace;
    canvas.sortingOrder = 2;

    CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
    scaler.dynamicPixelsPerUnit = 10f;

    RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
    canvasRect.sizeDelta = new Vector2( 200f, 40f );
    canvasRect.localScale = Vector3.one * 0.01f;

    GameObject bgObj = new GameObject( "Background" );
    bgObj.transform.SetParent( canvasObj.transform, false );
    Image bgImage = bgObj.AddComponent<Image>();
    bgImage.color = bgColor;
    RectTransform bgRect = bgObj.GetComponent<RectTransform>();
    bgRect.anchorMin = Vector2.zero;
    bgRect.anchorMax = Vector2.one;
    bgRect.offsetMin = Vector2.zero;
    bgRect.offsetMax = Vector2.zero;

    GameObject textObj = new GameObject( "RoleText" );
    textObj.transform.SetParent( canvasObj.transform, false );
    roleText = textObj.AddComponent<Text>();
    roleText.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    roleText.fontSize = fontSize;
    roleText.color = textColor;
    roleText.alignment = TextAnchor.MiddleCenter;
    roleText.horizontalOverflow = HorizontalWrapMode.Overflow;
    roleText.verticalOverflow = VerticalWrapMode.Overflow;

    RectTransform textRect = textObj.GetComponent<RectTransform>();
    textRect.anchorMin = Vector2.zero;
    textRect.anchorMax = Vector2.one;
    textRect.offsetMin = Vector2.zero;
    textRect.offsetMax = Vector2.zero;

    canvasTransform = canvasObj.transform;
    lastRole = SquadRole.None;
    roleText.text = "";
  }

  private void LateUpdate()
  {
    if( mainCamera == null || squadController == null || npcController == null )
    {
      return;
    }

    canvasTransform.forward = mainCamera.transform.forward;

    SquadRole currentRole = squadController.Blackboard.GetRole( npcController );
    if( currentRole != lastRole )
    {
      lastRole = currentRole;
      roleText.text = currentRole == SquadRole.None ? "" : currentRole.ToString().ToUpper();
      roleText.color = GetRoleColor( currentRole );
    }
  }

  private Color GetRoleColor( SquadRole role )
  {
    switch( role )
    {
      case SquadRole.Rusher:  return new Color( 1f, 0.3f, 0.3f );
      case SquadRole.Flanker: return new Color( 1f, 0.8f, 0.2f );
      case SquadRole.Vantage: return new Color( 0.3f, 0.6f, 1f );
      case SquadRole.Healer:  return new Color( 0.3f, 1f, 0.4f );
      default:                return textColor;
    }
  }
}
