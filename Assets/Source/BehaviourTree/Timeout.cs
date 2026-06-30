using UnityEngine;

namespace Models.BehaviourTree
{
  public class Timeout : Task
  {
    private readonly Task child;
    private readonly float timeoutDuration;
    private float startTime;
    private bool isRunning;

    public Timeout( Task child, float timeoutDuration )
    {
      this.child = child;
      this.timeoutDuration = timeoutDuration;
      isRunning = false;
    }

    public override Status Run()
    {
      if( !isRunning )
      {
        startTime = Time.time;
        isRunning = true;
      }

      if( Time.time - startTime > timeoutDuration )
      {
        isRunning = false;
        return Status.Failure;
      }

      Status result = child.Run();

      if( result != Status.Running )
      {
        isRunning = false;
      }

      return result;
    }
  }
}
