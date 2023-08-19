using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.Miner.Enum;

namespace IA.FSM.Entity.Miner.States
{
    public class IdleState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            bool setNewMine = (bool)parameters[0];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (setNewMine)
                {
                    Transition((int)Flags.OnSetMine);
                }
            });
            behaviours.Add(() => Debug.Log("IDLE"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
            List<Action> enterBehaviours = new List<Action>();

            return enterBehaviours;
        }

        public override List<Action> GetOnExitBehaviours(params object[] parameters)
        {
            List<Action> exitBehaviours = new List<Action>();

            return exitBehaviours;
        }
    }
}
