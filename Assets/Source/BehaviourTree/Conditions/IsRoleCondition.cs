namespace Models.BehaviourTree.Conditions
{
  public class IsRoleCondition : Task
  {
    private readonly NPCController npc;
    private readonly SquadBlackboard blackboard;
    private readonly SquadRole expectedRole;

    public IsRoleCondition( NPCController npc, SquadBlackboard blackboard, SquadRole expectedRole )
    {
      this.npc = npc;
      this.blackboard = blackboard;
      this.expectedRole = expectedRole;
    }

    public override Status Run()
    {
      return blackboard.GetRole( npc ) == expectedRole ? Status.Success : Status.Failure;
    }
  }
}
