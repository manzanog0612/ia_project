using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using UnityEngine;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController;
using IA.FSM.Entity.MinersController.Enums;
using IA.FSM.Entity.MinersController.States;
using IA.Game.Entity.UrbanCenterController;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Entity.MinersController
{
    public class MinersController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private LayerMask objectsLayerMask = default;
        [SerializeField] private string minerTag = null;
        [SerializeField] private string mineTag = null;

        [SerializeField] private GameObject minerPrefab = null;
        [SerializeField] private Transform minersHolder = null;
        #endregion

        #region PRIVATE_FIELDS
        private List<Miner> selectedMiners = new List<Miner>();
        private List<Mine> selectedMines = new List<Mine>();

        private ConcurrentBag<Miner> minersFsm = new ConcurrentBag<Miner>();
        private Miner[] miners = null;

        private FSM fsm;
        #endregion

        #region PUBLIC_METHODS
        public void Init(int minersAmount, Grid grid, UrbanCenter urbanCenter, Vector2Int[] minesTiles, Func<Vector2, Mine> onGetMineOnPos)
        {
            fsm = new FSM(Enum.GetValues(typeof(Enums.States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)Enums.States.Idle, (int)Flags.OnSelectObject, (int)Enums.States.SelectingObjects);

            fsm.SetRelation((int)Enums.States.SelectingObjects, (int)Flags.OnStartArragement, (int)Enums.States.ArrangingMiners);

            fsm.SetRelation((int)Enums.States.ArrangingMiners, (int)Flags.OnArragementApplied, (int)Enums.States.Idle);

            fsm.AddState<IdleState>((int)Enums.States.Idle,
                () => (new object[1] { objectsLayerMask }));

            fsm.AddState<SelectingObjectsState>((int)Enums.States.SelectingObjects,
                () => (new object[5] { objectsLayerMask, selectedMiners, selectedMines, minerTag, mineTag }), null,
                () => (new object[2] { selectedMiners, selectedMines }));

            fsm.AddState<ArrangingMinersState>((int)Enums.States.ArrangingMiners,
                () => (new object[2] { selectedMiners, selectedMines }), null,
                () => (new object[2] { selectedMiners, selectedMines }));

            fsm.SetCurrentStateForced((int)Enums.States.Idle);

            miners = new Miner[minersAmount];

            for (int i = 0; i < miners.Length; i++)
            {
                miners[i] = Instantiate(minerPrefab, minersHolder).GetComponent<Miner>();
                miners[i].Init(urbanCenter, grid, minesTiles, onGetMineOnPos);
                minersFsm.Add(miners[i]);
            }
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

            fsm.Update();
        }
        #endregion
    }
}
