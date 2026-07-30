using StarterAssets;
using UnityEngine;

namespace Source.States
{
    public class NPCStateController: MonoBehaviour
    {
        
        [SerializeField]private string activeState;
        
        [SerializeField] private NPCController npcController;
        [SerializeField] private StateMachine stateMachine;
        
        [SerializeField] private ThirdPersonController playerController;

        private void Start()
        {
            InitStates();
        }

        private void Update()
        {
            activeState = stateMachine.ActiveState;
        }

        public string GetStateText()
        {
            if (stateMachine != null)
            {
                return activeState;
            }

            return "No Finite State Machine";
        }

        private void InitStates()
        {
            //Create states and initialize them
            var idleState = new IdleState();
            idleState.Init(npcController);

            var patrolState = new PatrolState();
            patrolState.Init(npcController);

            var chaseState = new ChaseState();
            chaseState.Init(npcController);

            var punchState = new PunchState();
            punchState.Init(npcController);

            var fleeState = new FleeState();
            fleeState.Init(npcController);

            //Time-based conditions
            var patrolTimerCondition = new PatrolTimerCondition(10f);
            var idleTimerCondition = new PatrolTimerCondition(5f);

            //Link timer conditions so they reset on state entry
            idleState.SetTimerCondition(idleTimerCondition);
            patrolState.SetTimerCondition(patrolTimerCondition);

            //Set up transitions for states

            //Idle State : after 3 seconds, start patrolling. If player in chase range, chase. If low health, flee.
            idleState.AddTransition(patrolState, idleTimerCondition);
            idleState.AddTransition(chaseState, new ChaseRangeCondition(npcController,playerController,5f));
            idleState.AddTransition(fleeState, new HealthCondition(npcController,0.4f));

            //Patrol State : after 10 seconds become idle. If in chase range, chase. If low health, flee.
            patrolState.AddTransition(idleState, patrolTimerCondition);
            patrolState.AddTransition(chaseState, new ChaseRangeCondition(npcController,playerController,5f));
            patrolState.AddTransition(fleeState, new HealthCondition(npcController,0.4f));

            //Chase State : if in punch range change state, if not in chase range go idle, if low health flee
            chaseState.AddTransition(punchState, new PunchRangeCondition(npcController,playerController,1.5f));
            chaseState.AddTransition(idleState, new NotCondition(new ChaseRangeCondition(npcController,playerController,5f)));
            chaseState.AddTransition(fleeState, new HealthCondition(npcController,0.4f));

            //Punch State : if not in punch range chase, if low health flee
            punchState.AddTransition(chaseState,new NotCondition(new PunchRangeCondition(npcController,playerController,1.5f)));
            punchState.AddTransition(fleeState, new HealthCondition(npcController,0.4f));

            //Flee State : when health recovered, go back to idle
            fleeState.AddTransition(idleState, new NotCondition(new HealthCondition(npcController,0.4f)));

            //Set initial state in State Machine - NPC starts idle
            stateMachine.SetInitialState(idleState);

        }
        
        
    }
}