
using UnityEngine;

namespace Source.DecisionMake
{
    public class Decision: DecisionTreeNode
    {
        //pointers to other nodes in the tree
        protected DecisionTreeNode TrueNode;
        protected DecisionTreeNode FalseNode;


        public void SetNodes(DecisionTreeNode trueNode, DecisionTreeNode falseNode)
        {
            this.TrueNode = trueNode;
            this.FalseNode = falseNode;
        }


        //Define in subclasses with appropriate type
        //object can be any type
        public virtual object TestValue()
        {
            //returns the piece of data in the character's knowledge 
            //forming the basis of the test
            return null;
        }
        
        //Perform the Test, and return the branch that should be followed
        public virtual DecisionTreeNode GetBranch()
        {
            //return
            return TrueNode;
        }

        //Recursively walk through the tree, 
        public override DecisionTreeNode MakeDecision()
        {
            //Gets a branch and performs it's Make Decision
            //from root to action
            DecisionTreeNode branch = GetBranch();
            return branch.MakeDecision();
        }
        
        

    }
}