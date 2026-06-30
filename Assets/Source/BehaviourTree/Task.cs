using UnityEngine;

namespace Models.BehaviourTree
{
  public enum Status
  {
    Success,
    Failure,
    Running
  }

  public abstract class Task
  {
    public abstract Status Run();
  }
}

