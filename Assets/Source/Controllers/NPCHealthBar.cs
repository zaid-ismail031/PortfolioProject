using UnityEngine;
using UnityEngine.UI;

public class NPCHealthBar : MonoBehaviour
{
  [SerializeField]
  private NPCController npcController;

  [SerializeField]
  private float maxHealth = 100f;

  [SerializeField]
  private Vector3 offset = new Vector3( 0f, 2.2f, 0f );

  [SerializeField]
  private float barWidth = 120f;

  [SerializeField]
  private float barHeight = 15f;

  private Camera mainCamera;
  private RectTransform fillRect;
  private Transform canvasTransform;

  private void Start()
  {
    mainCamera = Camera.main;

    // Build the world-space canvas
    GameObject canvasObj = new GameObject( "HealthBarCanvas" );
    canvasObj.transform.SetParent( transform );
    canvasObj.transform.localPosition = offset;

    Canvas canvas = canvasObj.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.WorldSpace;
    canvas.sortingOrder = 1;

    CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
    scaler.dynamicPixelsPerUnit = 10f;

    RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
    canvasRect.sizeDelta = new Vector2( barWidth, barHeight );
    canvasRect.localScale = Vector3.one * 0.01f;

    // Background (dark)
    GameObject bgObj = new GameObject( "Background" );
    bgObj.transform.SetParent( canvasObj.transform, false );
    Image bgImage = bgObj.AddComponent<Image>();
    bgImage.color = new Color( 0.15f, 0.15f, 0.15f, 0.9f );
    RectTransform bgRect = bgObj.GetComponent<RectTransform>();
    bgRect.anchorMin = Vector2.zero;
    bgRect.anchorMax = Vector2.one;
    bgRect.offsetMin = Vector2.zero;
    bgRect.offsetMax = Vector2.zero;

    // Fill (red)
    GameObject fillObj = new GameObject( "Fill" );
    fillObj.transform.SetParent( canvasObj.transform, false );
    Image fillImage = fillObj.AddComponent<Image>();
    fillImage.color = Color.red;
    fillRect = fillObj.GetComponent<RectTransform>();
    fillRect.anchorMin = Vector2.zero;
    fillRect.anchorMax = Vector2.one;
    fillRect.pivot = new Vector2( 0f, 0.5f );
    fillRect.offsetMin = Vector2.zero;
    fillRect.offsetMax = Vector2.zero;

    canvasTransform = canvasObj.transform;
  }

  private void LateUpdate()
  {
    if( npcController == null || mainCamera == null || fillRect == null )
    {
      return;
    }

    // Update fill
    float healthPercent = Mathf.Clamp01( npcController.NPCHealthPoints / maxHealth );
    fillRect.anchorMax = new Vector2( healthPercent, 1f );

    // Face the camera
    canvasTransform.forward = mainCamera.transform.forward;
  }
}
