namespace Models.BehaviourTree.Actions
{
  public class PatrolAction : Task
  {
    private readonly NPCController npc;

    public PatrolAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.Patrol();
      return Status.Running;
    }
  }
}
