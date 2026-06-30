namespace Models.BehaviourTree.Actions
{
  public class ChaseAction : Task
  {
    private readonly NPCController npc;

    public ChaseAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.Chase();
      return Status.Running;
    }
  }
}
