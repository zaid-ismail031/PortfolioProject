namespace Models.BehaviourTree
{
  public class Inverter : Task
  {
    private readonly Task child;

    public Inverter( Task child )
    {
      this.child = child;
    }

    public override Status Run()
    {
      Status result = child.Run();
      switch( result )
      {
        case Status.Success:
          return Status.Failure;
        case Status.Failure:
          return Status.Success;
        default:
          return Status.Running;
      }
    }
  }
}
