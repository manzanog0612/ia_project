using UnityEngine;

using System.Collections.Generic;

namespace IA.Pathfinding
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private int width = 9;
        [SerializeField] private int height = 9;
        [SerializeField] private GameObject prefabTile = null;

        [SerializeField] private Material notWalkableMat = null;
        [SerializeField] private Material walkedMat = null;
        [SerializeField] private Material gridMat = null;

        private Tile[,] grid =null;
        private GameObject[,] gridGOs = null;

        public int Width { get => width; }
        public int Height { get => height; }

        private void Awake()
        {
            grid = new Tile[width, height];
            gridGOs = new GameObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject tileGO = Instantiate(prefabTile, new Vector3(x, 0, y), Quaternion.identity, transform);
                    tileGO.gameObject.name = "X:" + x + " Y:" + y;

                    Tile tile = new Tile();
                    tile.x = x; 
                    tile.y = y;

                    if ((y == 3 && x < 5) || (x == 4 && (y != 7 && y > 2)))//(i == 4 && (j > 1 && j < 7)) || (j == 4 && (i > 1 && i < 7)))
                    {
                        tileGO.GetComponent<MeshRenderer>().material = notWalkableMat;
                        tile.walkable = false;
                    }
                    else
                    {
                        tile.walkable = true;

                        if ((x == 3 || x == 2) && (y != 7 && y > 3))
                        {
                            tile.weight = 2;
                        }
                        else if (x == 1 && y == 7)
                        {
                            tile.weight = 30;
                        }
                        else
                        {
                            tile.weight = 1;
                        }

                        Material material = new Material(gridMat);
                        material.color *= new Color(material.color.r - 0.2f * tile.weight, material.color.g, material.color.b);

                        tileGO.GetComponent<MeshRenderer>().material = material;
                    }

                    grid[x,y] = tile;
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
            return grid[x, y];
        }

        public int Dist(Tile t1, Tile t2)
        {
            return Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y);
        }

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
    }
}