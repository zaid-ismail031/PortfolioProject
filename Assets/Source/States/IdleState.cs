namespace Source.States
{
    public class IdleState : State
    {
        public override string StateName => "Idle";

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
