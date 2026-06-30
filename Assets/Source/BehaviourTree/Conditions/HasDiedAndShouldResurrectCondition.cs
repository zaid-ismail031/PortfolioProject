using Models.BehaviourTree;
using UnityEngine;

public class HasDiedAndShouldResurrectCondition : Task
{
  private readonly NPCController npcController;

  public HasDiedAndShouldResurrectCondition( NPCController npcController )
  {
    this.npcController = npcController;
  }

  public override Status Run()
  {
    return npcController.IsDead && npcController.NPCHealthPoints > 0f ? Status.Success : Status.Failure;
  }
}
