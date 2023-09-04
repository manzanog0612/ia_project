using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pathfinderv3 : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Vector2Int objetive;
    [SerializeField] private Vector2Int start;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TravelPath(FindPath()));
        }
    }

    private IEnumerator TravelPath(List<Tile> path)
    {
        int index = 0;
        while (new Vector2Int((int)transform.position.x, (int)transform.position.y) != objetive)
        {
            yield return new WaitForSeconds(0.5f);

            transform.position = (Vector2)path[index].position;
            grid.SetWalked(path[index]);

            index++;
        }
    }

    private List<Tile> FindPath()
    {
        List<Tile> openTiles = new List<Tile>();
        List<Tile> closedTiles = new List<Tile>();

        Tile objetiveTile = grid.GetTile(objetive);
        Tile startTile = grid.GetTile(start);
        grid.ConfigureGHCosts(startTile, objetiveTile);

        grid.SetWalked(startTile);
        transform.position = (Vector2)startTile.position;

        openTiles.Add(startTile);

        while (openTiles.Count > 0)
        {
            Tile currentTile = FindOpenTileWithLowestFCost(openTiles);
            openTiles.Remove(currentTile);
            closedTiles.Add(currentTile);

            if (currentTile == objetiveTile)
            {
                break;
            }

            foreach (Tile neighbour in currentTile.neighbours)
            {
                if (!neighbour.walkable || closedTiles.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentTile.gCost + Dist(currentTile, neighbour) + neighbour.weight;
                if (newMovementCostToNeighbour < neighbour.gCost || !openTiles.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = Dist(neighbour, objetiveTile);
                    neighbour.fromTile = currentTile;

                    if (!openTiles.Contains(neighbour))
                    {
                        openTiles.Add(neighbour);
                    }
                }
            }
        }

        List<Tile> path = new List<Tile>();
        Tile tile = objetiveTile;

        while (tile != startTile)
        {
            path.Add(tile);
            tile = tile.fromTile;
        }

        path.Reverse();

        return path;
    }

    private Tile FindOpenTileWithLowestFCost(List<Tile> openTiles)
    {
        Tile lowestFCostTile = null;
        int lowestFCost = int.MaxValue;

        for (int i = 0; i < openTiles.Count; i++)
        {
            if (openTiles[i].fCost < lowestFCost)
            {
                lowestFCostTile = openTiles[i];
                lowestFCost = lowestFCostTile.fCost;
            }
        }

        return lowestFCostTile;
    }

    private int Dist(Tile t1, Tile t2)
    {
        return Mathf.Abs(t1.position.x - t2.position.x) + Mathf.Abs(t1.position.y - t2.position.y);
    }
}
