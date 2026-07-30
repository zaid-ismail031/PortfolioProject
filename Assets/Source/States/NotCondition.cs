namespace Source.States
{
    public class NotCondition : Condition
    {
        
        private Condition condition;

        public NotCondition(Condition baseCondition)
        {
            condition = baseCondition;
        }
        
        public override bool Test()
        {
            return !condition.Test();
        }
    }
}