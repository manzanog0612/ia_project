using System.Collections.Generic;

using IA.Pathfinding;

namespace IA.FSM.Entity.CarrouseController.Constants
{
    public class CarrouseConstants
    {
        public static Dictionary<TILE_TYPE, int> GetTileWeigths()
        {
            return new()
            {
                { TILE_TYPE.DIRT, 2 },
                { TILE_TYPE.COBBLESTONE, 1 },
                { TILE_TYPE.SAND, 8 },
                { TILE_TYPE.WATER, 32 }
            };
        }
    }
}