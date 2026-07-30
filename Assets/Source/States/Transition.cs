using System;
using UnityEngine;

namespace Source.States
{
    [Serializable]
    public class Transition
    {
        //[SerializeField]protected Action[] actions;
        protected State TargetState;
        protected Condition Condition;

        protected int Level;

        public Transition(State targetState, Condition condition)
        {
            TargetState = targetState;
            Condition = condition;
            Level = 0;
        }
        
        public Transition(State targetState, Condition condition, int transitionLevel)
        {
            TargetState = targetState;
            Condition = condition;
            Level = transitionLevel;
        }

        //HSM Extension
        public int GetLevel()
        {
            return Level;
        }
        
        

        //returns true if transition can fire
        public virtual bool IsTriggered()
        {
            return Condition.Test();
        }
        
        //reports which state to transition to
        public virtual State GetTargetState()
        {
            return TargetState;
        }

        public virtual Condition GetCondition()
        {
            return Condition;
        }
        
        
        
      
        
    }
}