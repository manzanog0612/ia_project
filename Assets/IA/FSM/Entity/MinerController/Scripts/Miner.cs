using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController.Constants;

using IA.FSM.Common.Entity.PathfinderEntityController;

using IA.Pathfinding;

namespace IA.FSM.Entity.MinerController
{
    public class Miner : PathfinderEntity
    {
        #region PROPERTIE
        public MinerBehaviour MinerBehaviour { get => pathfinderBehaviour as MinerBehaviour; }
        #endregion

        #region OVERRIDE
        protected override void InitializePathfinder()
        {
            Dictionary<TILE_TYPE, int> tileWeigths = MinerConstants.GetTileWeigths();
            Dictionary<TILE_TYPE, bool> tilesWalkableState = MinerConstants.GetTilesWalkableState();

            CalculateTilesWeights(tileWeigths);

            pathfinderBehaviour = new MinerBehaviour();
            pathfinder.Init(grid, tileWeigths, tilesWalkableState);
        }

        #endregion

        #region PUBLIC_METHODS
        public void InitBehaviour(Func<Vector2, Mine> onGetMineOnPos, Func<Mine[]> onGetAllMinesLeft)
        {
            Action onLeaveMineralsInUrbanCenter = LeaveMineralsInUrbanCenter;

            pathfinderBehaviour.Init(pathfinder, voronoidGenerator, urbanCenter, grid,  onGetMineOnPos, weights, onLeaveMineralsInUrbanCenter, onGetAllMinesLeft);
        }
        #endregion

        #region PRIVATE_METHODS
        private void LeaveMineralsInUrbanCenter()
        {
            urbanCenter.PlaceMinerals(pathfinderBehaviour.Inventory);
        }
        #endregion
    }
}