using System.Collections.Generic;

namespace Source.States
{
    public interface IHSMBase
    {
        
        //public List<Action> GetActions();


        public UpdateResult RecurseUpdate();
     

        public List<State> GetStates();


    }
}