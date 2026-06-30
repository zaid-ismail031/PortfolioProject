namespace Models.BehaviourTree.Conditions
{
  public class IsHealthLowCondition : Task
  {
    private readonly NPCController npc;
    private readonly float healthThreshold;

    public IsHealthLowCondition( NPCController npc, float healthThreshold )
    {
      this.npc = npc;
      this.healthThreshold = healthThreshold;
    }

    public override Status Run()
    {
      return npc.NPCHealthPoints <= healthThreshold ? Status.Success : Status.Failure;
    }
  }
}
