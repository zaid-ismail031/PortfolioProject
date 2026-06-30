using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
  TalkAndKill,
  FetchAndDeliver,
  ExploreAndReport
}

[System.Serializable]
public class GeneratedQuest
{
  public string QuestName { get; private set; }
  public string Step1Description { get; private set; }
  public string Step2Description { get; private set; }
  public QuestType Type { get; private set; }
  public int Step1RoomIndex { get; private set; }
  public int Step2RoomIndex { get; private set; }
  public int Difficulty { get; private set; }
  public int ChainOrder { get; private set; }
  public bool Step1Completed { get; private set; }
  public bool Step2Completed { get; private set; }
  public GameObject TargetNPC { get; private set; }

  public GeneratedQuest( string questName, string step1Description, string step2Description, QuestType type, int step1RoomIndex, int step2RoomIndex, int difficulty, int chainOrder, GameObject targetNPC )
  {
    QuestName = questName;
    Step1Description = step1Description;
    Step2Description = step2Description;
    Type = type;
    Step1RoomIndex = step1RoomIndex;
    Step2RoomIndex = step2RoomIndex;
    Difficulty = difficulty;
    ChainOrder = chainOrder;
    Step1Completed = false;
    Step2Completed = false;
    TargetNPC = targetNPC;
  }

  public void CompleteStep1()
  {
    Step1Completed = true;
  }

  public void CompleteStep2()
  {
    Step2Completed = true;
  }

  public bool IsCompleted()
  {
    return Step1Completed && Step2Completed;
  }

  public int GetCurrentStep()
  {
    if( !Step1Completed )
    {
      return 0;
    }

    if( !Step2Completed )
    {
      return 1;
    }

    return 2;
  }
}

[System.Serializable]
public class GeneratedNPC
{
  public string FullName { get; private set; }
  public string Role { get; private set; }
  public string Backstory { get; private set; }
  public string[] Traits { get; private set; }
  public int RoomIndex { get; private set; }

  public GeneratedNPC( string fullName, string role, string backstory, string[] traits, int roomIndex )
  {
    FullName = fullName;
    Role = role;
    Backstory = backstory;
    Traits = traits;
    RoomIndex = roomIndex;
  }
}

[System.Serializable]
public class EnvironmentalNote
{
  public string Text { get; private set; }
  public int RoomIndex { get; private set; }

  public EnvironmentalNote( string text, int roomIndex )
  {
    Text = text;
    RoomIndex = roomIndex;
  }
}

public class NarrativeGenerator : MonoBehaviour
{
  [Header( "References" )]
  [SerializeField]
  private DungeonGenerator DungeonGen;
  [SerializeField]
  private NarrativeRules Rules;

  [Header( "NPC Settings" )]
  [SerializeField]
  [Range( 1, 20 )]
  private int NPCCount = 5;
  [SerializeField]
  [Range( 1, 5 )]
  private int TraitsPerNPC = 3;

  [Header( "Quest Settings" )]
  [SerializeField]
  [Range( 1, 10 )]
  private int QuestChainLength = 4;

  [Header( "Environmental Storytelling" )]
  [SerializeField]
  [Range( 0, 20 )]
  private int EnvironmentalNoteCount = 5;

  [Header( "Generation" )]
  [SerializeField]
  private int Seed;
  [SerializeField]
  private bool RandomizeSeed = true;
  [SerializeField]
  private bool GenerateOnStart = true;

  private List<GeneratedNPC> generatedNPCs;
  private List<GeneratedQuest> questChain;
  private List<EnvironmentalNote> environmentalNotes;
  private System.Random random;

  private void Start()
  {
    if( RandomizeSeed )
    {
      Seed = Random.Range( 0, 100000 );
    }

    if( GenerateOnStart )
    {
      Generate();
    }
  }

  [ContextMenu( "Generate Narrative" )]
  public void Generate()
  {
    if( DungeonGen == null || Rules == null )
    {
      return;
    }

    random = new System.Random( Seed );
    generatedNPCs = new List<GeneratedNPC>();
    questChain = new List<GeneratedQuest>();
    environmentalNotes = new List<EnvironmentalNote>();

    List<RectInt> rooms = DungeonGen.GetRooms();
    if( rooms == null || rooms.Count == 0 )
    {
      return;
    }

    GenerateNPCs( rooms );
    GenerateQuestChain( rooms );
    GenerateEnvironmentalNotes( rooms );
    LogGeneratedContent();
  }

