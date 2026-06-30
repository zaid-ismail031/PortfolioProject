namespace Models.BehaviourTree.Actions
{
  public class IdleAction : Task
  {
    private readonly NPCController npc;

    public IdleAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.Idle();
      return Status.Running;
    }
  }
}
