namespace Models.BehaviourTree
{
  public class RunUntilSuccess : Task
  {
    private readonly Task child;

    public RunUntilSuccess( Task child )
    {
      this.child = child;
    }

    public override Status Run()
    {
      Status result = child.Run();

      if( result == Status.Failure )
      {
        return Status.Running;
      }

      return result;
    }
  }
}
