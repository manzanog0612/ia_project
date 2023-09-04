using System.Collections.Generic;

using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public List<Tile> neighbours;
    public bool walkable;
    public Tile fromTile = null;
    public int weight = 0;

    public int gCost = 0; // distance from starting node
    public int hCost = 0; // distance from end node

    public int fCost => gCost + hCost;

    public MeshRenderer meshRenderer;
}
