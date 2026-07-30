using System;
using System.Collections.Generic;

namespace Source.States
{
    public class HierarchicalStateMachine: State
    {
        //List of states at this level of the hierarchy
        private List<State> states = new List<State>();
        
        //Initial state for when machine has no current state
        protected State initialState;
        
        //current state of the machine
        protected State currentState;
        
        //parent reference
        public HierarchicalStateMachine Parent { get; set; }

        
        
        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(StateName))
            {
                StateName = name;
            }
        }

        public void Update()
        {
            UpdateMachine();
        }
        
        
        public void SetInitialState(State state)
        {
            initialState = state;
            currentState = state;
            
            //ChangeSubState(state);
            ActiveSubState = state;
        }
        
        
        //add states
        public void AddState(State state)
        {
            if (!states.Contains(state))
            {
                states.Add(state);
                state.ParentState = this;
            }
        }
        
        //Gets the current state stack
        //states can now be state machines themselves, hence why we need this
        public override List<State> GetStates()
        {
            List<State> list = new List<State>();
            list.Add(this);
            
            if (currentState != null)
            {
                list.AddRange(currentState.GetStates());
            }
            
            return list;
        }


        private void GetParentState(State childState, List<State> stack)
        {
            if (childState.ParentState != null)
            {
                GetParentState(childState.ParentState, stack);
            }
            
            stack.Add(childState);
            
        }
        
        
        public List<State> GetCurrentStateStack()
        {
            
            
            List<State> list = new List<State>();
            
            if (currentState != null)
            {
                GetParentState(currentState,list);
            }
            
            return list;
        }
        

        //Recurse update will check transitions at this level first
        public override UpdateResult RecurseUpdate()
        {
            
            UpdateResult result = new UpdateResult();
            
            //Check transitions at this machine level, may or may not be necessary
            foreach (var t in Transitions)
            {
                if (t.IsTriggered())
                {
                    result = new UpdateResult
                        {
                            
                            Transition = t,
                            Level = t.GetLevel()
                            
                        };
                    
                    return result;
                }
            }
            
            //if no transition, recurse into current state
            if (currentState != null)
            {
                return currentState.RecurseUpdate();
            }

            result = new UpdateResult()
            {
                Transition = null,
                Level = 0
            };
            
            return result;
        }


        //Updates the machine recursively
        private void UpdateMachine()
        {
            
            //if we're in no state, set the initial state
            if (currentState == null && initialState != null)
            {
                ChangeState(initialState);
                return;
            }
            
            //if null current and initial, return
            if (currentState == null) return;
            
            
            //recursive update checks our current state for any transitions made
            UpdateResult result = currentState.RecurseUpdate();
            
            
            //if we found a result transition trigger to fire
            //handle that result using our recurse structure, which stored transition information and level information
            if (result.Transition != null)
            {
                //to handle our cross hierarchy transitions
                HandleTransitions(result);
            }
            //if we didn't get a transition
            else
            {
                //Executes current states actions
                currentState.ExecuteActions();
            }

        }

        private void HandleTransitions(UpdateResult result)
        {
            //perform state actions and transitions based on its level
            if (result.Level == 0)
            {
                    
                //if result is on our level, then just find the 
                var targetState = result.Transition.GetTargetState();
                    
                //complete transition with a change state
                ChangeState(targetState);
                    
                    
                //Clear the transition, so nobody else does it
                result.Transition = null;
                    
            }
                
            else if (result.Level > 0)
            {
                //operating on a higher level
                //exit our current state
                currentState.OnExitAction();
                currentState = null;
                    
                //Decrease the number of levels to go
                result.Level -= 1;

                //make the parent handle the transition
                if (Parent != null)
                {
                    Parent.HandleTransitions(result);
                }
                else 
                {
                    if (result.Level == 0 && result.Transition != null)
                    {
                        var targetState = result.Transition.GetTargetState();
                        ChangeState(targetState);
                        result.Transition = null;
                    }
                }
                    

            }

            else
            {
                //result level is currently negative
                //it needs to be passed down
                var targetState = result.Transition.GetTargetState();
                UpdateDown(targetState, -result.Level);
                    
                
                //clear the transition, so nobody else does it
                result.Transition = null;
            }

            
        }
        
        
        //Recurses up the parent hierarchy, transitioning into each state
        //in turn for the given number of levels.
        private void UpdateDown(State state, int level)
        {
            //if we're not at top level, continue recursing
            if (level > 0)
            {
                //if parent is not null, update down
                if (Parent != null)
                {
                    //pass self as the transition state to the parent
                    Parent.UpdateDown(this, level - 1);
                }
               
            }
           
            
            //if we have a current state, exit it
            if (currentState != null)
            {
                //not necessary
                currentState.OnExitAction();
                
            }
            
            //Move to the new state, and return all the actions
            ChangeState(state);
            
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
            //activeState = currentState.StateName;
            
            //ChangeSubState(newState);
            ActiveSubState = currentState;
        }

        
        
        //
        public override void ExecuteActions()
        {
            UpdateMachine();
        }
        
        public override void OnUpdateAction()
        {
            UpdateMachine();
        }
        
    }
}