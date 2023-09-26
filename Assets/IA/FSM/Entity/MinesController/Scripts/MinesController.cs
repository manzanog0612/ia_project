using UnityEngine;

using System.Collections.Generic;

using IA.FSM.Entity.MineController;
using System;

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
        public void Init(Vector2Int[] minesTiles, Func<Vector2Int, Vector2> onGetTilePos)
        {
            for (int i = 0; i < minesTiles.Length; i++)
            {
                Mine mine = Instantiate(minePrefab, transform).GetComponent<Mine>();
                mine.Init(minesTiles[i], onGetTilePos.Invoke(minesTiles[i]));
                mines.Add(mine);
            }
        }

        public Mine GetMineOnPos(Vector2 position)
        {
            return mines.Find(m => m.Position == position);
        }

        public void UpdateMines()
        {
            for (int i = 0; i < mines.Count; i++)
            {
                mines[i].UpdateMine();
            }
        }

        public Mine[] GetMinesLeft()
        {
            return mines.FindAll(m => m.Minerals > 0).ToArray();
        }
        #endregion
    }
}
