using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

using UnityEngine;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController;
using IA.Game.Entity.UrbanCenterController;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Entity.MinersController
{
    public class MinersController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject minerPrefab = null;
        [SerializeField] private Transform minersHolder = null;
        #endregion

        #region PRIVATE_FIELDS
        private ConcurrentBag<Miner> minersFsm = new ConcurrentBag<Miner>();
        private Miner[] miners = null;
        #endregion

        #region PUBLIC_METHODS
        public void Init(int minersAmount, Grid grid, UrbanCenter urbanCenter, Func<Vector2, Mine> onGetMineOnPos, Func<Mine[]> onGetAllMinesLeft)
        {
            miners = new Miner[minersAmount];

            for (int i = 0; i < miners.Length; i++)
            {
                miners[i] = Instantiate(minerPrefab, minersHolder).GetComponent<Miner>();
                miners[i].Init(urbanCenter, grid);
                miners[i].InitBehaviour(onGetMineOnPos, onGetAllMinesLeft);
                minersFsm.Add(miners[i]);
            }
        }

        public List<Miner> GetMinersMining()
        {
            return miners.ToList().FindAll(m => m.MinerBehaviour.ActualState == MinerController.Enums.States.Mining || 
                                                m.MinerBehaviour.ActualState == MinerController.Enums.States.WaitingForFood);
        }

        public void UpdateBehaviours()
        {
            Parallel.ForEach(minersFsm,
                miner =>
                {
                    miner.MinerBehaviour.UpdateFsm();
                });

            for (int i = 0; i < minersFsm.Count; i++)
            {
                miners[i].UpdateBehaviour();
            }
        }
        #endregion
    }
}
