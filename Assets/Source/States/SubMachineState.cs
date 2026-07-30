using System.Collections.Generic;

namespace Source.States
{
    //State inherits HSM
    public class SubMachineState : HierarchicalStateMachine
    {
      
        
        //Get states by adding self to active children
        public override List<State> GetStates()
        {
            
            var list = new List<State>();
            list.Add(this);
            
            if (currentState != null)
            {
                list.AddRange(currentState.GetStates());
            }
         
            return list;
            
        }
        
        
        /*//route to update machine
        public override void OnUpdateAction()
        {
            base.OnUpdateAction();
        }

        public override void ExecuteActions()
        {
            UpdateMachine();
        }*/
    }
}