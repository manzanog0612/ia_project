using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Vector2Int objetive;
    [SerializeField] private Vector2Int start;
    [SerializeField] private float secondsInterval;

    private Tile actualTile = null;
    private int actualDist = 0;

    private Tile objetiveTile = null;
    private int maxDistFound = 0;
    //private int maxCount = 0;
    //private int count = 0;

    private List<Tile> walkedTiles = new List<Tile>();
    private List<Tile> openedTiles = new List<Tile>();
    private List<Tile> blockedTiles = new List<Tile>();
    //private Dictionary<Tile, Tile> tileBeforeTile = new Dictionary<Tile, Tile>();

    private void Start()
    {
        objetiveTile = grid.GetTile(objetive);
        MoveToTile(grid.GetTile(start));
        maxDistFound = 0;

        StartCoroutine(FindPath());
    }

    private IEnumerator FindPath()
    {
        while (actualTile.position != objetive)
        {
            yield return new WaitForSeconds(secondsInterval);

            Tile nextTile = null;
            Tile neighbourTile = null;

            int minorDistNeighbour = 9999;

            for (int i = 0; i < actualTile.neighbours.Count; i++)
            {
                neighbourTile = actualTile.neighbours[i];
                int tileDist = Dist(objetiveTile, neighbourTile);

                if (CanTileBeOpened(neighbourTile))
                {
                    if (tileDist < minorDistNeighbour)
                    {
                        if (tileDist < actualDist)
                        {
                            nextTile = neighbourTile;
                        }

                        minorDistNeighbour = tileDist;
                    }

                    openedTiles.Add(neighbourTile);
                }
                else
                {
                    if (maxDistFound < tileDist)
                    {
                        maxDistFound = tileDist;
                    }
                }
            }

            if (nextTile == null)
            {
                for (int i = walkedTiles.Count - 1; i >= 0; i--)
                {
                    List<Tile> tiles = new List<Tile>();
                    tiles.AddRange(walkedTiles[i].neighbours.FindAll(t => !walkedTiles.Contains(t)));
                    tiles.Add(walkedTiles[i]);

                    Tile closerWalkedTile = tiles.Find(t => Dist(objetiveTile, neighbourTile) <= minorDistNeighbour);

                    BlockTile(walkedTiles[i]);

                    if (closerWalkedTile != null && CanTileBeOpened(closerWalkedTile))
                    {
                        if (Dist(closerWalkedTile, objetiveTile) < Dist(grid.GetTile(start), objetiveTile))
                        {
                            nextTile = closerWalkedTile;
                        }
                        else
                        {
                            nextTile = grid.GetTile(start);
                        }
                        break;
                    }
                }
            }

            if (nextTile == null)
            {

            }

            MoveToTile(nextTile);
            
        }

        Debug.Log("WIN");
    }

    private void MoveToTile(Tile tile)
    {
        //if (actualTile != null)
        //{ 
        //    tileBeforeTile.Add(actualTile, nextTile); 
        //}

        actualTile = tile;
        transform.position = (Vector2)actualTile.position;
        walkedTiles.Add(actualTile);
        grid.SetWalked(tile);
        actualDist = Dist(objetiveTile, actualTile);
    }

    private int Dist(Tile t1, Tile t2)
    {
        return Mathf.Abs(t1.position.x - t2.position.x) + Mathf.Abs(t1.position.y - t2.position.y);
    }

    private bool CanTileBeOpened(Tile tile)
    {
        return !walkedTiles.Contains(tile) && !blockedTiles.Contains(tile) && tile.walkable;
    }

    private void BlockTile(Tile tile)
    {
        blockedTiles.Add(tile);

        if (walkedTiles.Contains(tile))
        { 
            walkedTiles.Remove(tile); 
        }

        if (openedTiles.Contains(tile))
        {
            openedTiles.Remove(tile);
        }

        grid.SetBlocked(tile);
    }
}
