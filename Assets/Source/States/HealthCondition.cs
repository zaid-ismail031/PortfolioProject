using UnityEngine;


namespace Source.States
{
    public class HealthCondition : Condition
    {
        //Needs Health Information
        private NPCController npcController;
        private float safeRatio;
        
        
        //Need reference to NPC Characters health
        //private CharacterHealth characterHealth;

        public HealthCondition(NPCController controller,float ratio)
        {
            npcController = controller;
            safeRatio = ratio;
        }
        
        
        //will send correct test values
        public override bool Test()
        {
            var testVal = npcController.NPCHealthPoints / 100f;
            
            //will result to fleeing
            if (testVal <= safeRatio)
            {
                return true;
            }
            
            return false;
        }
        
        
        
        
    }
}