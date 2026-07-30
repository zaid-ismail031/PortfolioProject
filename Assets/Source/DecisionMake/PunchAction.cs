using UnityEngine;

namespace Source.DecisionMake
{
    public class PunchAction : Action
    {
        public override DecisionTreeNode MakeDecision()
        {
            //Make sure punch function on character controller has cooldowns etc.
            
            //Perform Punch
            Controller.Punch();
            
            
            //returns this
            return null;
        }
    }
}