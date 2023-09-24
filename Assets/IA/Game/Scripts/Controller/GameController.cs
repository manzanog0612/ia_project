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
            Vector2Int gridPos = new Vector2Int(Random.Range(0, grid.Width), Random.Range(0, grid.Height)); ;
            return grid.GetRealPosition(gridPos);
        }

        private Vector2Int[] GetRandomTiles(int amountRandomTiles)
        {
            List<Vector2Int> tiles = new List<Vector2Int>();

            //for (int i = 0; i < amountRandomTiles; i++)
            //{
            //    int iterations = 0;
            //    Vector2Int tile;
            //
            //    do
            //    {
            //        iterations++;
            //        tile = GetRandomTile();
            //    }
            //    while (tiles.Contains(tile) && iterations < amountRandomTiles);
            //    
            //    tiles.Add(tile);
            //}

            tiles.Add(new Vector2Int(1, 1));
            tiles.Add(new Vector2Int(1, 7));
            tiles.Add(new Vector2Int(7, 1));
            tiles.Add(new Vector2Int(7, 7));
            tiles.Add(new Vector2Int(4, 4));

            return tiles.ToArray();
        }
        #endregion
    }
}