namespace Models.BehaviourTree.Actions
{
  public class DieAction : Task
  {
    private readonly NPCController npc;

    public DieAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.Die();
      return Status.Running;
    }
  }
}
