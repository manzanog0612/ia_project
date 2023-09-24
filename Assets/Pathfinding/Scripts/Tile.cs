using System.Collections.Generic;
using UnityEngine;

namespace IA.Pathfinding
{
    //Carrouse              2     1            8     32
    //Villager              1     2            4     16     
    public enum TILE_TYPE { DIRT, COBBLESTONE, SAND, WATER, LIMIT }

    public class Tile
    {
        public int x;
        public int y;
        public Vector2 pos;
        public List<Tile> neighbours;
        public bool walkable;
        public TILE_TYPE type = TILE_TYPE.DIRT;
    }
}
