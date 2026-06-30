using UnityEngine;

public class QuestTracker : MonoBehaviour
{
  [Header( "References" )]
  [SerializeField]
  private QuestHUD QuestHud;

  private bool step1Triggered;
  private bool step2Triggered;
  private bool talkedToQuestGiver;

  private void Update()
  {
    if( QuestHud == null )
    {
      return;
    }

    GeneratedQuest quest = QuestHud.GetActiveQuest();
    if( quest == null || quest.IsCompleted() )
    {
      return;
    }

    if( !quest.Step1Completed && !step1Triggered )
    {
      if( talkedToQuestGiver )
      {
        step1Triggered = true;
        QuestHud.CompleteCurrentStep();
      }
    }
    else if( quest.Step1Completed && !quest.Step2Completed && !step2Triggered )
    {
      if( IsTargetNPCDead( quest ) )
      {
        step2Triggered = true;
        QuestHud.CompleteCurrentStep();
      }
    }
  }

  public void OnTalkedToQuestGiver()
  {
    talkedToQuestGiver = true;
  }

  private bool IsTargetNPCDead( GeneratedQuest quest )
  {
    if( quest.TargetNPC == null )
    {
      return false;
    }

    NPCController controller = quest.TargetNPC.GetComponent<NPCController>();
    if( controller != null && controller.IsDead )
    {
      return true;
    }

    return false;
  }

  public void ResetTracking()
  {
    step1Triggered = false;
    step2Triggered = false;
    talkedToQuestGiver = false;
  }
}
