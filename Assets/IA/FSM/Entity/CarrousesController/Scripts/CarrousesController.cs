using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Common.Entity.PathfinderEntityController;
using IA.FSM.Entity.MinerController;
using IA.FSM.Entity.MineController;
using IA.FSM.Entity.CarrouseController;

using IA.Game.Entity.UrbanCenterController;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Entity.CarrousesController
{
    public class CarrousesController : PathfindersController
    {
        #region PUBLIC_METHODS
        public override void Init(int entitiesAmount, Grid grid, UrbanCenter urbanCenter, params object[] paramenters)
        {
            base.Init(entitiesAmount, grid, urbanCenter, paramenters);

            for (int i = 0; i < entitiesAmount; i++)
            {
                (pathfinders[i] as Carrouse).InitBehaviour((Func<Vector2, Mine>)paramenters[0], (Func<List<Miner>>)paramenters[1]);
            }
        }
        #endregion
    }
}