using StarterAssets;
using UnityEngine;

namespace Source.States
{
    public class HSMController: MonoBehaviour
    {
        
        [SerializeField] private string activeStateStack;  // Shows full hierarchy
        [SerializeField] private string currentState; 
        
        [SerializeField] private NPCController npcController;
        [SerializeField] private ThirdPersonController playerController;

        
        [SerializeField] private HierarchicalStateMachine hsmStateMachine;
        
        private void Start()
        {
            if (hsmStateMachine == null)
            {
                hsmStateMachine = new HierarchicalStateMachine();
                hsmStateMachine.SetName("HSM");
            }

            InitStates();
        }


        private void InitStates()
        {
            
            //create states
            var guardState = new GuardSuperState();
            
            //Initialize guard state with player ref first,
            
            var fleeState = new FleeState();
            fleeState.Init(npcController);

            
            guardState.SetFleeState(fleeState);
            guardState.InitTarget(playerController);
            guardState.Init(npcController);
            
            
            //Set up transitions
            //Condition healthCondition = new HealthCondition(npcController,0.4f);
            guardState.AddTransition(fleeState,new HealthCondition(npcController,0.4f));
            
            //to return to guard state from flee when health is recovered
            fleeState.AddTransition(guardState,new NotCondition(new HealthCondition(npcController,0.4f)));
            
            //adding to state machine
            hsmStateMachine.AddState(guardState);
            hsmStateMachine.AddState(fleeState);
            
            hsmStateMachine.SetInitialState(guardState);
            
            
        }

        private void Update()
        {
            if (hsmStateMachine != null)
            {
                hsmStateMachine.Update();
            }
            
            // Update the displayed state information
            UpdateActiveStateDisplay();
            
        }
        
        private void UpdateActiveStateDisplay()
        {
            if (hsmStateMachine != null)
            {
                activeStateStack = GetStateStack();
                currentState = GetActiveState();
            }
            else
            {
                activeStateStack = "No State Machine";
                currentState = "None";
            }
        }

        public string GetStateText()
        {
            if (hsmStateMachine != null)
            {
                return activeStateStack;
            }

            return "No Hierarchical State Machine";

        }
        
        private string GetStateStack()
        {
            var states = hsmStateMachine.GetCurrentStateStack();
            
            if (states.Count == 0)
            {
                return "None";   
            }
            
            string display = "";
            for (int i = 0; i < states.Count; i++)
            {
                
                display += states[i].StateName;
                
                if (i != states.Count - 1)
                {
                    display += " -> ";
                }
             
            }
            
            return display;
            
        }
        
        private string GetActiveState()
        {
            var states = hsmStateMachine.GetStates();
            
            if (states.Count == 0)
            {
                return "None";
            }
           
            // current active state
            return states[^1].StateName;
            
        }

    }
}