using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using UnityEngine;

using IA.FSM.Entity.MineController;

using IA.FSM.Entity.MinerController;
using IA.FSM.Entity.MinersController.Enums;
using IA.FSM.Entity.MinersController.States;

namespace IA.FSM.Entity.MinersController
{
    public class MinersController : MonoBehaviour
    {        
        [SerializeField] private LayerMask objectsLayerMask = default;
        [SerializeField] private string minerTag = null;
        [SerializeField] private string mineTag = null;

        private List<Miner> selectedMiners = new List<Miner>();
        private List<Mine> selectedMines = new List<Mine>();

        private ConcurrentBag<Miner> miners = new ConcurrentBag<Miner>();

        private FSM fsm;

        private void Start()
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

            Miner[] minersArray = FindObjectsOfType<Miner>();

            for (int i = 0; i < minersArray.Length; i++)
            {
                miners.Add(minersArray[i]);
            }
        }

        private void Update()
        {
            Parallel.ForEach(miners,
                miner =>
                {
                    miner.MinerBehaviour.UpdateFsm();
                });

            fsm.Update();
        }

    }
}
