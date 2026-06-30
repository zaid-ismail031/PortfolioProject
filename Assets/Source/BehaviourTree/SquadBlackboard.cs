using System.Collections.Generic;
using UnityEngine;

namespace Models.BehaviourTree
{
  public enum SquadState
  {
    Idle,
    Engaged,
    Retreating
  }

  public enum SquadRole
  {
    None,
    Rusher,
    Flanker,
    Vantage,
    Healer
  }

  public class SquadBlackboard
  {
    public SquadState CurrentState { get; set; }
    public Vector3 PlayerLastKnownPosition { get; set; }
    public float PlayerLastSeenTime { get; set; }
    public bool IsPlayerVisible { get; set; }
    public float ThreatLevel { get; set; }

    public Vector3 FlankPosition { get; set; }
    public Vector3 VantagePosition { get; set; }
    public bool IsVantageOccupied { get; set; }
    public bool IsFlankOccupied { get; set; }

    public Vector3 HealerHidePosition { get; set; }

    public float SquadHealthPercentage { get; set; }
    public NPCController MostDamagedAlly { get; set; }
    public bool IsAnyAllyHurt { get; set; }

    private Dictionary<NPCController, SquadRole> roles = new Dictionary<NPCController, SquadRole>();

    public void SetRole( NPCController npc, SquadRole role )
    {
      roles[ npc ] = role;
    }

    public SquadRole GetRole( NPCController npc )
    {
      if( roles.TryGetValue( npc, out SquadRole role ) )
      {
        return role;
      }
      return SquadRole.None;
    }

    public void ClearRoles()
    {
      roles.Clear();
    }

    public void UpdatePlayerPosition( Vector3 position )
    {
      PlayerLastKnownPosition = position;
      PlayerLastSeenTime = Time.time;
      IsPlayerVisible = true;
    }

    public float TimeSincePlayerSeen()
    {
      return Time.time - PlayerLastSeenTime;
    }
  }
}
