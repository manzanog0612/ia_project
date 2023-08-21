using System;

using UnityEngine;

using IA.FSM.Entity.Miner.Enums;
using IA.FSM.Entity.Miner.States;
using IA.FSM.Entity.Miner.Constants;

using IA.Background.Entity.Home;

using IA.Common.Entity.SelectableObject;

using TMPro;

namespace IA.FSM.Entity.Miner
{
    public class Miner : SelectableObject
    {
        [Header("Miner")]
        [SerializeField] private TextMeshProUGUI txtInventory = null;

        public Home home = null;
        
        public float speed = 5;
        public int inventory = 0;

        private Mine.Mine targetMine = null;
        private bool setNewMine = false;
        
        private FSM fsm;

        private void Start()
        {
            fsm = new FSM(Enum.GetValues(typeof(Enums.States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)Enums.States.Idle, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);

            fsm.SetRelation((int)Enums.States.GoingToMine, (int)Flags.OnReachMine, (int)Enums.States.Mining);

            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnEmptyMine, (int)Enums.States.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnFullInventory, (int)Enums.States.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);

            fsm.SetRelation((int)Enums.States.ReturningToHome, (int)Flags.OnReachHome, (int)Enums.States.Idle);

            Action onLeaveIdle = UpdateSetNewMine;
            Action onMine = Mine;
            Action onLeaveMineralsInHome = LeaveMineralsInHome;

            fsm.AddState<IdleState>((int)Enums.States.Idle, 
                () => (new object[1] { setNewMine }), null,
                () => (new object[1] { onLeaveIdle }));

            fsm.AddState<GoingToMineState>((int)Enums.States.GoingToMine,
               () => (new object[3] { transform, targetMine.transform, speed }));

            fsm.AddState<MiningState>((int)Enums.States.Mining,
               () => (new object[3] { targetMine, inventory, onMine }),
               () => (new object[2] { MinerConstants.miningTime, MinerConstants.inventoryCapacity }));

            fsm.AddState<ReturningToHome>((int)Enums.States.ReturningToHome,
               () => (new object[3] { transform, home.transform, speed }),
               () => (new object[1] { onLeaveMineralsInHome }));

            fsm.SetCurrentStateForced((int)Enums.States.Idle);

            txtInventory.text = inventory.ToString();
        }

        private void Update()
        {
            fsm.Update();
        }

        public void SetMine(Mine.Mine targetMine) 
        {
            this.targetMine = targetMine;
            setNewMine = true;
        }

        private void UpdateSetNewMine()
        {
            setNewMine = false;
        }

        private void Mine()
        {
            inventory++;
            txtInventory.text = inventory.ToString();
        }

        private void LeaveMineralsInHome()
        {
            home.PlaceMinerals(inventory);
            inventory = 0;
            txtInventory.text = inventory.ToString();
        }
    }
}