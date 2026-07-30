using UnityEngine;

namespace Source.DecisionMake
{
    public class ChaseAction: Action
    {
        //Call on the character controller to move towards its target

        public override DecisionTreeNode MakeDecision()
        {
            
            //Calls chase on NPC controller
            Controller.Chase();
            
            //returns this
            return null;
        }
    }
}