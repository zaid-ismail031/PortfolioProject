using UnityEngine;

namespace Source.DecisionMake
{
    public class GoToAction: Action
    {
        
        //Pickup Point ref
        private HealthPickup healthPickup;
        private Transform pickUpPoint;
        
        private float distanceForPickUp;

        
        public void UpdateGoToPosition(HealthPickup pickUp)
        {
            healthPickup = pickUp;
            pickUpPoint = healthPickup.GetPickupTransformDT();
        }


        public override DecisionTreeNode MakeDecision()
        {
            if( !healthPickup || !healthPickup.GetIsActiveDT() ) return null;

            Controller.GoTo( pickUpPoint.position );
            Controller.AndCheckPickUpDT( healthPickup, distanceForPickUp = 1.5f );

            return null;
        }
    }
}