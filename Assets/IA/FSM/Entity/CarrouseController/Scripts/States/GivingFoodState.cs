using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinerController;
using IA.FSM.Entity.CarrouseController.Enums;

namespace IA.FSM.Entity.CarrouseController.States
{
    public class GivingFoodState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Miner miner = (Miner)parameters[0];
            Func<int> onGiveFood = (Func<int>)parameters[1];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(()=>
            {
                if (miner == null)
                {
                    Transition((int)Flags.OnFindNextMine);
                }
                else
                {
                    int inventory = onGiveFood.Invoke();

                    miner.MinerBehaviour.ReceiveFood();

                    if (inventory <= 0)
                    {
                        Transition((int)Flags.OnEmptyInventory);
                    }
                    else
                    {
                        Transition((int)Flags.OnFindNextMine);
                    }
                }
            });
            behaviours.Add(() => Debug.Log("GIVING FOOD"));

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
