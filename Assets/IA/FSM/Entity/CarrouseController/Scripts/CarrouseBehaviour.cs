using System.Collections.Generic;
using System;

using UnityEngine;

using IA.FSM.Common.Enums;
using IA.FSM.Common.Entity.PathfinderEntityController;
using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController;
using IA.FSM.Entity.CarrouseController.Enums;
using IA.FSM.Entity.CarrouseController.Constants;
using IA.FSM.Entity.CarrouseController.States;

using IA.Game.Entity.UrbanCenterController;

using IA.Pathfinding;

using IA.Voronoid.Generator;

using Grid = IA.Pathfinding.Grid;
using IA.FSM.Entity.MinerController.Constants;

namespace IA.FSM.Entity.CarrouseController
{
    public class CarrouseBehaviour : PathfinderBehaviour
    {
        #region ACTIONS
        private Func<List<Miner>> onGetAllMinersMining = null;
        #endregion

        #region PUBLIC_METHODS
        public override void Init(Pathfinder pathfinder, VoronoidGenerator voronoidGenerator, UrbanCenter urbanCenter, Grid grid,
            Func<Vector2, Mine> onGetMineOnPos, int[,] weights, params object[] parameters)
        {
            base.Init(pathfinder, voronoidGenerator, urbanCenter, grid, onGetMineOnPos, weights, parameters);

            onGetAllMinersMining = (Func<List<Miner>>)parameters[0];

            fsm.SetRelation((int)CommonStates.GoingToMine, (int)CommonFlags.OnReachMine, (int)Enums.States.GivingFood);
                                 
            fsm.SetRelation((int)Enums.States.GivingFood, (int)Flags.OnFindNextMine, (int)CommonStates.SearchingMine);
            fsm.SetRelation((int)Enums.States.GivingFood, (int)Flags.OnEmptyInventory, (int)CommonStates.ReturningToHome);

            fsm.SetRelation((int)CommonStates.ReturningToHome, (int)CommonFlags.OnFinishJob, (int)CommonStates.SearchingMine);

            Func<(int, int)> onGiveFood = GiveFood;
                      
            fsm.AddState<GivingFoodState>((int)Enums.States.GivingFood, 
                () => (new object[2] { GetMinerOnMine(), onGiveFood }));
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
            return CarrouseConstants.moveSpeed;
        }

        protected override void OnReachHome()
        {
            inventory = 10;
        }

        protected override Vector2[] GetPositionsOfInterest()
        {
            List<Miner> minersLeft = onGetAllMinersMining.Invoke();
            List<Vector2> minersPositions = new List<Vector2>();

            for (int i = 0; i < minersLeft.Count; i++)
            {
                if (minersLeft[i].MinerBehaviour.FoodsLeft < MinerConstants.foodCapacity)
                {
                    minersPositions.Add(minersLeft[i].MinerBehaviour.Position);
                }
            }

            return minersPositions.ToArray();
        }
        #endregion

        #region PRIVATE_METHODS
        private Miner GetMinerOnMine()
        {
            List<Miner> miners = onGetAllMinersMining.Invoke();

            for (int i = 0; i < miners.Count; i++)
            {
                if (miners[i].MinerBehaviour.TargetMine == targetMine && miners[i].MinerBehaviour.FoodsLeft < MinerConstants.foodCapacity)
                {
                    return miners[i];
                }
            }

            return null;
        }

        private (int, int) GiveFood()
        {
            int foodToGive = CarrouseConstants.foodPack;
            if (inventory < CarrouseConstants.foodPack)
            {
                foodToGive = inventory;
            }

            inventory -= foodToGive;

            return (inventory, foodToGive);
        }
        #endregion
    }
}
