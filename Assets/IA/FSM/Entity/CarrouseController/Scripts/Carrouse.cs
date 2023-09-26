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
        #region PRIVATE_FIELDS
        private CarrouseBehaviour carrouseBehaviour = new CarrouseBehaviour();
        #endregion

        #region PROPERTIE
        public CarrouseBehaviour CarrouseBehaviour { get => carrouseBehaviour; }
        #endregion

        #region OVERRIDE
        protected override void InitializePathfinder()
        {
            Dictionary<TILE_TYPE, int> tileWeigths = CarrouseConstants.GetTileWeigths();
            Dictionary<TILE_TYPE, bool> tilesWalkableState = CarrouseConstants.GetTilesWalkableState();

            CalculateTilesWeights(tileWeigths);

            pathfinder.Init(grid, tileWeigths, tilesWalkableState);
        }

        protected override void UpdateText()
        {
            txtInventory.text = carrouseBehaviour.Inventory.ToString();
        }

        protected override void UpdatePosition()
        {
            transform.position = carrouseBehaviour.Position;
        }

        public override void UpdateBehaviour()
        {
            base.UpdateBehaviour();
            carrouseBehaviour.SetDeltaTime(Time.deltaTime);
            carrouseBehaviour.UpdateFsm();
        }
        #endregion

        #region PUBLIC_METHODS
        public void InitBehaviour(Func<Vector2, Mine> onGetMineOnPos, Func<List<Miner>> onGetAllMinersMining)
        {
            carrouseBehaviour.Init(pathfinder, voronoidGenerator, urbanCenter, grid, onGetMineOnPos, weights, onGetAllMinersMining);
        }
        #endregion
    }
}