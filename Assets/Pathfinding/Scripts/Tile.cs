using System.Collections.Generic;

namespace IA.Pathfinding
{
    public class Tile
    {
        public int x;
        public int y;
        public List<Tile> neighbours;
        public bool walkable;
        public Tile fromTile = null;
        public int weight = 0;
    }
}
