using UnityEngine;

namespace Models.BehaviourTree
{
  public class Retry : Task
  {
    private readonly Task child;
    private readonly int maxRetries;
    private readonly float delayBetweenRetries;
    private int currentRetry;
    private float retryStartTime;
    private bool isWaiting;

    public Retry( Task child, int maxRetries, float delayBetweenRetries = 0f )
    {
      this.child = child;
      this.maxRetries = maxRetries;
      this.delayBetweenRetries = delayBetweenRetries;
      currentRetry = 0;
      isWaiting = false;
    }

    public override Status Run()
    {
      if( isWaiting )
      {
        if( Time.time - retryStartTime < delayBetweenRetries )
        {
          return Status.Running;
        }
        isWaiting = false;
      }

      Status result = child.Run();

      if( result == Status.Success )
      {
        currentRetry = 0;
        return Status.Success;
      }

      if( result == Status.Running )
      {
        return Status.Running;
      }

      currentRetry++;
      if( currentRetry > maxRetries )
      {
        currentRetry = 0;
        return Status.Failure;
      }

      if( delayBetweenRetries > 0f )
      {
        retryStartTime = Time.time;
        isWaiting = true;
        return Status.Running;
      }

      return Status.Running;
    }
  }
}
