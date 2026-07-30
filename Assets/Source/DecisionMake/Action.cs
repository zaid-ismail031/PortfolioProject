namespace Source.DecisionMake
{
    public class Action: DecisionTreeNode
    {
    
        //class contains details of action to run
        
        //character controller component
        protected NPCController Controller;

        //A public init function to set up the character controller for our actions
        public void Init(NPCController controller)
        {
            Controller = controller;
        }
        
        
        public override DecisionTreeNode MakeDecision()
        {
            //so we then re-evaluate from the root
            return null;
        }
        
        
    }
}