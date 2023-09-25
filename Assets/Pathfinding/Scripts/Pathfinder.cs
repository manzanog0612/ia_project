using System.Collections.Generic;

using UnityEngine;

namespace IA.Pathfinding
{
    public class PathfinderTileData
    {
        public Tile fromTile = null;

        public int gCost = 0; // distance from starting node
        public int hCost = 0; // distance from end node

        public int fCost => gCost + hCost;
    }

    public class Pathfinder
    {
        private Grid grid;
        private Tile startTile;
        private Tile targetTile;

        private Dictionary<TILE_TYPE, int> tilesWeights;
        private Dictionary<TILE_TYPE, bool> tilesWalkableState;

        public void Init(Grid grid, Dictionary<TILE_TYPE, int> tilesWeights, Dictionary<TILE_TYPE, bool> walkableWeights)
        {
            this.grid = grid;
            this.tilesWeights = tilesWeights;
            this.tilesWalkableState = walkableWeights;
        }

        public List<Vector2> FindPath(Tile startTile, Tile targetTile)
        {
            this.startTile = startTile;
            this.targetTile = targetTile;

            return FindPath();
        }

        private List<Vector2> FindPath()
        {
            List<Tile> openTiles = new List<Tile>();
            List<Tile> closedTiles = new List<Tile>();

            Dictionary<(int,int), PathfinderTileData> tileCosts = GetGHCosts(startTile, targetTile);

            openTiles.Add(startTile);

            while (openTiles.Count > 0)
            {
                Tile currentTile = FindOpenTileWithLowestFCost(openTiles, tileCosts);
                openTiles.Remove(currentTile);
                closedTiles.Add(currentTile);

                if (currentTile == targetTile)
                {
                    break;
                }

                foreach (Tile neighbour in currentTile.neighbours)
                {
                    if (!neighbour.walkable || !tilesWalkableState[neighbour.type] || closedTiles.Contains(neighbour))
                    {
                        continue;
                    }

                    int currentTileGCost = tileCosts[(currentTile.x, currentTile.y)].gCost;
                    int neighbourTileGCost = tileCosts[(neighbour.x, neighbour.y)].gCost;

                    int newMovementCostToNeighbour = currentTileGCost + Dist(currentTile, neighbour) + tilesWeights[neighbour.type];
                    if (newMovementCostToNeighbour < neighbourTileGCost || !openTiles.Contains(neighbour))
                    {
                        tileCosts[(neighbour.x, neighbour.y)].gCost = newMovementCostToNeighbour;
                        tileCosts[(neighbour.x, neighbour.y)].hCost = Dist(neighbour, targetTile);
                        tileCosts[(neighbour.x, neighbour.y)].fromTile = currentTile;

                        if (!openTiles.Contains(neighbour))
                        {
                            openTiles.Add(neighbour);
                        }
                    }
                }
            }

            List<Vector2> path = new List<Vector2>();
            Tile tile = targetTile;

            while (tile != startTile)
            {
                path.Add(tile.pos);
                tile = tileCosts[(tile.x, tile.y)].fromTile;                
            }

            path.Reverse();

            return path;
        }

        private Dictionary<(int, int), PathfinderTileData> GetGHCosts(Tile start, Tile objective)
        {
            Dictionary<(int, int), PathfinderTileData> tileCosts = new Dictionary<(int, int), PathfinderTileData>();

            for (int i = 0; i < grid.Width; i++)
            {
                for (int j = 0; j < grid.Height; j++)
                {
                    Tile tile = grid.GetTile(i, j);

                    PathfinderTileData pathfinderTileData = new PathfinderTileData();
                    pathfinderTileData.gCost = Dist(start, tile);
                    pathfinderTileData.hCost = Dist(objective, tile);
                    tileCosts.Add((tile.x, tile.y), pathfinderTileData);
                }
            }

            return tileCosts;
        }

        private int Dist(Tile t1, Tile t2)
        {
            return Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y);
        }

        private Tile FindOpenTileWithLowestFCost(List<Tile> openTiles, Dictionary<(int, int), PathfinderTileData> tileCosts)
        {
            Tile lowestFCostTile = null;
            int lowestFCost = int.MaxValue;

            for (int i = 0; i < openTiles.Count; i++)
            {
                Tile openTile = openTiles[i];

                if (tileCosts[(openTile.x, openTile.y)].fCost < lowestFCost)
                {
                    lowestFCostTile = openTiles[i];
                    lowestFCost = tileCosts[(lowestFCostTile.x, lowestFCostTile.y)].fCost;
                }
            }

            return lowestFCostTile;
        }
    }
}