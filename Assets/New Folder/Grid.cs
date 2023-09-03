using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private int width = 9;
    [SerializeField] private int height = 9;
    [SerializeField] private GameObject prefabTile = null;

    [SerializeField] private Material notWalkableMat = null;
    [SerializeField] private Material blockedMat = null;
    [SerializeField] private Material walkedMat = null;

    private List<Tile> gridTiles = new List<Tile>();

    private void Awake()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Tile tile = Instantiate(prefabTile, new Vector3(i, j, 0), Quaternion.identity, transform).GetComponent<Tile>();
                tile.gameObject.name = "X:" + i + " Y:" + j;
                tile.position = new Vector2Int(i, j);

                if ((j == 3 && i < 5) || (i == 4 && (j != 7 && j > 2 )))//(i == 4 && (j > 1 && j < 7)) || (j == 4 && (i > 1 && i < 7)))
                {
                    tile.meshRenderer.material = notWalkableMat;
                    tile.walkable = false;
                }
                else
                {
                    tile.walkable = true;
                }

                gridTiles.Add(tile);
            }
        }

        for (int i = 0; i < gridTiles.Count; i++)
        {
            gridTiles[i].neighbors = FindNeighbours(gridTiles[i].position);
        }
    }

    private List<Tile> FindNeighbours(Vector2Int position)
    {
        List<Tile> tiles = gridTiles.FindAll(t => (t.position.x == position.x &&
                                                  (t.position.y == position.y + 1 || t.position.y == position.y - 1)) ||
                                                  (t.position.y == position.y &&
                                                  (t.position.x == position.x + 1 || t.position.x == position.x - 1)));

        return tiles;
    }

    public Tile GetTile(int x, int y)
    {
        return gridTiles.Find(t => t.position.x == x && t.position.y == y);
    }

    public Tile GetTile(Vector2Int pos)
    {
        return gridTiles.Find(t => t.position.x == pos.x && t.position.y == pos.y);
    }

    public void SetBlocked(Tile tile)
    {
        tile.meshRenderer.material = blockedMat;
    }

    public void SetWalked(Tile tile)
    {
        tile.meshRenderer.material = walkedMat;
    }
}
