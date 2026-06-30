using UnityEngine;

public enum NarrativeTheme
{
  AncientRuins,
  Lab,
  Office
}

[CreateAssetMenu( fileName = "NewNarrativeRules", menuName = "PCG/Narrative Rules" )]
public class NarrativeRules : ScriptableObject
{
  [Header( "Theme Preset" )]
  [SerializeField]
  private NarrativeTheme ThemePreset;

  [Header( "NPC Names" )]
  [SerializeField]
  private string[] Titles;
  [SerializeField]
  private string[] FirstNames;

  [Header( "NPC Roles" )]
  [SerializeField]
  private string[] Roles;

  [Header( "NPC Traits" )]
  [SerializeField]
  private string[] Traits;

  [Header( "Backstory Templates" )]
  [SerializeField]
  [TextArea]
  private string[] BackstoryTemplates;

  [Header( "Vocabulary" )]
  [SerializeField]
  private string[] EnemyNames;
  [SerializeField]
  private string[] ItemNames;
  [SerializeField]
  private string[] LocationNames;

  [Header( "Quest Name Templates" )]
  [SerializeField]
  private string[] QuestNameTemplates;

  [Header( "Talk And Kill Templates" )]
  [SerializeField]
  [TextArea]
  private string[] TalkAndKillStep1;
  [SerializeField]
  [TextArea]
  private string[] TalkAndKillStep2;

  [Header( "Fetch And Deliver Templates" )]
  [SerializeField]
  [TextArea]
  private string[] FetchAndDeliverStep1;
  [SerializeField]
  [TextArea]
  private string[] FetchAndDeliverStep2;

  [Header( "Explore And Report Templates" )]
  [SerializeField]
  [TextArea]
  private string[] ExploreAndReportStep1;
  [SerializeField]
  [TextArea]
  private string[] ExploreAndReportStep2;

  [Header( "Environmental Storytelling" )]
  [SerializeField]
  [TextArea]
  private string[] EnvironmentalNoteTemplates;

  public void SetThemeAndApply( NarrativeTheme theme )
  {
    ThemePreset = theme;
  }

  public string GetRandomTitle( System.Random random )
  {
    return Titles[ random.Next( Titles.Length ) ];
  }

  public string GetRandomFirstName( System.Random random )
  {
    return FirstNames[ random.Next( FirstNames.Length ) ];
  }

  public string GetRandomRole( System.Random random )
  {
    return Roles[ random.Next( Roles.Length ) ];
  }

  public string GetRandomTrait( System.Random random )
  {
    return Traits[ random.Next( Traits.Length ) ];
  }

  public string GetRandomBackstoryTemplate( System.Random random )
  {
    return BackstoryTemplates[ random.Next( BackstoryTemplates.Length ) ];
  }

  public string GetRandomEnemyName( System.Random random )
  {
    return EnemyNames[ random.Next( EnemyNames.Length ) ];
  }

  public string GetRandomItemName( System.Random random )
  {
    return ItemNames[ random.Next( ItemNames.Length ) ];
  }

  public string GetRandomLocationName( System.Random random )
  {
    return LocationNames[ random.Next( LocationNames.Length ) ];
  }

  public string GetRandomQuestNameTemplate( System.Random random )
  {
    return QuestNameTemplates[ random.Next( QuestNameTemplates.Length ) ];
  }

  public string GetStep1Template( QuestType questType, System.Random random )
  {
    switch( questType )
    {
      case QuestType.TalkAndKill:
        return TalkAndKillStep1[ random.Next( TalkAndKillStep1.Length ) ];
      case QuestType.FetchAndDeliver:
        return FetchAndDeliverStep1[ random.Next( FetchAndDeliverStep1.Length ) ];
      case QuestType.ExploreAndReport:
        return ExploreAndReportStep1[ random.Next( ExploreAndReportStep1.Length ) ];
      default:
        return TalkAndKillStep1[ random.Next( TalkAndKillStep1.Length ) ];
    }
  }

  public string GetStep2Template( QuestType questType, System.Random random )
  {
    switch( questType )
    {
      case QuestType.TalkAndKill:
        return TalkAndKillStep2[ random.Next( TalkAndKillStep2.Length ) ];
      case QuestType.FetchAndDeliver:
        return FetchAndDeliverStep2[ random.Next( FetchAndDeliverStep2.Length ) ];
      case QuestType.ExploreAndReport:
        return ExploreAndReportStep2[ random.Next( ExploreAndReportStep2.Length ) ];
      default:
        return TalkAndKillStep2[ random.Next( TalkAndKillStep2.Length ) ];
    }
  }

  public string GetRandomEnvironmentalNote( System.Random random )
  {
    return EnvironmentalNoteTemplates[ random.Next( EnvironmentalNoteTemplates.Length ) ];
  }

  public int GetTraitCount()
  {
    return Traits.Length;
  }

  public int GetFirstNameCount()
  {
    return FirstNames.Length;
  }

  public NarrativeTheme GetTheme()
  {
    return ThemePreset;
  }
}
