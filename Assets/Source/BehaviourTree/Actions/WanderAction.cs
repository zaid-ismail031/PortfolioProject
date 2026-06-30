namespace Models.BehaviourTree.Actions
{
  public class WanderAction : Task
  {
    private readonly NPCController npc;

    public WanderAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.Wander();
      return Status.Running;
    }
  }
}