  private void GenerateNPCs( List<RectInt> rooms )
  {
    int count = Mathf.Min( NPCCount, rooms.Count );
    List<int> availableRooms = new List<int>();
    for( int i = 0; i < rooms.Count; i++ )
    {
      availableRooms.Add( i );
    }

    List<string> usedNames = new List<string>();

    for( int i = 0; i < count; i++ )
    {
      int roomListIndex = random.Next( availableRooms.Count );
      int roomIndex = availableRooms[ roomListIndex ];
      availableRooms.RemoveAt( roomListIndex );

      string fullName = GenerateUniqueName( usedNames );
      usedNames.Add( fullName );

      string role = Rules.GetRandomRole( random );
      string[] traits = GenerateTraits();
      string backstory = GenerateBackstory( fullName, role );

      GeneratedNPC npc = new GeneratedNPC( fullName, role, backstory, traits, roomIndex );
      generatedNPCs.Add( npc );
    }
  }

  private string GenerateUniqueName( List<string> usedNames )
  {
    int maxAttempts = 50;
    for( int i = 0; i < maxAttempts; i++ )
    {
      string title = Rules.GetRandomTitle( random );
      string firstName = Rules.GetRandomFirstName( random );
      string fullName = title + " " + firstName;

      if( !usedNames.Contains( fullName ) )
      {
        return fullName;
      }
    }

    return Rules.GetRandomTitle( random ) + " " + Rules.GetRandomFirstName( random );
  }

  private string[] GenerateTraits()
  {
    int count = Mathf.Min( TraitsPerNPC, Rules.GetTraitCount() );
    List<string> selectedTraits = new List<string>();

    while( selectedTraits.Count < count )
    {
      string trait = Rules.GetRandomTrait( random );
      if( !selectedTraits.Contains( trait ) )
      {
        selectedTraits.Add( trait );
      }
    }

    return selectedTraits.ToArray();
  }

  private string GenerateBackstory( string npcName, string role )
  {
    string template = Rules.GetRandomBackstoryTemplate( random );
    return ResolveTokens( template, npcName );
  }

  private void GenerateQuestChain( List<RectInt> rooms )
  {
  }

  public void GenerateQuestFromSpawnedNPCs( List<GameObject> spawnedNPCs )
  {
    if( spawnedNPCs == null || spawnedNPCs.Count == 0 )
    {
      return;
    }

    if( random == null )
    {
      random = new System.Random( Seed );
    }

    questChain = new List<GeneratedQuest>();

    string questGiverName = "";
    List<GameObject> killTargets = new List<GameObject>();

    foreach( GameObject npcObj in spawnedNPCs )
    {
      if( npcObj == null )
      {
        continue;
      }

      PCGNPCBehaviourTree bt = npcObj.GetComponent<PCGNPCBehaviourTree>();
      NPCBackstoryLabel label = npcObj.GetComponent<NPCBackstoryLabel>();

      if( bt == null || label == null )
      {
        continue;
      }

      if( bt.Role == PCGNPCRole.QuestGiver )
      {
        questGiverName = label.NPCName;
      }
      else
      {
        killTargets.Add( npcObj );
      }
    }

    if( killTargets.Count == 0 )
    {
      return;
    }

    GameObject targetNPC = killTargets[ random.Next( killTargets.Count ) ];
    NPCBackstoryLabel targetLabel = targetNPC.GetComponent<NPCBackstoryLabel>();
    string targetName = targetLabel.NPCName;
    string targetRoom = targetNPC.name;
    int roomIndex = -1;

    int roomStart = targetRoom.IndexOf( "Room" );
    if( roomStart >= 0 )
    {
      string roomNumber = targetRoom.Substring( roomStart + 4 );
      int.TryParse( roomNumber, out roomIndex );
    }

    GeneratedQuest quest = new GeneratedQuest(
      "The Quest",
      "Talk to " + questGiverName + " in Room 0",
      "Kill " + targetName + " in Room " + roomIndex,
      QuestType.TalkAndKill,
      0,
      roomIndex,
      1,
      1,
      targetNPC
    );

    questChain.Add( quest );
  }

