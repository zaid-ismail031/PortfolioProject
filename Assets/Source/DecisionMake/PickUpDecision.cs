using UnityEngine;

namespace Source.DecisionMake
{
    public class PickUpDecision : Decision
    {
        
        private NPCController npcController;

        private HealthPickup pickUp;

        private GoToAction GoToRef;

        public PickUpDecision(GoToAction goToNode, NPCController controller, HealthPickup pickUp )
        {
            GoToRef = goToNode;
            npcController = controller;
      this.pickUp = pickUp;
            
        }
        

        public override object TestValue()
        {
            if( pickUp == null )
            {
                Debug.LogWarning( "PickUpDecision: pickUp reference is null. Assign HealthPickup in the Inspector." );
                return null;
            }

            if( !pickUp.GetIsActiveDT() )
            {
                Debug.Log( "PickUpDecision: pickup is not active (already collected, waiting for respawn)." );
                return null;
            }

            return pickUp;
        }

        public override DecisionTreeNode GetBranch()
        {
            var testSearch = (HealthPickup)TestValue();

            if( testSearch != null )
            {
                GoToRef.UpdateGoToPosition( testSearch );
                return TrueNode;
            }

            return FalseNode;
        }
        
    }
}