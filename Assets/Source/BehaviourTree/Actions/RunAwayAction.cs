namespace Models.BehaviourTree.Actions
{
  public class RunAwayAction : Task
  {
    private readonly NPCController npc;

    public RunAwayAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.RunAway();
      return Status.Running;
    }
  }
}
