using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using IA.Game.Entity.UrbanCenterController;
using IA.Pathfinding;
using IA.FSM.Entity.MinerController.Constants;
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

        [SerializeField] private GameObject urbanCenterPrefab = null;

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

            Camera.main.transform.position = new Vector3 (grid.RealWidth / 2f, grid.RealHeight / 2f, -10);
            Camera.main.orthographicSize = grid.RealWidth;

            urbanCenter = Instantiate(urbanCenterPrefab).GetComponent<UrbanCenter>();

            List<TILE_TYPE> allWalkableTiles = GetWalkableTilesForAllPathfinders();

            Vector2Int urbanCenterTile = GetRandomWalkableTile(allWalkableTiles);
            urbanCenter.Init(urbanCenterTile, grid.GetRealPosition(urbanCenterTile));

            Vector2Int[] minesTiles = GetRandomWalkableTiles(minesAmount, allWalkableTiles, urbanCenterTile);

            minesController.Init(minesTiles, grid.GetRealPosition);
            minersController.Init(minersAmount, grid, urbanCenter, minesController.GetMineOnPos, minesController.GetMinesLeft);
        }

        private void Update()
        {
            urbanCenter.UpdateText();
            minersController.UpdateBehaviours();
            minesController.UpdateMines();
        }
        #endregion

        #region PRIVATE_METHODS
        private Vector2Int GetRandomWalkableTile(List<TILE_TYPE> walkableTiles)
        {
            Tile tile = null;
            Vector2Int tileGridPos = Vector2Int.zero;
            do
            {
                tileGridPos = new Vector2Int(Random.Range(0, grid.Width), Random.Range(0, grid.Height));
                tile = grid.GetTile(tileGridPos.x, tileGridPos.y);
            }
            while (!walkableTiles.Contains(tile.type));

            return tileGridPos;
        }

        private Vector2Int[] GetRandomWalkableTiles(int amountRandomTiles, List<TILE_TYPE> walkableTiles, params Vector2Int[] exceptions)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            
            for (int i = 0; i < amountRandomTiles; i++)
            {
                int iterations = 0;
                Vector2Int tile;
            
                do
                {
                    iterations++;
                    tile = GetRandomWalkableTile(walkableTiles);
                }
                while ((tiles.Contains(tile) || exceptions.ToList().Contains(tile)) && iterations < (grid.Width * grid.Height));
                
                tiles.Add(tile);
            }

            return tiles.ToArray();
        }

        private List<TILE_TYPE> GetWalkableTilesForAllPathfinders()
        {
            List<TILE_TYPE> walkableTiles = MinerConstants.GetWalkableTiles();

            return walkableTiles;
        }
        #endregion
    }
}