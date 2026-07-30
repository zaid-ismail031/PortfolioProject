namespace Source.States
{
    public class ChaseState:State
    {
        
        public override string StateName => "Chase";
        
        public override void OnEnterAction()
        {
            //Entry
        }

        public override void OnUpdateAction()
        {
            //Chase Target
            Controller.Chase();
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