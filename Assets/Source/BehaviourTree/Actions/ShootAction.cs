namespace Models.BehaviourTree.Actions
{
  public class ShootAction : Task
  {
    private readonly NPCController npc;

    public ShootAction( NPCController npc )
    {
      this.npc = npc;
    }

    public override Status Run()
    {
      npc.Shoot();
      return Status.Running;
    }
  }
}
