namespace Models.BehaviourTree.Conditions
{
  public class IsPlayerVisibleCondition : Task
  {
    private readonly SquadBlackboard blackboard;

    public IsPlayerVisibleCondition( SquadBlackboard blackboard )
    {
      this.blackboard = blackboard;
    }

    public override Status Run()
    {
      return blackboard.IsPlayerVisible ? Status.Success : Status.Failure;
    }
  }
}
