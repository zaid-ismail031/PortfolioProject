using System.Collections.Generic;

namespace Source.States
{
    public abstract class State : IHSMBase
    {
        /*public abstract Action[] GetActions();
        public abstract Action[] GetEntryActions();
        public abstract Action[] GetExitActions();
        */
        
        public virtual string StateName { get; protected set; }
        protected NPCController Controller;
        
        protected List<Transition> Transitions = new List<Transition>();
        
        
        //HSM Parent
        public State ParentState { get; set; }
        
        //Track Active sub state
        public State ActiveSubState { get; set; }
        
        
        public virtual void Init(NPCController controller)
        {
            Controller = controller;
        }

        public virtual void OnEnterAction() { }

        public virtual void OnUpdateAction() { }

        public virtual void OnExitAction() { }
        
        //Transitions setup 
        public void AddTransition(State nextState, Condition condition)
        {
            var transition = new Transition(nextState, condition);

            //check if transition does not exist in transition list
            var alreadyExists = Transitions.Exists(t => t.GetTargetState() == nextState
                                                        && t.GetCondition().GetType() == condition.GetType());

            if (!alreadyExists)
            {
                Transitions.Add(transition);
            }
            
        }

        public void AddTransition(State nextState, Condition condition, int transitionLevel)
        {
            var transition = new Transition(nextState, condition,transitionLevel);

            //check if transition does not exist in transition list
            var alreadyExists = Transitions.Exists(t => t.GetTargetState() == nextState
                                                        && t.GetCondition().GetType() == condition.GetType());

            if (!alreadyExists)
            {
                Transitions.Add(transition);
            }
        }
        
        
        //list of outgoing transitions from this state
        public List<Transition> GetTransitions()
        {
            return Transitions;
        }

        
        public virtual void ExecuteActions()
        {
            OnUpdateAction();
        }
        
        //HSM Extension
        
        //Get states
        public virtual List<State> GetStates()
        {
            //if we're just a state, then the stack is just us
            List<State> states = new List<State>();
            states.Add(this);
            
            return states;
        }


        //Recursively get update result from this state 
        public virtual UpdateResult RecurseUpdate()
        {

            UpdateResult result = new UpdateResult();
            
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

            if (ActiveSubState != null)
            {
                result = ActiveSubState.RecurseUpdate();
                return result;
            }
            
            //if no transition is found
            result.Transition = null;
            result.Level = 0;
        
            return result;
        }
        
        //Change Substate
        public virtual void ChangeSubState(State newState)
        {
            //Perform exit action
            if (ActiveSubState != null)
            {
                ActiveSubState.OnExitAction();
            }
            
            //Enter new state and perform entry actions
            ActiveSubState = newState;
            ActiveSubState.ParentState = this;
            ActiveSubState.OnEnterAction();
        }
        
        
    }
}