using UnityEngine;

namespace Source.DecisionMake
{
    //will call the patrol action
    public class PatrolAction: Action
    {

        public override DecisionTreeNode MakeDecision()
        {
            //Do Patrol
            Controller.Patrol();
            
            //returns this
            return null;
        }
        
        
    }
}