using System;
using System.Collections.Generic;

using IA.Pathfinding;

namespace IA.FSM.Entity.CarrouseController.Constants
{
    public class CarrouseConstants
    {
        public static float GetMovementSpeed()
        {
            Random random = new Random();
            return 1.5f + random.Next(1, 5) * 0.2f;
        }

        public static Dictionary<TILE_TYPE, int> GetTileWeigths()
        {
            return new()
            {
                { TILE_TYPE.DIRT, 2 },
                { TILE_TYPE.COBBLESTONE, 1 },
                { TILE_TYPE.SAND, 32 },
                { TILE_TYPE.WATER, 4 }
            };
        }

        public static Dictionary<TILE_TYPE, bool> GetTilesWalkableState()
        {
            return new()
            {
                { TILE_TYPE.DIRT, true },
                { TILE_TYPE.COBBLESTONE, true },
                { TILE_TYPE.SAND, false },
                { TILE_TYPE.WATER, true }
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