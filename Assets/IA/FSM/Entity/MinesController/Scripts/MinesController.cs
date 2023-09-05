using UnityEngine;

using System.Collections.Generic;

namespace IA.FSM.Entity.MinesController
{
    public class MinesController : MonoBehaviour
    {
        [SerializeField] private Pathfinding.Grid grid = null;
        [SerializeField] private List<Vector2Int> minesPositions = new List<Vector2Int>();
        [SerializeField] private GameObject minePrefab = null;

        private List<Mine.Mine> mines = new List<Mine.Mine>();

        private void Start()
        {
            for (int i = 0; i < minesPositions.Count; i++)
            {
                Mine.Mine mine = Instantiate(minePrefab, transform).GetComponent<Mine.Mine>();
                mine.Init(new Vector3(minesPositions[i].x, 0, minesPositions[i].y));
                mines.Add(mine);
            }
        }
    }
}
