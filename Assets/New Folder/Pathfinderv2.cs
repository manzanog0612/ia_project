using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pathfinderv2 : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Vector2Int objetive;
    [SerializeField] private Vector2Int start;

    private Tile objetiveTile = null;
    private Tile actualTile = null;

    private Stack<Tile> walkedTiles = new Stack<Tile>();
    private List<Tile> openedTiles = new List<Tile>();
    private List<Tile> blockedTiles = new List<Tile>();

    private void Start()
    {
        objetiveTile = grid.GetTile(objetive);
        MoveToTile(grid.GetTile(start));
        transform.position = (Vector2)actualTile.position;
    }

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
            yield return new WaitForSeconds(1);

            transform.position = (Vector2)path[index].position;
            grid.SetWalked(path[index]);

            index++;
        }
    }

    private List<Tile> FindPath()
    {
        List<Tile> path = new List<Tile>();

        while (actualTile.position != objetive)
        {
            Tile nextTile = GetOpenedMinorDist();
            Tile fromTile = actualTile;

            if (!actualTile.neighbors.Contains(nextTile))
            {
                Tile walkedTile = actualTile;

                while (walkedTile != null && !walkedTile.neighbors.Contains(nextTile))
                {
                    BlockTile(walkedTile);
                    walkedTiles.Pop();
                    walkedTile = walkedTile.fromTile;
                }

                UpdateOpenedTilesStatus();
            }
            else
            {
                Tile tileWhomWasTheNeighborBefore = CheckForBetterPath(nextTile);

                if (tileWhomWasTheNeighborBefore != null)
                {
                    Tile walkedTile = actualTile;

                    while (walkedTile != tileWhomWasTheNeighborBefore)
                    {
                        BlockTile(walkedTile);
                        walkedTiles.Pop();
                        walkedTile = walkedTile.fromTile;
                    }

                    UpdateOpenedTilesStatus();

                    fromTile = tileWhomWasTheNeighborBefore;
                }
            }

            MoveToTile(nextTile, fromTile);
        }

        for (int i = walkedTiles.Count - 1; i >= 0; i--)
        {
            path.Add(walkedTiles.Pop());
        }

        path.Reverse();

        return path;
    }

    private void MoveToTile(Tile tile, Tile fromTile = null)
    {
        tile.fromTile = fromTile;
        actualTile = tile;
        OpenNeighborTiles();

        walkedTiles.Push(actualTile);
        openedTiles.Remove(tile);
    }

    private void OpenNeighborTiles()
    {
        for (int i = 0; i < actualTile.neighbors.Count; i++)
        {
            Tile neighborTile = actualTile.neighbors[i];

            if (CanTileBeOpened(neighborTile))
            {
                openedTiles.Add(neighborTile);
            }
        }
    }

    private Tile GetOpenedMinorDist()
    {
        Tile minorDistTile = null;
        int tileMinorDist = int.MaxValue;

        for (int i = 0; i < openedTiles.Count; i++)
        {
            int tileDist = Dist(objetiveTile, openedTiles[i]);

            if (tileDist < tileMinorDist)
            {
                tileMinorDist = tileDist;
                minorDistTile = openedTiles[i];
            }
        }

        return minorDistTile;
    }

    private int Dist(Tile t1, Tile t2)
    {
        return Mathf.Abs(t1.position.x - t2.position.x) + Mathf.Abs(t1.position.y - t2.position.y);
    }

    private bool CanTileBeOpened(Tile tile)
    {
        return !openedTiles.Contains(tile) && !walkedTiles.Contains(tile) && !blockedTiles.Contains(tile) && tile.walkable;
    }

    private void BlockTile(Tile tile)
    {
        blockedTiles.Add(tile);

        if (openedTiles.Contains(tile))
        {
            openedTiles.Remove(tile);
        }

        grid.SetBlocked(tile);
    }

    private void UpdateOpenedTilesStatus()
    {
        for (int i = openedTiles.Count - 1; i >= 0; i--)
        {
            bool found = false;

            if (walkedTiles.Contains(openedTiles[i]))
            {
                openedTiles.RemoveAt(i);
            }
            else
            {
                Stack<Tile> auxStack = new Stack<Tile>();

                for (int j = walkedTiles.Count - 1; j >= 0; j--)
                {
                    Tile tile = walkedTiles.Pop();
                    auxStack.Push(tile);

                    if (tile.neighbors.Contains(openedTiles[i]))
                    {
                        found = true;
                        break;
                    }
                }

                for (int j = auxStack.Count - 1; j >= 0; j--)
                {
                    walkedTiles.Push(auxStack.Pop());
                }

                if (!found)
                {
                    openedTiles.RemoveAt(i);
                }
            }
        }
    }

    private Tile CheckForBetterPath(Tile nextTile)
    {
        Tile tileWhomWasTheNeighborBefore = null;

        Tile previousTile = actualTile.fromTile;

        while (previousTile != null)
        {
            if (previousTile.neighbors.Contains(nextTile))
            {
                tileWhomWasTheNeighborBefore = previousTile;
            }

            previousTile = previousTile.fromTile;
        }

        return tileWhomWasTheNeighborBefore;
    }
}
