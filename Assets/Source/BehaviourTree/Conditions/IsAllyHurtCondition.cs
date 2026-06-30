namespace Models.BehaviourTree.Conditions
{
  public class IsAllyHurtCondition : Task
  {
    private readonly SquadBlackboard blackboard;

    public IsAllyHurtCondition( SquadBlackboard blackboard )
    {
      this.blackboard = blackboard;
    }

    public override Status Run()
    {
      return blackboard.IsAnyAllyHurt ? Status.Success : Status.Failure;
    }
  }
}
