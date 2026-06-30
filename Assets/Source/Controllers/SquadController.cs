using System.Collections.Generic;
using System.Linq;
using DIGA7009A.Tools;
using Models.BehaviourTree;
using UnityEngine;

public class SquadController : MonoBehaviour
{
  [SerializeField] private List<NPCController> squadMembers = new List<NPCController>();
  [SerializeField] private Transform playerTarget;
  [SerializeField] private Transform vantagePoint;
  [SerializeField] private Transform healerHidePoint;
  [SerializeField] private float activationRange = 20f;
  [SerializeField] private float flankDistance = 8f;
  [SerializeField] private float lowHealthThreshold = 50f;
  [SerializeField] private float playerLostTimeout = 5f;

  private SquadBlackboard blackboard;

  private Stopwatch FlankPositionUpdateTimer = new Stopwatch( 5.0f );

  public SquadBlackboard Blackboard => blackboard;

  private OnValueChangeCallback<bool> onPlayerSighted = null;

  private void Awake()
  {
    blackboard = new SquadBlackboard();
    blackboard.CurrentState = SquadState.Idle;
  }

  private void Start()
  {
    onPlayerSighted = new OnValueChangeCallback<bool>( false, AssignRoles );
  }

  private void FixedUpdate()
  {
    if( playerTarget == null )
    {
      return;
    }

    UpdatePlayerTracking();
    UpdateSquadHealth();
    UpdateSquadState();
    UpdateFlankPosition();
    UpdateVantagePosition();
    UpdateHealerHidePosition();

    FlankPositionUpdateTimer.DoLogic( Time.fixedDeltaTime );

    if( FlankPositionUpdateTimer.HasElapsed() )
    {
      FlankPositionUpdateTimer.Stop();
    }

    onPlayerSighted.Value = blackboard.IsPlayerVisible;
  }

  private void UpdatePlayerTracking()
  {
    float distanceToPlayer = Mathf.Infinity;
    foreach( NPCController enemy in squadMembers )
    {
      if( enemy == null || enemy.IsDead ) continue;
      float tempDistance = Vector3.Distance( enemy.transform.position, playerTarget.position );
      if( tempDistance < distanceToPlayer )
      {
        distanceToPlayer = tempDistance;
      }
    }

    if( distanceToPlayer <= activationRange )
    {
      blackboard.UpdatePlayerPosition( playerTarget.position );
    }
    else
    {
      if( blackboard.TimeSincePlayerSeen() > playerLostTimeout )
      {
        blackboard.IsPlayerVisible = false;
      }
    }
  }

  private void UpdateSquadHealth()
  {
    float totalHealth = 0f;
    float maxHealth = 0f;
    NPCController mostDamaged = null;
    float lowestHealth = float.MaxValue;
    bool anyHurt = false;

    foreach( NPCController member in squadMembers )
    {
      if( member == null )
      {
        continue;
      }

      totalHealth += member.NPCHealthPoints;
      maxHealth += 100f;

      if( member.NPCHealthPoints <= lowHealthThreshold )
      {
        anyHurt = true;
      }

      if( member.NPCHealthPoints < lowestHealth && member.NPCHealthPoints < 100f )
      {
        lowestHealth = member.NPCHealthPoints;
        mostDamaged = member;
      }
    }

    blackboard.SquadHealthPercentage = maxHealth > 0f ? totalHealth / maxHealth : 0f;
    blackboard.MostDamagedAlly = mostDamaged;
    blackboard.IsAnyAllyHurt = anyHurt;
  }

  private void UpdateSquadState()
  {
    if( !blackboard.IsPlayerVisible )
    {
      blackboard.CurrentState = SquadState.Idle;
      blackboard.ThreatLevel = 0f;
      return;
    }

    if( blackboard.SquadHealthPercentage < 0.3f )
    {
      blackboard.CurrentState = SquadState.Retreating;
      blackboard.ThreatLevel = 1f;
    }
    else
    {
      blackboard.CurrentState = SquadState.Engaged;
      blackboard.ThreatLevel = 1f - blackboard.SquadHealthPercentage;
    }
  }

  private void UpdateFlankPosition()
  {
    NPCController flanker = null;

    foreach( NPCController enemy in squadMembers )
    {
      if( blackboard.GetRole( enemy ) == SquadRole.Flanker )
      {
        flanker = enemy;
        break;
      }
    }

    if( flanker == null )
    {
      return;
    }

    if( FlankPositionUpdateTimer.Running )
    {
      return;
    }
    FlankPositionUpdateTimer.Start();
    Vector3 rightFlank = playerTarget.forward * -1.0f;
    blackboard.FlankPosition = blackboard.PlayerLastKnownPosition + rightFlank * flankDistance;
  }

  private void UpdateVantagePosition()
  {
    if( vantagePoint != null )
    {
      blackboard.VantagePosition = vantagePoint.position;
    }
  }


  private void UpdateHealerHidePosition()
  {
    if( healerHidePoint != null )
    {
      blackboard.HealerHidePosition = healerHidePoint.position;
    }
  }

  private void AssignRoles( bool value )
  {
    if( value )
    {
      List<NPCController> sorted = squadMembers
      .Where( npc => npc != null && !npc.IsDead && npc.NPCHealthPoints > 0 )
      .OrderBy( npc => Vector3.Distance( npc.transform.position, blackboard.PlayerLastKnownPosition ) )
      .ToList();

      blackboard.ClearRoles();
      blackboard.IsFlankOccupied = false;
      blackboard.IsVantageOccupied = false;

      for( int i = 0; i < sorted.Count; i++ )
      {
        switch( i )
        {
          case 0:
            blackboard.SetRole( sorted[ i ], SquadRole.Rusher );
            break;
          case 1:
            blackboard.SetRole( sorted[ i ], SquadRole.Flanker );
            blackboard.IsFlankOccupied = true;
            break;
          case 2:
            blackboard.SetRole( sorted[ i ], SquadRole.Vantage );
            blackboard.IsVantageOccupied = true;
            break;
          case 3:
            blackboard.SetRole( sorted[ i ], SquadRole.Healer );
            break;
          default:
            blackboard.SetRole( sorted[ i ], SquadRole.Rusher );
            break;
        }
      }
    }    
  }

  public Transform GetPlayerTarget()
  {
    return playerTarget;
  }
}
