using System;
using System.Collections.Generic;

using UnityEngine;

using IA.Game.Entity.UrbanCenterController;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController.Constants;
using IA.FSM.Entity.MinerController.Enums;
using IA.FSM.Entity.MinerController.States;

using IA.Pathfinding;

using IA.Voronoid.Generator;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Entity.MinerController
{
    public class MinerBehaviour
    {
        #region PRIVATE_FIELDS
        private Mine targetMine = null;
        private UrbanCenter urbanCenter = null;

        private Vector2 position = Vector3.zero;
        private int inventory = 0;
        private int foodsLeft = 22;

        private FSM fsm;
        private Pathfinder pathfinder = null;
        private VoronoidGenerator voronoidGenerator = null;
        private Grid grid = null;

        private bool outOfMines = false;

        private int[,] weights = null;

        private float deltaTime = 0;
        #endregion

        #region ACTIONS
        private Func<Mine[]> onGetAllMinesLeft = null;
        #endregion

        #region PROPERTIES
        public int Inventory { get => inventory; }
        public Vector3 Position { get => position; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Pathfinder pathfinder, VoronoidGenerator voronoidGenerator, UrbanCenter urbanCenter, Grid grid,
            Action onLeaveMineralsInHome, Func<Vector2, Mine> onGetMineOnPos, Func<Mine[]> onGetAllMinesLeft, int[,] weights)
        {
            this.pathfinder = pathfinder;
            this.voronoidGenerator = voronoidGenerator;
            this.urbanCenter = urbanCenter;
            this.onGetAllMinesLeft = onGetAllMinesLeft;
            this.grid = grid;

            this.weights = weights;

            position = grid.GetTile(urbanCenter.Tile.x, urbanCenter.Tile.y).pos;

            fsm = new FSM(Enum.GetValues(typeof(Enums.States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)Enums.States.SearchingCloserMine, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);
            fsm.SetRelation((int)Enums.States.SearchingCloserMine, (int)Flags.OnNoMinesFound, (int)Enums.States.ReturningToHome);

            fsm.SetRelation((int)Enums.States.GoingToMine, (int)Flags.OnReachMine, (int)Enums.States.Mining);

            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnEmptyMine, (int)Enums.States.SearchingCloserMine);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnHungry, (int)Enums.States.WaitingForFood);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnFullInventory, (int)Enums.States.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);

            fsm.SetRelation((int)Enums.States.WaitingForFood, (int)Flags.OnReceivedFood, (int)Enums.States.Mining);

            fsm.SetRelation((int)Enums.States.ReturningToHome, (int)Flags.OnReachHome, (int)Enums.States.SearchingCloserMine);
            fsm.SetRelation((int)Enums.States.ReturningToHome, (int)Flags.OnFinishJob, (int)Enums.States.Idle);

            Action<Mine> onSetTargetMine = SetMine;
            Action OnMine = Mine;
            Action<Vector2> OnSetPosition = SetPosition;
            Action OnLeaveMineralsInHome = () =>
            {
                onLeaveMineralsInHome.Invoke();
                inventory = 0;
            };
            Func<bool> onUpdateMap = UpdateMap;

            fsm.AddState<SearchingCloserMineState>((int)Enums.States.SearchingCloserMine,
                () => (new object[5] { position, voronoidGenerator, onGetMineOnPos, onSetTargetMine, onUpdateMap }));

            fsm.AddState<GoingToMineState>((int)Enums.States.GoingToMine,
               () => (new object[4] { OnSetPosition, position, MinerConstants.moveSpeed, deltaTime }),
               () => (new object[3] { grid.GetCloserTileToPosition(position), grid.GetTile(targetMine.Tile.x, targetMine.Tile.y), pathfinder }));

            fsm.AddState<MiningState>((int)Enums.States.Mining,
               () => (new object[5] { targetMine, inventory, OnMine, deltaTime, foodsLeft }),
               () => (new object[2] { MinerConstants.miningTime, MinerConstants.inventoryCapacity }));

            fsm.AddState<HungryState>((int)Enums.States.WaitingForFood,
               () => (new object[1] { foodsLeft }));

            fsm.AddState<ReturningToHome>((int)Enums.States.ReturningToHome,
               () => (new object[5] { OnSetPosition, position, MinerConstants.moveSpeed, deltaTime, outOfMines }),
               () => (new object[4] { grid.GetTile(targetMine.Tile.x, targetMine.Tile.y), grid.GetTile(urbanCenter.Tile.x, urbanCenter.Tile.y), pathfinder, OnLeaveMineralsInHome }));

            fsm.AddState<IdleState>((int)Enums.States.Idle, () => (new object[1] { 0 }));

            fsm.SetCurrentStateForced((int)Enums.States.SearchingCloserMine);
        }

        public void UpdateFsm()
        {
            fsm.Update();
        }

        public void SetDeltaTime(float deltaTime)
        {
            this.deltaTime = deltaTime;
        }

        public void ReceiveFood(int amountOfFood)
        {
            foodsLeft += amountOfFood;
        }

        public void SetMine(Mine targetMine)
        {
            this.targetMine = targetMine;
        }
        #endregion

        #region PRIVATE_METHODS
        private bool UpdateMap()
        {
            Vector2[] minesPositions = GetMinesPositions();

            if (minesPositions.Length == 0)
            {
                outOfMines = true;
                return false;
            }

            voronoidGenerator.Configure(minesPositions, new Vector2(grid.RealWidth, grid.RealHeight), weights);
            return true;
        }

        private Vector2[] GetMinesPositions()
        {
            Mine[] minesLeft = onGetAllMinesLeft.Invoke();
            List<Vector2> minesPositions = new List<Vector2>();

            for (int i = 0; i < minesLeft.Length; i++)
            {
                minesPositions.Add(minesLeft[i].Position);
            }

            return minesPositions.ToArray();
        }

        private void SetPosition(Vector2 pos)
        {
            position = pos;
        }

        private void Mine()
        {
            inventory++;
            foodsLeft--;
        }
        #endregion
    }
}
