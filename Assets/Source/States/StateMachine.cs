using System;
using UnityEngine;

namespace Source.States
{
    public class StateMachine : MonoBehaviour
    {
        
        private string activeState;
        public string ActiveState => activeState;
        
        //initial state
        private State initialState;
        private State currentState;
        
      
        //Set initial state for state machine
        public void SetInitialState(State state)
        {
            initialState = state;
            ChangeState(initialState);
        }
        
        
        private void Start()
        {
            if (initialState != null)
            {
                ChangeState(initialState);   
            }
        }


        //private 
        //checks and applies transitions
        private void ExecuteActions()
        {
            //assume no transition is triggered
            Transition triggered = null;
            
            //check through each transition and store the first one that triggers
            foreach (var t in currentState.GetTransitions())
            {
                if (!t.IsTriggered()) continue;
                triggered = t;
                break;
            }
            
            //check if we have a transition to fire
            if (triggered != null)
            {
                //find the target state
                var targetState = triggered.GetTargetState();
                
                //complete the transition 
                ChangeState(targetState);
     
            }
           
            //return the current state's actions
            currentState.ExecuteActions();
        
        }
        
        
        private void Update()
        {
            ExecuteActions();
        }



        private void ChangeState(State newState)
        {
            //Perform exit action
            if (currentState != null)
            {
                currentState.OnExitAction();
            }
            
            //Enter new state and perform entry actions
            currentState = newState;
            currentState.OnEnterAction();
            activeState = currentState.StateName;

        }
        
        
        
        
    }
}