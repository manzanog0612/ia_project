using System;

using UnityEngine;

using IA.FSM.Entity.Miner.Constants;
using IA.FSM.Entity.Miner.Enums;
using IA.FSM.Entity.Miner.States;

using IA.Background.Entity.Home;

namespace IA.FSM.Entity.Miner
{
    public class MinerBehaviour
    {
        #region PRIVATE_METHODS
        private Mine.Mine targetMine = null;
        private Home home = null;

        private Vector3 position = Vector3.zero;
        private float speed = 8;
        private int inventory = 0;
        private bool setNewMine = false;

        private FSM fsm;

        private float deltaTime = 0;
        #endregion

        #region PROPERTIES
        public int Inventory { get => inventory; }
        public Vector3 Position { get => position; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Vector3 initialPostion, Home home, Action onLeaveMineralsInHome)
        {
            this.home = home;
            position = initialPostion;

            fsm = new FSM(Enum.GetValues(typeof(Enums.States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)Enums.States.Idle, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);

            fsm.SetRelation((int)Enums.States.GoingToMine, (int)Flags.OnReachMine, (int)Enums.States.Mining);

            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnEmptyMine, (int)Enums.States.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnFullInventory, (int)Enums.States.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);

            fsm.SetRelation((int)Enums.States.ReturningToHome, (int)Flags.OnReachHome, (int)Enums.States.Idle);

            Action OnLeaveIdle = UpdateSetNewMine;
            Action OnMine = () => inventory++;
            Action<Vector3> OnSetPosition = SetPosition;
            Action OnLeaveMineralsInHome = () =>
            {
                onLeaveMineralsInHome.Invoke();
                inventory = 0;
            };

            fsm.AddState<IdleState>((int)Enums.States.Idle,
                () => (new object[1] { setNewMine }), null,
                () => (new object[1] { OnLeaveIdle }));

            fsm.AddState<GoingToMineState>((int)Enums.States.GoingToMine,
               () => (new object[5] { OnSetPosition, position, targetMine.Position, speed, deltaTime }));

            fsm.AddState<MiningState>((int)Enums.States.Mining,
               () => (new object[4] { targetMine, inventory, OnMine, deltaTime }),
               () => (new object[2] { MinerConstants.miningTime, MinerConstants.inventoryCapacity }));

            fsm.AddState<ReturningToHome>((int)Enums.States.ReturningToHome,
               () => (new object[5] { OnSetPosition, position, this.home.Position, speed, deltaTime }),
               () => (new object[1] { OnLeaveMineralsInHome }));

            fsm.SetCurrentStateForced((int)Enums.States.Idle);
        }

        public void UpdateFsm()
        {
            fsm.Update();
        }

        public void SetDeltaTime(float deltaTime)
        {
            this.deltaTime = deltaTime;
        }

        public void SetMine(Mine.Mine targetMine)
        {
            this.targetMine = targetMine;
            setNewMine = true;
        }

        public void SetPosition(Vector3 pos)
        {
            position = pos;
        }
        #endregion

        #region PRIVATE_METHODS
        private void UpdateSetNewMine()
        {
            setNewMine = false;
        }
        #endregion
    }
}
