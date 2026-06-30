using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionHUD : MonoBehaviour
{
  [Header( "Interaction" )]
  [SerializeField]
  private float InteractionRange = 5f;
  [SerializeField]
  private KeyCode InteractKey = KeyCode.E;

  [Header( "Prompt" )]
  [SerializeField]
  private int PromptFontSize = 18;
  [SerializeField]
  private Color PromptColor = new Color( 1f, 1f, 1f, 0.8f );

  [Header( "Quest HUD" )]
  [SerializeField]
  private QuestHUD QuestHud;
  [SerializeField]
  private QuestTracker QuestTracker;

  [Header( "Backstory Panel" )]
  [SerializeField]
  private Canvas BackstoryCanvas;
  [SerializeField]
  private int TitleFontSize = 22;
  [SerializeField]
  private int BodyFontSize = 16;
  [SerializeField]
  private float PanelWidth = 400f;
  [SerializeField]
  private float PanelPadding = 15f;
  [SerializeField]
  private Color PanelColor = new Color( 0f, 0f, 0f, 0.8f );
  [SerializeField]
  private Color TitleColor = new Color( 1f, 0.85f, 0.3f );
  [SerializeField]
  private Color BodyColor = Color.white;

  private Text promptText;
  private GameObject backstoryPanel;
  private Text backstoryTitle;
  private Text backstoryBody;
  private Camera mainCamera;
  private NPCBackstoryLabel currentTarget;
  private bool isShowing;

  private void Start()
  {
    mainCamera = Camera.main;
    CreatePrompt();
    CreateBackstoryPanel();
  }

  private void Update()
  {
    NPCBackstoryLabel lookTarget = GetLookTarget();

    if( isShowing )
    {
      if( Input.GetKeyDown( InteractKey ) || lookTarget != currentTarget )
      {
        HideBackstory();
      }

      return;
    }

    if( lookTarget != null )
    {
      currentTarget = lookTarget;
      promptText.text = "[E] " + currentTarget.NPCName;
      promptText.enabled = true;

      if( Input.GetKeyDown( InteractKey ) )
      {
        ShowBackstory();
      }
    }
    else
    {
      currentTarget = null;
      promptText.enabled = false;
    }
  }

  private NPCBackstoryLabel GetLookTarget()
  {
    if( mainCamera == null )
    {
      return null;
    }

    int npcLayer = LayerMask.GetMask( "NPC" );
    Ray ray = new Ray( mainCamera.transform.position, mainCamera.transform.forward );
    if( Physics.SphereCast( ray, 1.0f, out RaycastHit hit, InteractionRange, npcLayer ) )
    {
      NPCBackstoryLabel label = hit.collider.GetComponentInParent<NPCBackstoryLabel>();
      return label;
    }

    return null;
  }

  private void ShowBackstory()
  {
    if( currentTarget == null )
    {
      return;
    }

    isShowing = true;
    promptText.enabled = false;
    backstoryPanel.SetActive( true );
    backstoryTitle.text = currentTarget.RoleName.ToUpper() + " - " + currentTarget.NPCName;
    backstoryBody.text = currentTarget.Backstory;

    PCGNPCBehaviourTree behaviourTree = currentTarget.GetComponent<PCGNPCBehaviourTree>();
    if( behaviourTree != null && behaviourTree.Role == PCGNPCRole.QuestGiver )
    {
      if( QuestTracker != null )
      {
        QuestTracker.OnTalkedToQuestGiver();
      }
    }
  }

  private void HideBackstory()
  {
    isShowing = false;
    backstoryPanel.SetActive( false );
  }

  private void CreatePrompt()
  {
    GameObject promptObj = new GameObject( "InteractPrompt" );
    promptObj.transform.SetParent( BackstoryCanvas.transform, false );

    promptText = promptObj.AddComponent<Text>();
    promptText.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    promptText.fontSize = PromptFontSize;
    promptText.color = PromptColor;
    promptText.alignment = TextAnchor.MiddleCenter;
    promptText.enabled = false;

    RectTransform promptRect = promptObj.GetComponent<RectTransform>();
    promptRect.anchorMin = new Vector2( 0.5f, 0.3f );
    promptRect.anchorMax = new Vector2( 0.5f, 0.3f );
    promptRect.pivot = new Vector2( 0.5f, 0.5f );
    promptRect.sizeDelta = new Vector2( 300f, 40f );
  }

  private void CreateBackstoryPanel()
  {
    backstoryPanel = new GameObject( "BackstoryPanel" );
    backstoryPanel.transform.SetParent( BackstoryCanvas.transform, false );

    Image panelImage = backstoryPanel.AddComponent<Image>();
    panelImage.color = PanelColor;

    RectTransform panelRect = backstoryPanel.GetComponent<RectTransform>();
    panelRect.anchorMin = new Vector2( 0.5f, 0.5f );
    panelRect.anchorMax = new Vector2( 0.5f, 0.5f );
    panelRect.pivot = new Vector2( 0.5f, 0.5f );
    panelRect.sizeDelta = new Vector2( PanelWidth, 0f );

    VerticalLayoutGroup layout = backstoryPanel.AddComponent<VerticalLayoutGroup>();
    layout.padding = new RectOffset(
      (int)PanelPadding,
      (int)PanelPadding,
      (int)PanelPadding,
      (int)PanelPadding
    );
    layout.spacing = 8f;
    layout.childControlWidth = true;
    layout.childControlHeight = true;
    layout.childForceExpandWidth = true;
    layout.childForceExpandHeight = false;

    ContentSizeFitter fitter = backstoryPanel.AddComponent<ContentSizeFitter>();
    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

    GameObject titleObj = new GameObject( "Title" );
    titleObj.transform.SetParent( backstoryPanel.transform, false );
    backstoryTitle = titleObj.AddComponent<Text>();
    backstoryTitle.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    backstoryTitle.fontSize = TitleFontSize;
    backstoryTitle.color = TitleColor;
    backstoryTitle.fontStyle = FontStyle.Bold;
    backstoryTitle.alignment = TextAnchor.MiddleCenter;

    GameObject bodyObj = new GameObject( "Body" );
    bodyObj.transform.SetParent( backstoryPanel.transform, false );
    backstoryBody = bodyObj.AddComponent<Text>();
    backstoryBody.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    backstoryBody.fontSize = BodyFontSize;
    backstoryBody.color = BodyColor;
    backstoryBody.alignment = TextAnchor.UpperLeft;
    backstoryBody.horizontalOverflow = HorizontalWrapMode.Wrap;

    backstoryPanel.SetActive( false );
  }
}
