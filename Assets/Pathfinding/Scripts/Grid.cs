using UnityEngine;

using System.Collections.Generic;
using System;

namespace IA.Pathfinding
{
    public class Grid : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private int width = 9;
        [SerializeField] private int height = 9;
        [SerializeField] private float distanceBetweenNodes = 0.5f;
        [SerializeField] private GameObject prefabTile = null;

        [SerializeField] private Material dirtMat = null;
        [SerializeField] private Material cobblestoneMat = null;
        [SerializeField] private Material sandMat = null;
        [SerializeField] private Material waterMat = null;
        [SerializeField] private Material limitMat = null;
        #endregion

        #region PRIVATE_FIELDS
        private Tile[,] grid = null;
        private GameObject[,] gridGOs = null;
        #endregion

        #region PROPERTIES
        public int Width { get => width - 2; }
        public int Height { get => height - 2; }
        public int RealWidth { get => width; }
        public int RealHeight { get => height; }
        #endregion

        #region PUBLIC_METHODS
        public void Init()
        {
            width += 2;
            height += 2;

            grid = new Tile[width, height];
            gridGOs = new GameObject[width, height];

            Dictionary<TILE_TYPE, Material> tileTypeMaterials = new Dictionary<TILE_TYPE, Material>()
            {
                { TILE_TYPE.DIRT, dirtMat },
                { TILE_TYPE.COBBLESTONE, cobblestoneMat },            
                { TILE_TYPE.SAND, sandMat },
                { TILE_TYPE.WATER, waterMat },
                { TILE_TYPE.LIMIT, limitMat }
            };

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 pos = new Vector2(x + distanceBetweenNodes * x, y + distanceBetweenNodes * y);
                    GameObject tileGO = Instantiate(prefabTile, pos, Quaternion.identity, transform);
                    tileGO.gameObject.name = "X:" + x + " Y:" + y;

                    Tile tile = new Tile();
                    tile.x = x;
                    tile.y = y;
                    tile.pos = pos;

                    if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                    {
                        tile.type = TILE_TYPE.LIMIT;
                    }
                    else
                    {
                        tile.type = (TILE_TYPE)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TILE_TYPE)).Length - 1);
                    }
                    
                    tileGO.GetComponent<MeshRenderer>().material = tileTypeMaterials[tile.type];
                    
                    grid[x, y] = tile;
                    gridGOs[x, y] = tileGO;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y].neighbours = FindNeighbours(x, y);
                }
            }
        }

        public Tile GetTile(int x, int y)
        {
            return grid[x + 1, y + 1];
        }

        public Vector2Int GetRealPosition(Vector2Int gridPosition)
        {
            Tile tile = GetTile(gridPosition.x, gridPosition.y);

            return new Vector2Int(tile.x, tile.y);
        }
        #endregion

        #region PRIVATE_METHODS
        private List<Tile> FindNeighbours(int posX, int posY)
        {
            List<Tile> tiles = new List<Tile>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile t = grid[x, y];
                    if ((t.x == posX &&
                        (t.y == posY + 1 || t.y == posY - 1)) ||
                        (t.y == posY &&
                        (t.x == posX + 1 || t.x == posX - 1)))
                    {
                        tiles.Add(t);
                    }
                }
            }

            return tiles;
        }

        #endregion
    }
}