using UnityEngine;

namespace Source.DecisionMake
{
    public class FleeAction : Action
    {
        public override DecisionTreeNode MakeDecision()
        {
            
            //call on controller to flee
            Controller.RunAway();
            
            //Debug.Log("FleeAction calling");
            
            //returns this
            return null;
        }
    }
}