using StarterAssets;
using UnityEngine;

namespace Source.DecisionMake
{
    //checking if we are close enough to perform an attack
    public class PunchDistanceDecision: Decision
    {
        
        float punchDistance = 2f;
        private Transform playerCharacter;
        private Transform currentPos;

        private NPCController npcController;
        private ThirdPersonController thirdPersonController;
        
        public PunchDistanceDecision(float distance, ThirdPersonController target, NPCController current){
            
            punchDistance = distance;
            
            npcController = current;
            thirdPersonController = target;
            
        }


        //gets the value of the distance
        public override object TestValue()
        {

            playerCharacter = thirdPersonController.transform;
            currentPos = npcController.transform;
            
            
            //add null checks 
            //if(!playerCharacter)return float.MaxValue;
            //if(!currentPos)return float.MaxValue;
            
            return Vector3.Distance(currentPos.position, playerCharacter.position);
        }
        
        
        public override DecisionTreeNode GetBranch()
        {
            var distance = (float)TestValue();

            //if less than punch range, punch else chase
            if (distance < punchDistance)
            {
                return TrueNode;
            }
            return FalseNode;
            
        }

    }
}