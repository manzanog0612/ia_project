using System;

using UnityEngine;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController.Constants;
using IA.FSM.Entity.MinerController.Enums;
using IA.FSM.Entity.MinerController.States;
using IA.Game.Entity.UrbanCenterController;
using IA.Pathfinding;

namespace IA.FSM.Entity.MinerController
{
    public class MinerBehaviour
    {
        #region PRIVATE_FIELDS
        private Mine targetMine = null;
        private UrbanCenter home = null;

        private Vector3 position = Vector3.zero;
        private float speed = 8;
        private int inventory = 0;
        private bool setNewMine = false;

        private FSM fsm;
        private Pathfinder pathfinder = null;

        private float deltaTime = 0;
        #endregion

        #region PROPERTIES
        public int Inventory { get => inventory; }
        public Vector3 Position { get => position; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Pathfinder pathfinder, Vector2Int initiaTile, UrbanCenter home, Action onLeaveMineralsInHome, Func<int,int,Tile> onGetTile)
        {
            this.pathfinder = pathfinder;
            this.home = home;

            position = new Vector3(initiaTile.x, 0, initiaTile.y);

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
               () => (new object[4] { OnSetPosition, position, speed, deltaTime }),
               () => (new object[3] { onGetTile.Invoke(initiaTile.x, initiaTile.y), onGetTile.Invoke(targetMine.Tile.x, targetMine.Tile.y), pathfinder }));

            fsm.AddState<MiningState>((int)Enums.States.Mining,
               () => (new object[4] { targetMine, inventory, OnMine, deltaTime }),
               () => (new object[2] { MinerConstants.miningTime, MinerConstants.inventoryCapacity }));

            fsm.AddState<ReturningToHome>((int)Enums.States.ReturningToHome,
               () => (new object[4] { OnSetPosition, position, speed, deltaTime }),
               () => (new object[4] { onGetTile.Invoke(targetMine.Tile.x, targetMine.Tile.y), onGetTile.Invoke((int)home.Tile.x, (int)home.Tile.y), pathfinder, OnLeaveMineralsInHome }));

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

        public void SetMine(Mine targetMine)
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
