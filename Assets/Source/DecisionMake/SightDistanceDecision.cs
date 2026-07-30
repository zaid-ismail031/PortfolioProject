using StarterAssets;
using UnityEngine;

namespace Source.DecisionMake
{
    //Node asks if the NPC can see the player in its range
    public class SightDistanceDecision: Decision
    {
        
        [SerializeField] float sightDistance = 5f;
        [SerializeField]private Transform playerCharacter;
        [SerializeField] private Transform currentPos;
        
        private NPCController npcController;
        private ThirdPersonController thirdPersonController;
        
        
        public SightDistanceDecision(float distance, ThirdPersonController target, NPCController current){
            
            sightDistance = distance;
            
            npcController = current;
            thirdPersonController = target;
            
        }



        //gets the value of the distance
        public override object TestValue()
        {
            playerCharacter = thirdPersonController.transform;
            currentPos = npcController.transform;
            
            
            //add checks 
            return Vector3.Distance(currentPos.position, playerCharacter.position);
        }
        
        
        public override DecisionTreeNode GetBranch()
        {
            
            
            var distance = (float)TestValue();

            //if within sight, start punch question, else patrol
            if (distance < sightDistance)
            {
                return TrueNode;
            }
            return FalseNode;
            
        }


       
    }
}