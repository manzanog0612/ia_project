using System.Collections.Generic;

using IA.Pathfinding;

namespace IA.FSM.Entity.MinerController.Constants
{
    public class MinerConstants
    {
        public const float near = 2f;
        public const float detection = 5f;

        public const int inventoryCapacity = 10;
        public const float miningTime = 0.5f;

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
    }
}