  private string GetNPCNameForQuest( int roomIndex )
  {
    foreach( GeneratedNPC npc in generatedNPCs )
    {
      if( npc.RoomIndex == roomIndex )
      {
        return npc.FullName;
      }
    }

    if( generatedNPCs.Count > 0 )
    {
      return generatedNPCs[ random.Next( generatedNPCs.Count ) ].FullName;
    }

    return Rules.GetRandomTitle( random ) + " " + Rules.GetRandomFirstName( random );
  }

  private List<int> GetRoomsSortedByDifficulty( List<RectInt> rooms )
  {
    List<int> indices = new List<int>();
    for( int i = 0; i < rooms.Count; i++ )
    {
      indices.Add( i );
    }

    indices.Sort( ( a, b ) => DungeonGen.GetRoomDifficulty( a ).CompareTo( DungeonGen.GetRoomDifficulty( b ) ) );
    return indices;
  }

  private void GenerateEnvironmentalNotes( List<RectInt> rooms )
  {
    int count = Mathf.Min( EnvironmentalNoteCount, rooms.Count );
    List<int> availableRooms = new List<int>();
    for( int i = 0; i < rooms.Count; i++ )
    {
      availableRooms.Add( i );
    }

    for( int i = 0; i < count; i++ )
    {
      int roomListIndex = random.Next( availableRooms.Count );
      int roomIndex = availableRooms[ roomListIndex ];
      availableRooms.RemoveAt( roomListIndex );

      string noteText = ResolveTokens( Rules.GetRandomEnvironmentalNote( random ), null );
      EnvironmentalNote note = new EnvironmentalNote( noteText, roomIndex );
      environmentalNotes.Add( note );
    }
  }

  private string ResolveTokens( string template, string npcName )
  {
    string result = template;
    result = result.Replace( "{enemy}", Rules.GetRandomEnemyName( random ) );
    result = result.Replace( "{item}", Rules.GetRandomItemName( random ) );
    result = result.Replace( "{location}", Rules.GetRandomLocationName( random ) );
    result = result.Replace( "{role}", Rules.GetRandomRole( random ) );

    if( npcName != null )
    {
      result = result.Replace( "{npc}", npcName );
      result = result.Replace( "{name}", npcName );
    }
    else
    {
      string fallbackName = Rules.GetRandomTitle( random ) + " " + Rules.GetRandomFirstName( random );
      result = result.Replace( "{npc}", fallbackName );
      result = result.Replace( "{name}", fallbackName );
    }

    return result;
  }

  private void LogGeneratedContent()
  {
    Debug.Log( "=== GENERATED NARRATIVE ===" );

    Debug.Log( "--- NPCs ---" );
    foreach( GeneratedNPC npc in generatedNPCs )
    {
      Debug.Log( "Name: " + npc.FullName + " | Role: " + npc.Role + " | Room: " + npc.RoomIndex );
      Debug.Log( "Traits: " + string.Join( ", ", npc.Traits ) );
      Debug.Log( "Backstory: " + npc.Backstory );
    }

    Debug.Log( "--- Quest Chain ---" );
    foreach( GeneratedQuest quest in questChain )
    {
      Debug.Log( "Quest " + quest.ChainOrder + ": " + quest.QuestName + " (" + quest.Type + ", Difficulty " + quest.Difficulty + ")" );
      Debug.Log( "  Step 1 (Room " + quest.Step1RoomIndex + "): " + quest.Step1Description );
      Debug.Log( "  Step 2 (Room " + quest.Step2RoomIndex + "): " + quest.Step2Description );
    }

    Debug.Log( "--- Environmental Notes ---" );
    foreach( EnvironmentalNote note in environmentalNotes )
    {
      Debug.Log( "Room " + note.RoomIndex + ": " + note.Text );
    }
  }

  public List<GeneratedNPC> GetGeneratedNPCs()
  {
    return generatedNPCs;
  }

  public List<GeneratedQuest> GetQuestChain()
  {
    return questChain;
  }

  public List<EnvironmentalNote> GetEnvironmentalNotes()
  {
    return environmentalNotes;
  }

  public void SetRules( NarrativeRules rules )
  {
    Rules = rules;
  }
}
