namespace Models.BehaviourTree.Conditions
{
  public class IsDeadCondition : Task
  {
    private readonly NPCController npc;

    public IsDeadCondition( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      return npc.NPCHealthPoints <= 0f ? Status.Success : Status.Failure;
    }
  }
}
