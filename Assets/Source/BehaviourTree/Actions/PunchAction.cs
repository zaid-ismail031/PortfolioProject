namespace Models.BehaviourTree.Actions
{
  public class PunchAction : Task
  {
    private readonly NPCController npc;

    public PunchAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.Punch();
      return Status.Running;
    }
  }
}
