using UnityEngine;

using System.Collections.Generic;

using IA.FSM.Entity.MineController;

namespace IA.FSM.Entity.MinesController
{
    public class MinesController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject minePrefab = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<Mine> mines = new List<Mine>();
        #endregion

        #region PUBLIC_METHODS
        public void Init(Vector2Int[] minesPositions)
        {
            for (int i = 0; i < minesPositions.Length; i++)
            {
                Mine mine = Instantiate(minePrefab, transform).GetComponent<Mine>();
                mine.Init(minesPositions[i]);
                mines.Add(mine);
            }
        }

        public void UpdateMines()
        {
            for (int i = 0; i < mines.Count; i++)
            {
                mines[i].UpdateText();
            }
        }
        #endregion
    }
}
