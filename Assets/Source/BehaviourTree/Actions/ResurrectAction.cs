using Models.BehaviourTree;
using UnityEngine;

public class ResurrectAction : Task
{
  private readonly NPCController npc;

  public ResurrectAction( NPCController npc )
  {
    this.npc = npc;
  }

  public override Status Run()
  {
    npc.Revive();
    return Status.Running;
  }
}
