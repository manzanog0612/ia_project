using System;
using System.Collections.Generic;

using UnityEngine;

namespace IA.FSM.Entity.Miner.Enum
{
    public class MiningState : State
    {
        private float miningTime = 0;
        private int inventoryCapacity = 0;

        private float miningTimer = 0;

        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Mine.Mine targetMine = parameters[0] as Mine.Mine;
            int inventory = (int)parameters[1];
            Action onMine = parameters[2] as Action;

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (targetMine.Minerals == 0)
                {
                    Transition((int)Flags.OnEmptyMine);
                }
                else
                if (inventory == inventoryCapacity)
                {
                    Transition((int)Flags.OnFullInventory);
                }
                else                
                {
                    miningTimer += Time.deltaTime;

                    if (miningTimer > miningTime)
                    {
                        targetMine.Extract();
                        onMine.Invoke();

                        miningTimer = 0;
                    }
                }
            });
            behaviours.Add(() => Debug.Log("MINING"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
            miningTime = (float)parameters[0];
            inventoryCapacity = (int)parameters[1];

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
