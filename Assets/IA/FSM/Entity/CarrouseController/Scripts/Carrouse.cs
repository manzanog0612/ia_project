using System;
using System.Collections.Generic;

using UnityEngine;

using IA.Pathfinding;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController;

using IA.FSM.Common.Entity.PathfinderEntityController;

using IA.FSM.Entity.CarrouseController.Constants;

namespace IA.FSM.Entity.CarrouseController
{
    public class Carrouse : PathfinderEntity
    {
        #region OVERRIDE
        protected override void InitializePathfinder()
        {
            Dictionary<TILE_TYPE, int> tileWeigths = CarrouseConstants.GetTileWeigths();
            Dictionary<TILE_TYPE, bool> tilesWalkableState = CarrouseConstants.GetTilesWalkableState();

            CalculateTilesWeights(tileWeigths);

            pathfinderBehaviour = new CarrouseBehaviour();
            pathfinder.Init(grid, tileWeigths, tilesWalkableState);
        }
        #endregion

        #region PUBLIC_METHODS
        public void InitBehaviour(Func<Vector2, Mine> onGetMineOnPos, Func<List<Miner>> onGetAllMinersMining)
        {
            pathfinderBehaviour.Init(pathfinder, voronoidGenerator, urbanCenter, grid, onGetMineOnPos, weights, onGetAllMinersMining);
        }
        #endregion
    }
}