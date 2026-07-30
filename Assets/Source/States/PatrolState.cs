namespace Source.States
{
    public class PatrolState : State
    {
        public override string StateName => "Patrol";

        private PatrolTimerCondition timerCondition;

        public void SetTimerCondition( PatrolTimerCondition condition )
        {
            timerCondition = condition;
        }

        public override void OnEnterAction()
        {
            if( timerCondition != null )
            {
                timerCondition.Reset();
            }
        }

        public override void OnUpdateAction()
        {
            Controller.Patrol();
        }

        public override void OnExitAction()
        {
        }

        public override void ExecuteActions()
        {
            OnUpdateAction();
        }
    }
}