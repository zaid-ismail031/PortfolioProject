using StarterAssets;

namespace Source.States
{
    public class GuardSuperState : SubMachineState
    {
        public override string StateName => "GuardSuperState";
        
        private PatrolState patrolState;
        private ChaseState chaseState;
        private PunchState punchState;
        private FleeState fleeRef;
        
        
        //Target Player reference
        private ThirdPersonController playerController;

        //make sure to call this
        public void InitTarget(ThirdPersonController controller)
        {
            playerController = controller;
        }


        public void SetFleeState(FleeState fleeState)
        {
            this.fleeRef = fleeState;
        }

        public override void Init(NPCController controller)
        {
            base.Init(controller);
            
            //Create substates
            patrolState = new PatrolState();
            patrolState.Init(controller);
            AddState(patrolState);
            
            chaseState = new ChaseState();
            chaseState.Init(controller);
            AddState(chaseState);
            
            punchState = new PunchState();
            punchState.Init(controller);
            AddState(punchState);
            
            
            //Conditions
            var chaseRangeCondition = new ChaseRangeCondition(Controller, playerController, 5f);
            var punchRangeCondition = new PunchRangeCondition(Controller, playerController, 2f);
            var healthCondition = new HealthCondition(Controller, 0.4f);
            
            //Sub state transitions
            if (playerController != null)
            {
                //patrol
                patrolState.AddTransition(chaseState,chaseRangeCondition);
                
                //transition out of state
                patrolState.AddTransition(fleeRef,healthCondition,1);
                
            
                //chase
                chaseState.AddTransition(punchState, new PunchRangeCondition(controller,playerController ,2f));
                chaseState.AddTransition(patrolState, new NotCondition(new ChaseRangeCondition(controller,playerController ,5f)));
                
                chaseState.AddTransition(fleeRef,healthCondition,1);
                
                //punch
                punchState.AddTransition(chaseState, new NotCondition(new PunchRangeCondition(controller, playerController,2f)));
                punchState.AddTransition(fleeRef,healthCondition,1);
                
                
            }
           
            // Set initial 
            SetInitialState(patrolState);
            
        }

        //can override On Enter and Exit to debug
        
        
        
    }
}