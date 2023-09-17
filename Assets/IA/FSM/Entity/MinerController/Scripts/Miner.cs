using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinerController.Constants;
using IA.Game.Entity.UrbanCenterController;
using IA.Pathfinding;
using IA.Voronoid.Generator;

using TMPro;

using Grid = IA.Pathfinding.Grid;
using IA.Voronoid.Entity;
using System.Collections;

namespace IA.FSM.Entity.MinerController
{
    public class Miner : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TextMeshProUGUI txtInventory = null;
        #endregion

        #region PRIVATE_FIELDS
        private Pathfinder pathfinder = new Pathfinder();
        private MinerBehaviour minerBehaviour = new MinerBehaviour();
        private VoronoidGenerator voronoidGenerator = new VoronoidGenerator();

        private UrbanCenter urbanCenter = null;
        private Grid grid = null;

        private int[,] weights = null;
        #endregion

        #region PROPERTIES
        public MinerBehaviour MinerBehaviour { get => minerBehaviour; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(UrbanCenter urbanCenter, Grid grid, Vector2Int[] minesPositions)
        {
            this.urbanCenter = urbanCenter;
            this.grid = grid;

            Dictionary<TILE_TYPE, int> tileWeigths = MinerConstants.GetTileWeigths();

            CalculateTilesWeights(tileWeigths);

            pathfinder.Init(grid, tileWeigths);
            minerBehaviour.Init(pathfinder, urbanCenter.Tile, urbanCenter, OnLeaveMineralsInHome, grid.GetTile);
            voronoidGenerator.Configure(minesPositions, new Vector2Int(grid.Width, grid.Height), weights);
        }

        public void UpdateBehaviour()
        {
            transform.position = MinerBehaviour.Position;
            minerBehaviour.SetDeltaTime(Time.deltaTime);
            txtInventory.text = minerBehaviour.Inventory.ToString();
        }

        private void OnDrawGizmos()
        {
            voronoidGenerator.Draw();
        }

        public void CalculateTilesWeights(Dictionary<TILE_TYPE, int> tileWeigths)
        {
            weights = new int[grid.Width, grid.Height];

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    weights[x,y] = tileWeigths[grid.GetTile(x, y).type];
                }
            }
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