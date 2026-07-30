using UnityEngine;

namespace Source.DecisionMake
{
    //Checking if we have enough health to continue pursuing target
    public class HealthDecision: Decision
    {
        [SerializeField]private float safeRatio = 0.4f;
        
        //Need reference to NPC Characters health
        [SerializeField] private NPCController npcController;
        
        private float currentHealth;
        

        public HealthDecision(float ratio, NPCController controller)
        {
            safeRatio = ratio;
            npcController = controller;
        }
        

        public override object TestValue()
        {

            currentHealth = npcController.NPCHealthPoints;
            
            float val = currentHealth / 100f;
            return val;
        }

        public override DecisionTreeNode GetBranch()
        {
            var testRatio = (float)TestValue();

            //do we have enough health to continue chasing
            if (testRatio > safeRatio)
            {
                //returns true node
                return TrueNode;
            }
            
            //else flee
            return FalseNode;

        }

      
    }
}