using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinerController.Enums;

namespace IA.FSM.Entity.MinerController.States
{
    public class HungryState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            int foodLeft = (int)parameters[0];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (foodLeft > 0)
                {
                    Transition((int)Flags.OnReceivedFood);
                }
            });
            behaviours.Add(() => Debug.Log("HUNGRY"));

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
