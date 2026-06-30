namespace Models.BehaviourTree.Conditions
{
  public class IsSquadRetreatingCondition : Task
  {
    private readonly SquadBlackboard blackboard;

    public IsSquadRetreatingCondition( SquadBlackboard blackboard )
    {
      this.blackboard = blackboard;
    }

    public override Status Run()
    {
      return blackboard.CurrentState == SquadState.Retreating ? Status.Success : Status.Failure;
    }
  }
}
