using System.Collections.Generic;

namespace Models.BehaviourTree
{
  public class Selector : Task
  {
    private readonly List<Task> children;

    public Selector( List<Task> children )
    {
      this.children = children;
    }

    public override Status Run()
    {
      foreach( Task child in children )
      {
        Status result = child.Run();
        if( result != Status.Failure )
        {
          return result;
        }
      }
      return Status.Failure;
    }
  }
}
