using System;

using UnityEngine;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController.Constants;
using IA.FSM.Entity.MinerController.Enums;
using IA.FSM.Entity.MinerController.States;
using IA.Game.Entity.UrbanCenterController;
using IA.Pathfinding;
using IA.Voronoid.Generator;

namespace IA.FSM.Entity.MinerController
{
    public class MinerBehaviour
    {
        #region PRIVATE_FIELDS
        private Mine targetMine = null;
        private UrbanCenter home = null;

        private Vector2 position = Vector3.zero;
        private int inventory = 0;
        private int foodsLeft = 3;

        private FSM fsm;
        private Pathfinder pathfinder = null;
        private VoronoidGenerator voronoidGenerator = null;

        private float deltaTime = 0;
        #endregion

        #region PROPERTIES
        public int Inventory { get => inventory; }
        public Vector3 Position { get => position; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Pathfinder pathfinder, Vector2Int initialTile, VoronoidGenerator voronoidGenerator, UrbanCenter home, 
            Action onLeaveMineralsInHome, Func<int,int,Tile> onGetTile, Func<Vector2, Mine> onGetMineOnPos, 
            Func<Vector2, Tile> onGetCloserTileToPos)
        {
            this.pathfinder = pathfinder;
            this.voronoidGenerator = voronoidGenerator;
            this.home = home;

            position = onGetTile.Invoke(initialTile.x, initialTile.y).pos;

            fsm = new FSM(Enum.GetValues(typeof(Enums.States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)Enums.States.SearchingCloserMine, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);

            fsm.SetRelation((int)Enums.States.GoingToMine, (int)Flags.OnReachMine, (int)Enums.States.Mining);

            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnEmptyMine, (int)Enums.States.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnHungry, (int)Enums.States.WaitingForFood);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnFullInventory, (int)Enums.States.ReturningToHome);
            fsm.SetRelation((int)Enums.States.Mining, (int)Flags.OnSetMine, (int)Enums.States.GoingToMine);

            fsm.SetRelation((int)Enums.States.WaitingForFood, (int)Flags.OnReceivedFood, (int)Enums.States.Mining);

            fsm.SetRelation((int)Enums.States.ReturningToHome, (int)Flags.OnReachHome, (int)Enums.States.SearchingCloserMine);

            Action<Mine> onSetTargetMine = SetMine;
            Action OnMine = Mine;
            Action<Vector3> OnSetPosition = SetPosition;
            Action OnLeaveMineralsInHome = () =>
            {
                onLeaveMineralsInHome.Invoke();
                inventory = 0;
            };

            fsm.AddState<SearchingCloserMineState>((int)Enums.States.SearchingCloserMine,
                () => (new object[4] { position, voronoidGenerator, onGetMineOnPos, onSetTargetMine }));

            fsm.AddState<GoingToMineState>((int)Enums.States.GoingToMine,
               () => (new object[4] { OnSetPosition, position, MinerConstants.moveSpeed, deltaTime }),
               () => (new object[3] { onGetCloserTileToPos.Invoke(position), onGetTile.Invoke(targetMine.Tile.x, targetMine.Tile.y), pathfinder }));

            fsm.AddState<MiningState>((int)Enums.States.Mining,
               () => (new object[5] { targetMine, inventory, OnMine, deltaTime, foodsLeft }),
               () => (new object[2] { MinerConstants.miningTime, MinerConstants.inventoryCapacity }));

            fsm.AddState<HungryState>((int)Enums.States.WaitingForFood,
               () => (new object[1] { foodsLeft }));

            fsm.AddState<ReturningToHome>((int)Enums.States.ReturningToHome,
               () => (new object[4] { OnSetPosition, position, MinerConstants.moveSpeed, deltaTime }),
               () => (new object[4] { onGetTile.Invoke(targetMine.Tile.x, targetMine.Tile.y), onGetTile.Invoke(home.Tile.x, home.Tile.y), pathfinder, OnLeaveMineralsInHome }));

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
        private void SetPosition(Vector3 pos)
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
