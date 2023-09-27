using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinerController.Enums;
using IA.FSM.Entity.MineController;
using IA.FSM.Common.Enums;

namespace IA.FSM.Entity.MinerController.States
{
    public class HungryState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            int foodLeft = (int)parameters[0];
            Mine mine = (Mine)parameters[1];
            bool panic = (bool)parameters[2];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (panic)
                {
                    Transition((int)CommonFlags.OnPanic);
                }
                else
                if (foodLeft > 0)
                {
                    Transition((int)Flags.OnReceivedFood);
                }
                else if (mine.Minerals == 0)
                {
                    Transition((int)Flags.OnEmptyMine);
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
