using System.Collections.Generic;

namespace IA.Pathfinding
{
    //Carrouse              2     1            8     32
    //Villager              1     2            4     16     
    public enum TILE_TYPE { DIRT, COBBLESTONE, SAND, WATER }

    public class Tile
    {
        public int x;
        public int y;
        public List<Tile> neighbours;
        public bool walkable;
        public TILE_TYPE type = TILE_TYPE.DIRT;
    }
}
