namespace Source.States
{
    public class PunchState:State
    {
        public override string StateName => "Punch";

        public override void OnEnterAction()
        {
            //Entry
        }

        public override void OnUpdateAction()
        {
            //Do Punch
            Controller.Punch();
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