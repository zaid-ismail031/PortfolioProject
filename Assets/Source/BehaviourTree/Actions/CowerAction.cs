using UnityEngine;

namespace Models.BehaviourTree.Actions
{
  public class CowerAction : Task
  {
    private readonly NPCController npc;
    private readonly float cowerDuration;
    private float startTime;
    private bool isCowering;

    public CowerAction( NPCController npc, float cowerDuration )
    {
      this.npc = npc;
      this.cowerDuration = cowerDuration;
      isCowering = false;
    }

    public override Status Run()
    {
      if( !isCowering )
      {
        npc.Cower( true );
        startTime = Time.time;
        isCowering = true;
      }

      if( Time.time - startTime >= cowerDuration )
      {
        npc.Cower( false );
        isCowering = false;
        return Status.Success;
      }

      return Status.Running;
    }
  }
}
