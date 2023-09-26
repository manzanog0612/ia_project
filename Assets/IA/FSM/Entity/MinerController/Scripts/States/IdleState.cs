using System;
using System.Collections.Generic;

using UnityEngine;

namespace IA.FSM.Entity.MinerController.States
{
    public class IdleState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            List<Action> behaviours = new List<Action>();
            behaviours.Add(()=> Debug.Log("FINISHED JOB"));
            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
            List<Action> behaviours = new List<Action>();
            return behaviours;
        }

        public override List<Action> GetOnExitBehaviours(params object[] parameters)
        {
            List<Action> behaviours = new List<Action>();
            return behaviours;
        }
    }
}
