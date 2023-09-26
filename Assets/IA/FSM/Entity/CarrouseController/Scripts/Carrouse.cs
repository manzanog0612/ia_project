using System;
using System.Collections.Generic;

using UnityEngine;

using IA.Pathfinding;
using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController.Constants;
using IA.FSM.Entity.MinerController;

namespace IA.FSM.Entity.CarrouseController
{
    public class Carrouse : PathfinderEntity
    {
        #region PRIVATE_FIELDS
        private MinerBehaviour minerBehaviour = new MinerBehaviour();
        #endregion

        #region PROPERTIES
        public MinerBehaviour MinerBehaviour { get => minerBehaviour; }
        #endregion

        #region OVERRIDE
        protected override void InitializePathfinder()
        {
            Dictionary<TILE_TYPE, int> tileWeigths = MinerConstants.GetTileWeigths();
            Dictionary<TILE_TYPE, bool> tilesWalkableState = MinerConstants.GetTilesWalkableState();

            CalculateTilesWeights(tileWeigths);

            pathfinder.Init(grid, tileWeigths, tilesWalkableState);
        }

        protected override void UpdateText()
        {
            txtInventory.text = minerBehaviour.Inventory.ToString();
        }

        protected override void UpdatePosition()
        {
            transform.position = MinerBehaviour.Position;
        }

        public override void UpdateBehaviour()
        {
            base.UpdateBehaviour();
            minerBehaviour.SetDeltaTime(Time.deltaTime);
        }
        #endregion

        #region PUBLIC_METHODS
        public void InitBehaviour(Func<Vector2, Mine> onGetMineOnPos, Func<Mine[]> onGetAllMinesLeft)
        {
            minerBehaviour.Init(pathfinder, voronoidGenerator, urbanCenter, grid, OnLeaveMineralsInHome, onGetMineOnPos, onGetAllMinesLeft, weights);
        }
        #endregion

        #region PRIVATE_METHODS
        private void OnLeaveMineralsInHome()
        {
            urbanCenter.PlaceMinerals(minerBehaviour.Inventory);
        }
        #endregion
    }
}