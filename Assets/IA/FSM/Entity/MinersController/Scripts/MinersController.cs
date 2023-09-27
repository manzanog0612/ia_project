using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController;

using IA.FSM.Common.Entity.PathfinderEntityController;

using IA.Game.Entity.UrbanCenterController;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Entity.MinersController
{
    public class MinersController : PathfindersController
    {
        #region PUBLIC_METHODS
        public override void Init(int entitiesAmount, Grid grid, UrbanCenter urbanCenter, params object[] paramenters)
        {
            base.Init(entitiesAmount, grid, urbanCenter, paramenters);

            for (int i = 0; i < entitiesAmount; i++)
            {
                (pathfinders[i] as Miner).InitBehaviour((Func<Vector2, Mine>)paramenters[0], (Func<Mine[]>)paramenters[1]);
            }
        }

        public List<Miner> GetMinersMining()
        {
            List<Miner> miners = new List<Miner>();
            for (int i = 0; i < pathfinders.Length; i++)
            {
                miners.Add(pathfinders[i] as Miner);
            }

            return miners.FindAll(m => m.MinerBehaviour.ActualState == MinerController.Enums.States.Mining ||
                                       m.MinerBehaviour.ActualState == MinerController.Enums.States.WaitingForFood);
        }
        #endregion
    }
}
