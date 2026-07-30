
namespace Source.DecisionMake
{
    [System.Serializable]
    public abstract class DecisionTreeNode
    {
        
        //Recursively walk through the tree
        public abstract DecisionTreeNode MakeDecision();
        


    }
}
