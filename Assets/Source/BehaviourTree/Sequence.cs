using System.Collections.Generic;

namespace Models.BehaviourTree
{
  public class Sequence : Task
  {
    private readonly List<Task> children;

    public Sequence( List<Task> children )
    {
      this.children = children;
    }

    public override Status Run()
    {
      foreach( Task child in children )
      {
        Status result = child.Run();
        if( result != Status.Success )
        {
          return result;
        }
      }
      return Status.Success;
    }
  }
}
