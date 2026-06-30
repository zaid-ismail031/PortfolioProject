namespace Models.BehaviourTree
{
  public interface IBehaviourTree
  {
    Task RootTask { get; }
    void Tick();
  }
}
