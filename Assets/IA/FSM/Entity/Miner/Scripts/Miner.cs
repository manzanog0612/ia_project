using System;

using UnityEngine;

using IA.FSM.Entity.Miner.Enum;
using IA.FSM.Entity.Miner.States;
using IA.FSM.Entity.Miner.Constants;

using IA.Background.Entity.Home;

using TMPro;

namespace IA.FSM.Entity.Miner
{
    public class Miner : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtInventory = null;

        public Mine.Mine targetMine = null;
        public Home home = null;
        public bool setNewMine = false;
        
        public float speed = 5;
        public int inventory = 0;

        private float miningTimer = 0; 

        private FSM fsm;

        private void Start()
        {
            fsm = new FSM(System.Enum.GetValues(typeof(Enum.States)).Length, System.Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)Enum.States.Idle, (int)Flags.OnSetMine, (int)Enum.States.GoingToMine);

            fsm.SetRelation((int)Enum.States.GoingToMine, (int)Flags.OnReachMine, (int)Enum.States.Mining);

            fsm.SetRelation((int)Enum.States.Mining, (int)Flags.OnEmptyMine, (int)Enum.States.ReturningToHome);
            fsm.SetRelation((int)Enum.States.Mining, (int)Flags.OnFullInventory, (int)Enum.States.ReturningToHome);
            fsm.SetRelation((int)Enum.States.Mining, (int)Flags.OnSetMine, (int)Enum.States.GoingToMine);

            fsm.SetRelation((int)Enum.States.ReturningToHome, (int)Flags.OnReachHome, (int)Enum.States.Idle);

            Action onMine = Mine;
            Action onLeaveMineralsInHome = LeaveMineralsInHome;

            fsm.AddState<IdleState>((int)Enum.States.Idle, 
                () => (new object[1] { setNewMine }));

            fsm.AddState<GoingToMineState>((int)Enum.States.GoingToMine,
               () => (new object[3] { transform, targetMine.transform, speed }));

            fsm.AddState<MiningState>((int)Enum.States.Mining,
               () => (new object[3] { targetMine, inventory, onMine }),
               () => (new object[2] { MinerConstants.miningTime, MinerConstants.inventoryCapacity }));

            fsm.AddState<ReturningToHome>((int)Enum.States.ReturningToHome,
               () => (new object[3] { transform, home.transform, speed }),
               () => (new object[1] { onLeaveMineralsInHome }));

            fsm.SetCurrentStateForced((int)Enum.States.Idle);

            txtInventory.text = inventory.ToString();
        }

        private void Update()
        {
            fsm.Update();
        }

        public void SetMine(Mine.Mine targetMine) 
        {
            this.targetMine = targetMine;
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