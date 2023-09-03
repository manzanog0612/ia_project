using System.Collections.Generic;

using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public List<Tile> neighbors;
    public bool walkable;
    public Tile fromTile = null;


    public MeshRenderer meshRenderer;
}
