using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Common.Entity.PathfinderEntityController;
using IA.FSM.Common.Enums;
using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController.Constants;
using IA.FSM.Entity.MinerController.Enums;
using IA.FSM.Entity.MinerController.States;

using IA.Game.Entity.UrbanCenterController;

using IA.Pathfinding;

using IA.Voronoid.Generator;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Entity.MinerController
{
    public class MinerBehaviour : PathfinderBehaviour
    {
        #region PRIVATE_FIELDS
        private int foodsLeft = 3;

        private float speed = 0;
        #endregion

        #region ACTIONS
        private Func<Mine[]> onGetAllMinesLeft = null;
        private Action onLeaveMineralsInHome = null;
        #endregion

        #region PROPERTIES
        public Enums.States ActualState => (Enums.States)fsm.currentStateIndex;
        public int FoodsLeft => foodsLeft;
        #endregion

        #region PUBLIC_METHODS
        public override void Init(Pathfinder pathfinder, VoronoidGenerator voronoidGenerator, UrbanCenter urbanCenter, Grid grid,
             Func<Vector2, Mine> onGetMineOnPos, int[,] weights, params object[] parameters)
        {
            base.Init(pathfinder, voronoidGenerator, urbanCenter, grid, onGetMineOnPos, weights, parameters);

            onLeaveMineralsInHome = (Action)parameters[0];
            onGetAllMinesLeft = (Func<Mine[]>)parameters[01];

            fsm.SetRelation((int)CommonStates.GoingToMine, (int)CommonFlags.OnReachMine, (int)Enums.States.Mining);

            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnEmptyMine, (int)CommonStates.SearchingMine);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnHungry, (int)Enums.States.WaitingForFood);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnFullInventory, (int)CommonStates.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)CommonFlags.OnSetMine, (int)CommonStates.GoingToMine);

            fsm.SetRelation((int)Enums.States.WaitingForFood, (int)Flags.OnReceivedFood, (int)Enums.States.Mining);
            fsm.SetRelation((int)Enums.States.WaitingForFood, (int)Flags.OnEmptyMine, (int)CommonStates.SearchingMine);

            fsm.SetRelation((int)CommonStates.ReturningToHome, (int)CommonFlags.OnFinishJob, (int)CommonStates.Idle);

            Action OnMine = Mine;
            Action OnLeaveMineralsInHome = () =>
            {
                onLeaveMineralsInHome.Invoke();
                inventory = 0;
            };

            fsm.AddState<MiningState>((int)Enums.States.Mining,
               () => (new object[5] { targetMine, inventory, OnMine, deltaTime, foodsLeft }),
               () => (new object[2] { MinerConstants.miningTime, MinerConstants.inventoryCapacity }));

            fsm.AddState<HungryState>((int)Enums.States.WaitingForFood,
               () => (new object[1] { foodsLeft }));

            speed = MinerConstants.GetMovementSpeed();
        }

        public void ReceiveFood(int amountOfFood)
        {
            foodsLeft += amountOfFood;
        }
        #endregion

        #region OVERRIDE
        protected override int GetAmountOfStates()
        {
            return Enum.GetValues(typeof(Enums.States)).Length + Enum.GetValues(typeof(CommonStates)).Length;
        }

        protected override int GetAmountOfFlags()
        {
            return Enum.GetValues(typeof(Flags)).Length + Enum.GetValues(typeof(CommonFlags)).Length;
        }

        protected override float GetSpeed()
        {
            return speed;
        }

        protected override void OnReachHome()
        {
            onLeaveMineralsInHome.Invoke();
            inventory = 0;
        }

        protected override Vector2[] GetPositionsOfInterest()
        {
            Mine[] minesLeft = onGetAllMinesLeft.Invoke();
            List<Vector2> minesPositions = new List<Vector2>();

            for (int i = 0; i < minesLeft.Length; i++)
            {
                minesPositions.Add(minesLeft[i].Position);
            }

            return minesPositions.ToArray();
        }
        #endregion

        #region PRIVATE_METHODS
        private void Mine()
        {
            inventory++;
            foodsLeft--;
        }
        #endregion
    }
}
