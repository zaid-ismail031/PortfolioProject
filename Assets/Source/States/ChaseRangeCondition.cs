using StarterAssets;
using UnityEngine;

namespace Source.States
{
    public class ChaseRangeCondition : Condition
    {
        
        [SerializeField] float sightDistance = 5f;
        [SerializeField]private Transform playerCharacter;
        [SerializeField] private Transform currentPos;
        
        private NPCController npcController;
        private ThirdPersonController thirdPersonController;

        public ChaseRangeCondition(NPCController controller,ThirdPersonController target,float distance)
        {
            sightDistance = distance;
            
            thirdPersonController = target;
            npcController = controller;

          
        }
        
        
        public override bool Test()
        {
            
            playerCharacter = thirdPersonController.transform;
            currentPos = npcController.transform;
            
            var testVal = Vector3.Distance(playerCharacter.position, currentPos.position);

            //if distance is less than, chase condition 
            if (testVal < sightDistance)
            {
                return true;
            }
            
            return false;
            
        }
    }
}