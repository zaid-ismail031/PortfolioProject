using UnityEngine;

namespace Models.BehaviourTree
{
  public class Cooldown : Task
  {
    private readonly Task child;
    private readonly float cooldownDuration;
    private float lastSuccessTime;

    public Cooldown( Task child, float cooldownDuration )
    {
      this.child = child;
      this.cooldownDuration = cooldownDuration;
      lastSuccessTime = -cooldownDuration;
    }

    public override Status Run()
    {
      if( Time.time - lastSuccessTime < cooldownDuration )
      {
        return Status.Failure;
      }

      Status result = child.Run();

      if( result == Status.Success )
      {
        lastSuccessTime = Time.time;
      }

      return result;
    }
  }
}
