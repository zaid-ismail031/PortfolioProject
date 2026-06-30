namespace Models.BehaviourTree.Conditions
{
  public class IsSquadEngagedCondition : Task
  {
    private readonly SquadBlackboard blackboard;

    public IsSquadEngagedCondition( SquadBlackboard blackboard )
    {
      this.blackboard = blackboard;
    }

    public override Status Run()
    {
      return blackboard.CurrentState == SquadState.Engaged ? Status.Success : Status.Failure;
    }
  }
}
