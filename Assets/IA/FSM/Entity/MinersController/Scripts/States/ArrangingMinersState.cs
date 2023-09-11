using System;
using System.Collections.Generic;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController;
using IA.FSM.Entity.MinersController.Enums;

namespace IA.FSM.Entity.MinersController.States
{
    public class ArrangingMinersState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            List<Miner> miners = parameters[0] as List<Miner>;
            List<Mine> mines = parameters[1] as List<Mine>;

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                for (int i = 0; i < mines.Count; i++)
                {
                    if (miners.Count > i)
                    {
                        miners[i].MinerBehaviour.SetMine(mines[i]);
                    }
                }

                if (mines.Count < miners.Count && mines.Count > 0)
                {
                    int minesIndex = 0;
                    for (int i = mines.Count; i < miners.Count; i++)
                    {
                        if (minesIndex == mines.Count)
                        {
                            minesIndex = 0;
                        }

                        miners[i].MinerBehaviour.SetMine(mines[minesIndex]);

                        minesIndex++;
                    }
                }

                Transition((int)Flags.OnArragementApplied);
            });

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
            List<Action> enterBehaviours = new List<Action>();

            return enterBehaviours;
        }

        public override List<Action> GetOnExitBehaviours(params object[] parameters)
        {
            List<Miner> miners = parameters[0] as List<Miner>;
            List<Mine> mines = parameters[1] as List<Mine>;

            List<Action> exitBehaviours = new List<Action>();

            exitBehaviours.Add(() =>
            {
                miners.Clear();
                mines.Clear();
            });

            return exitBehaviours;
        }
    }
}
