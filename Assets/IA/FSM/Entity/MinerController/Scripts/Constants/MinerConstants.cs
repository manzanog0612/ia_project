using System;
using System.Collections.Generic;

using IA.Pathfinding;

namespace IA.FSM.Entity.MinerController.Constants
{
    public class MinerConstants
    {
        public const int inventoryCapacity = 15;
        public const int foodCapacity = 3;
        public const float miningTime = 0.5f;

        public static float GetMovementSpeed()
        {
            Random random = new Random();
            return 0.5f + random.Next(1, 10) * 0.1f;
        }

        public static Dictionary<TILE_TYPE, int> GetTileWeigths()
        {
            return new()
            {
                { TILE_TYPE.DIRT, 1 },
                { TILE_TYPE.COBBLESTONE, 2 },
                { TILE_TYPE.SAND, 4 },
                { TILE_TYPE.WATER, 32 }
            };
        }

        public static Dictionary<TILE_TYPE, bool> GetTilesWalkableState()
        {
            return new()
            {
                { TILE_TYPE.DIRT, true },
                { TILE_TYPE.COBBLESTONE, true },
                { TILE_TYPE.SAND, true },
                { TILE_TYPE.WATER, false }
            };
        }

        public static List<TILE_TYPE> GetWalkableTiles()
        {
            List<TILE_TYPE> walkableTiles = new List<TILE_TYPE>();
            Dictionary<TILE_TYPE, bool> tilesWalkableState = GetTilesWalkableState();

            foreach (KeyValuePair<TILE_TYPE, bool> tileState in tilesWalkableState)
            {
                if (tileState.Value)
                {
                    walkableTiles.Add(tileState.Key);
                }
            }

            return walkableTiles;
        }
    }
}
