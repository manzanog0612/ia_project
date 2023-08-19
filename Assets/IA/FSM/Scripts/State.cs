using System;
using System.Collections.Generic;

namespace IA.FSM
{
    public abstract class State
    {
        public Action<int> SetFlag;
        public abstract List<Action> GetOnEnterBehaviours(params object[] parameters);
        public abstract List<Action> GetBehaviours(params object[] parameters);
        public abstract List<Action> GetOnExitBehaviours(params object[] parameters);
        public virtual void Transition(int flag)
        {
            SetFlag.Invoke(flag);
        }
    }
}