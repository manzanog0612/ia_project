using System.Collections.Generic;

using UnityEngine;

using IA.Game.Entity.UrbanCenterController;
using IA.FSM.Entity.MinersController;
using IA.FSM.Entity.MinesController;

using Grid = IA.Pathfinding.Grid;

namespace IA.Game.Controller
{
    public class GameController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Grid grid = null;
        [SerializeField] private MinersController minersController = null;
        [SerializeField] private MinesController minesController = null;

        [SerializeField] private GameObject villageCenterPrefab = null;

        [Header("Game Configs")]
        [SerializeField] private int minersAmount = 4;
        [SerializeField] private int minesAmount = 6;
        #endregion

        #region PRIVATE_FIELDS
        private UrbanCenter urbanCenter = null;
        #endregion

        #region UNITY_CALLS
        private void Start()        
        {
            grid.Init();

            urbanCenter = Instantiate(villageCenterPrefab).GetComponent<UrbanCenter>();
            urbanCenter.Init(GetRandomTile());

            Vector2Int[] minesPositions = GetRandomTiles(minesAmount);

            minesController.Init(minesPositions);
            minersController.Init(minersAmount, grid, urbanCenter, minesPositions);
        }

        private void Update()
        {
            urbanCenter.UpdateText();
            minersController.UpdateBehaviours();
            minesController.UpdateMines();
        }
        #endregion

        #region PRIVATE_METHODS
        private Vector2Int GetRandomTile()
        {
            return new Vector2Int(Random.Range(0, grid.Width), Random.Range(0, grid.Height));
        }

        private Vector2Int[] GetRandomTiles(int amountRandomTiles)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();

            for (int i = 0; i < amountRandomTiles; i++)
            {
                int iterations = 0;
                Vector2Int tile;

                do
                {
                    iterations++;
                    tile = new Vector2Int(Random.Range(0, grid.Width), Random.Range(0, grid.Height));
                }
                while (tiles.Contains(tile) && iterations < amountRandomTiles);

                tiles.Add(tile);
            }

            return tiles.ToArray();
        }
        #endregion
    }
}