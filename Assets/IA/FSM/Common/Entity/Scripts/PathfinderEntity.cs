using System.Collections.Generic;

using UnityEngine;

using IA.Voronoid.Generator;
using IA.Game.Entity.UrbanCenterController;
using IA.Pathfinding;

using TMPro;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Common.Entity.PathfinderEntityController
{
    public abstract class PathfinderEntity : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] protected TextMeshProUGUI txtInventory = null;
        [SerializeField] private bool drawVoronoi = false;
        #endregion

        #region PRIVATE_FIELDS
        protected Pathfinder pathfinder = new Pathfinder();
        protected VoronoidGenerator voronoidGenerator = new VoronoidGenerator();
        
        protected UrbanCenter urbanCenter = null;
        protected Grid grid = null;
        
        protected int[,] weights = null;
        #endregion

        #region UNITY_CALLS
        private void OnDrawGizmos()
        {
            if (drawVoronoi)
            { 
                voronoidGenerator.Draw(); 
            }
        }
        #endregion

        #region PUBLIC_METHODS
        public void Init(UrbanCenter urbanCenter, Grid grid)
        {
            this.urbanCenter = urbanCenter;
            this.grid = grid;

            InitializePathfinder();
        }

        public virtual void UpdateBehaviour()
        {
            UpdateText();
            UpdatePosition();
        }
        #endregion

        #region PROTECTED_METHOS
        protected abstract void InitializePathfinder();
        protected abstract void UpdateText();
        protected abstract void UpdatePosition();
        protected void CalculateTilesWeights(Dictionary<TILE_TYPE, int> tileWeigths)
        {
            weights = new int[grid.Width, grid.Height];

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    weights[x, y] = tileWeigths[grid.GetTile(x, y).type];
                }
            }
        }
        #endregion
    }
}
