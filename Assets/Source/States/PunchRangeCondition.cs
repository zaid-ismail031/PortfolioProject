using StarterAssets;
using UnityEngine;

namespace Source.States
{
    public class PunchRangeCondition : Condition
    {
        
        [SerializeField] float punchDistance = 1.5f;
        [SerializeField]private Transform playerCharacter;
        [SerializeField] private Transform currentPos;
        
        private NPCController npcController;
        private ThirdPersonController thirdPersonController;
        
        
        public PunchRangeCondition(NPCController controller,ThirdPersonController target,float distance)
        {
            punchDistance = distance;
            
            thirdPersonController = target;
            npcController = controller;
            
        }
        
        public override bool Test()
        {
            playerCharacter = thirdPersonController.transform;
            currentPos = npcController.transform;
            
            var testVal = Vector3.Distance(playerCharacter.position, currentPos.position);

            //if distance is less than, punch condition 
            if (testVal < punchDistance)
            {
                return true;
            }
            
            return false;
        }
    }
}