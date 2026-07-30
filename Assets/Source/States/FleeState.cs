using UnityEngine;

namespace Source.States
{
    public class FleeState : State
    {
        
        public override string StateName => "Flee";
        
        public override void OnEnterAction()
        {
            //Entry
            //Method on Controller to replenish health after 15 secs
            var clock = 8f;
            Debug.Log($"FleeState Will Find A Heal After {clock} seconds");
            Controller.RunAwayHeal(clock);
        }

        public override void OnUpdateAction()
        {
            //Call a Flee method
            Controller.RunAway();
        }

        public override void OnExitAction()
        {
            //Exit
        }

        public override void ExecuteActions()
        {
            OnUpdateAction();
        }
    }
}