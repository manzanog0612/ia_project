using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using IA.Game.Entity.UrbanCenterController;

using IA.Pathfinding;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinesController;
using IA.FSM.Entity.MinerController;
using IA.FSM.Entity.MinerController.Constants;
using IA.FSM.Entity.MinersController;
using IA.FSM.Entity.CarrouseController.Constants;
using IA.FSM.Entity.CarrousesController;

using Grid = IA.Pathfinding.Grid;

using TMPro;

namespace IA.Game.Controller
{
    public class GameController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Grid grid = null;
        [SerializeField] private MinersController minersController = null;
        [SerializeField] private MinesController minesController = null;
        [SerializeField] private CarrousesController carrousesController = null;
        [SerializeField] private Button btnPanicMode = null;

        [SerializeField] private GameObject urbanCenterPrefab = null;

        [Header("Game Configs")]
        [SerializeField] private int minersAmount = 4;
        [SerializeField] private int carrousesAmount = 4;
        [SerializeField] private int minesAmount = 6;
        #endregion

        #region PRIVATE_FIELDS
        private UrbanCenter urbanCenter = null;

        private bool panic = false;
        #endregion

        #region UNITY_CALLS
        private void Start()        
        {
            grid.Init();

            Camera.main.transform.position = new Vector3 (grid.RealWidth / 2f, grid.RealHeight / 2f, -10);
            Camera.main.orthographicSize = grid.RealWidth / 2f;

            urbanCenter = Instantiate(urbanCenterPrefab).GetComponent<UrbanCenter>();

            List<TILE_TYPE> allWalkableTiles = GetWalkableTilesForAllPathfinders();

            Vector2Int urbanCenterTile = GetRandomWalkableTile(allWalkableTiles);
            urbanCenter.Init(urbanCenterTile, grid.GetRealPosition(urbanCenterTile));

            Vector2Int[] minesTiles = GetRandomWalkableTiles(minesAmount, allWalkableTiles, urbanCenterTile);

            minesController.Init(minesTiles, grid.GetRealPosition);
            InitMiners();
            InitCarrouse();

            btnPanicMode.onClick.AddListener(() => SetPanic(!panic));
        }

        private void Update()
        {
            urbanCenter.UpdateText();
            minesController.UpdateMines();
            minersController.UpdateBehaviours();
            carrousesController.UpdateBehaviours();
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitCarrouse()
        {
            Func<Vector2, Mine> onGetMineOnPos = minesController.GetMineOnPos;
            Func<List<Miner>> onGetMinersMining = minersController.GetMinersMining;

            carrousesController.Init(carrousesAmount, grid, urbanCenter, onGetMineOnPos, onGetMinersMining);
        }

        private void InitMiners()
        {
            Func<Vector2, Mine> onGetMineOnPos = minesController.GetMineOnPos;
            Func<Mine[]> onGetMinesLeft = minesController.GetMinesLeft;

            minersController.Init(minersAmount, grid, urbanCenter, onGetMineOnPos, onGetMinesLeft);
        }

        private void SetPanic(bool status)
        {
            panic = status;

            if (status)
            {
                btnPanicMode.GetComponentInChildren<TextMeshProUGUI>().text = "Resume activities";
            }
            else
            {
                btnPanicMode.GetComponentInChildren<TextMeshProUGUI>().text = "Panic at the grid";
            }

            minersController.SetPanic(status);
            carrousesController.SetPanic(status);
        }

        private Vector2Int GetRandomWalkableTile(List<TILE_TYPE> walkableTiles)
        {
            Tile tile = null;
            Vector2Int tileGridPos = Vector2Int.zero;
            do
            {
                tileGridPos = new Vector2Int(UnityEngine.Random.Range(0, grid.Width), UnityEngine.Random.Range(0, grid.Height));
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
            List<TILE_TYPE> minerWalkableTiles = MinerConstants.GetWalkableTiles();
            List<TILE_TYPE> carrouseWalkableTiles = CarrouseConstants.GetWalkableTiles();
            List<TILE_TYPE> walkableTiles =  new List<TILE_TYPE>();

            for (int i = 0; i < minerWalkableTiles.Count; i++)
            {
                if (carrouseWalkableTiles.Contains(minerWalkableTiles[i]))
                {
                    walkableTiles.Add(minerWalkableTiles[i]);
                }
            }

            return walkableTiles;
        }
        #endregion
    }
}