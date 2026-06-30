using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class QuestHUD : MonoBehaviour
{
  [Header( "References" )]
  [SerializeField]
  private NarrativeGenerator NarrativeGen;

  [Header( "Layout" )]
  [SerializeField]
  private Canvas Canvas;
  [SerializeField]
  private Vector2 PanelPosition = new Vector2( 20f, -20f );
  [SerializeField]
  private float PanelWidth = 350f;
  [SerializeField]
  private float PanelPadding = 10f;

  [Header( "Title" )]
  [SerializeField]
  private int TitleFontSize = 22;
  [SerializeField]
  private Color TitleColor = new Color( 1f, 0.85f, 0.3f );

  [Header( "Quest Text" )]
  [SerializeField]
  private int QuestFontSize = 16;
  [SerializeField]
  private int StepFontSize = 14;
  [SerializeField]
  private Color ActiveQuestColor = Color.white;
  [SerializeField]
  private Color CompletedStepColor = new Color( 0.5f, 0.5f, 0.5f );

  [Header( "Background" )]
  [SerializeField]
  private Color BackgroundColor = new Color( 0f, 0f, 0f, 0.6f );


  private GameObject panelObj;
  private Text titleText;
  private List<Text> questNameTexts;
  private List<Text> step1Texts;
  private List<Text> step2Texts;
  private GeneratedQuest activeQuest;

  private void Start()
  {
    questNameTexts = new List<Text>();
    step1Texts = new List<Text>();
    step2Texts = new List<Text>();
  }

  public void BuildHUD()
  {
    if( NarrativeGen == null )
    {
      return;
    }

    List<GeneratedQuest> quests = NarrativeGen.GetQuestChain();
    if( quests == null || quests.Count == 0 )
    {
      return;
    }

    ClearHUD();
    CreateCanvas();
    CreatePanel();
    CreateTitle();

    questNameTexts = new List<Text>();
    step1Texts = new List<Text>();
    step2Texts = new List<Text>();

    int i = Random.Range( 0, quests.Count );
    activeQuest = quests[ i ];
    CreateQuestEntry( activeQuest, i );

    UpdateQuestDisplay();
  }

  public void ShowHUD()
  {
    if( panelObj != null )
    {
      panelObj.SetActive( true );
    }
  }

  public void HideHUD()
  {
    if( panelObj != null )
    {
      panelObj.SetActive( false );
    }
  }

  public bool IsVisible()
  {
    return panelObj != null && panelObj.activeSelf;
  }

  private void CreateCanvas()
  {
    GameObject canvasObj = new GameObject( "QuestHUDCanvas" );
    canvasObj.transform.SetParent( transform );

    Canvas = canvasObj.AddComponent<Canvas>();
    Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    Canvas.sortingOrder = 10;

    CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    scaler.referenceResolution = new Vector2( 1920f, 1080f );

    canvasObj.AddComponent<GraphicRaycaster>();
  }

  private void CreatePanel()
  {
    panelObj = new GameObject( "QuestPanel" );
    panelObj.transform.SetParent( Canvas.transform, false );

    Image panelImage = panelObj.AddComponent<Image>();
    panelImage.color = BackgroundColor;

    RectTransform panelRect = panelObj.GetComponent<RectTransform>();
    panelRect.anchorMin = new Vector2( 0f, 1f );
    panelRect.anchorMax = new Vector2( 0f, 1f );
    panelRect.pivot = new Vector2( 0f, 1f );
    panelRect.anchoredPosition = PanelPosition;
    panelRect.sizeDelta = new Vector2( PanelWidth, 0f );

    VerticalLayoutGroup layout = panelObj.AddComponent<VerticalLayoutGroup>();
    layout.padding = new RectOffset(
      (int)PanelPadding,
      (int)PanelPadding,
      (int)PanelPadding,
      (int)PanelPadding
    );
    layout.spacing = 6f;
    layout.childControlWidth = true;
    layout.childControlHeight = true;
    layout.childForceExpandWidth = true;
    layout.childForceExpandHeight = false;

    ContentSizeFitter fitter = panelObj.AddComponent<ContentSizeFitter>();
    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
  }

  private void CreateTitle()
  {
    GameObject titleObj = new GameObject( "QuestTitle" );
    titleObj.transform.SetParent( panelObj.transform, false );

    titleText = titleObj.AddComponent<Text>();
    titleText.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    titleText.fontSize = TitleFontSize;
    titleText.color = TitleColor;
    titleText.fontStyle = FontStyle.Bold;
    titleText.text = "QUESTS";
  }

  private void CreateQuestEntry( GeneratedQuest quest, int index )
  {
    GameObject nameObj = new GameObject( "QuestName_" + index );
    nameObj.transform.SetParent( panelObj.transform, false );

    Text nameText = nameObj.AddComponent<Text>();
    nameText.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    nameText.fontSize = QuestFontSize;
    nameText.color = ActiveQuestColor;
    nameText.fontStyle = FontStyle.Bold;
    nameText.text = quest.ChainOrder + ". " + quest.QuestName;
    questNameTexts.Add( nameText );

    GameObject s1Obj = new GameObject( "QuestStep1_" + index );
    s1Obj.transform.SetParent( panelObj.transform, false );

    Text s1Text = s1Obj.AddComponent<Text>();
    s1Text.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    s1Text.fontSize = StepFontSize;
    s1Text.color = ActiveQuestColor;
    s1Text.text = "    a) " + quest.Step1Description;
    step1Texts.Add( s1Text );

    GameObject s2Obj = new GameObject( "QuestStep2_" + index );
    s2Obj.transform.SetParent( panelObj.transform, false );

    Text s2Text = s2Obj.AddComponent<Text>();
    s2Text.font = Resources.GetBuiltinResource<Font>( "LegacyRuntime.ttf" );
    s2Text.fontSize = StepFontSize;
    s2Text.color = ActiveQuestColor;
    s2Text.text = "    b) " + quest.Step2Description;
    step2Texts.Add( s2Text );
  }

  private void UpdateQuestDisplay()
  {
    if( activeQuest == null || questNameTexts.Count == 0 )
    {
      return;
    }

    if( activeQuest.IsCompleted() )
    {
      questNameTexts[ 0 ].color = CompletedStepColor;
      questNameTexts[ 0 ].fontStyle = FontStyle.Italic;
      step1Texts[ 0 ].color = CompletedStepColor;
      step1Texts[ 0 ].fontStyle = FontStyle.Italic;
      step2Texts[ 0 ].color = CompletedStepColor;
      step2Texts[ 0 ].fontStyle = FontStyle.Italic;
    }
    else
    {
      questNameTexts[ 0 ].color = TitleColor;
      questNameTexts[ 0 ].fontStyle = FontStyle.Bold;

      if( activeQuest.Step1Completed )
      {
        step1Texts[ 0 ].color = CompletedStepColor;
        step1Texts[ 0 ].fontStyle = FontStyle.Italic;
        step2Texts[ 0 ].color = TitleColor;
        step2Texts[ 0 ].fontStyle = FontStyle.Normal;
      }
      else
      {
        step1Texts[ 0 ].color = TitleColor;
        step1Texts[ 0 ].fontStyle = FontStyle.Normal;
        step2Texts[ 0 ].color = ActiveQuestColor;
        step2Texts[ 0 ].fontStyle = FontStyle.Normal;
      }
    }
  }

  public void CompleteCurrentStep()
  {
    if( activeQuest == null || activeQuest.IsCompleted() )
    {
      return;
    }

    if( !activeQuest.Step1Completed )
    {
      activeQuest.CompleteStep1();
    }
    else if( !activeQuest.Step2Completed )
    {
      activeQuest.CompleteStep2();
    }

    UpdateQuestDisplay();
  }

  public GeneratedQuest GetActiveQuest()
  {
    return activeQuest;
  }

  public bool IsQuestCompleted()
  {
    if( activeQuest == null )
    {
      return false;
    }

    return activeQuest.IsCompleted();
  }

  private void ClearHUD()
  {
    if( questNameTexts != null )
    {
      questNameTexts.Clear();
    }

    if( step1Texts != null )
    {
      step1Texts.Clear();
    }

    if( step2Texts != null )
    {
      step2Texts.Clear();
    }
  }
}
