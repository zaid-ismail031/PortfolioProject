namespace Models.BehaviourTree.Actions
{
  public class HealAllyAction : Task
  {
    private readonly NPCController npc;
    private readonly SquadBlackboard blackboard;

    public HealAllyAction( NPCController npc, SquadBlackboard blackboard )
    {
      this.npc = npc;
      this.blackboard = blackboard;
    }

    public override Status Run()
    {
      NPCController target = blackboard.MostDamagedAlly;
      if( target == null || target == npc )
      {
        return Status.Success;
      }

      npc.HealAlly( target );
      return Status.Running;
    }
  }
}
