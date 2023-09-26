using System;

using UnityEngine;

using IA.FSM.Common.Enums;
using IA.FSM.Common.States;
using IA.FSM.Entity.MineController;

using IA.Game.Entity.UrbanCenterController;

using IA.Pathfinding;

using IA.Voronoid.Generator;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Common.Entity.PathfinderEntityController
{
    public abstract class PathfinderBehaviour
    {
        #region PRIVATE_FIELDS
        private Grid grid = null;
        private VoronoidGenerator voronoidGenerator = null;
        private int[,] weights = null;
        
        private Vector2 position = Vector3.zero;
        private bool outOfMines = false;
        #endregion

        #region PROTECTED_FIELDS
        protected Mine targetMine = null;

        protected FSM fsm;

        protected int inventory = 0;
        protected float deltaTime = 0;
        #endregion

        #region PROPERTIES
        public int Inventory { get => inventory; }
        public Vector3 Position { get => position; }
        public Mine TargetMine { get => targetMine;  }
        #endregion

        #region PUBLIC_METHODS
        public virtual void Init(Pathfinder pathfinder, VoronoidGenerator voronoidGenerator, UrbanCenter urbanCenter, Grid grid,
            Func<Vector2, Mine> onGetMineOnPos, int[,] weights, params object[] parameters)
        {
            this.voronoidGenerator = voronoidGenerator;
            this.grid = grid;
            this.weights = weights;

            position = grid.GetTile(urbanCenter.Tile.x, urbanCenter.Tile.y).pos;

            fsm = new FSM(GetAmountOfStates(), GetAmountOfFlags());

            fsm.SetRelation((int)CommonStates.SearchingMine, (int)CommonFlags.OnSetMine, (int)CommonStates.GoingToMine);
            fsm.SetRelation((int)CommonStates.SearchingMine, (int)CommonFlags.OnNoMinesFound, (int)CommonStates.ReturningToHome);

            fsm.SetRelation((int)CommonStates.ReturningToHome, (int)CommonFlags.OnReachHome, (int)CommonStates.SearchingMine);
            
            Action<Mine> onSetTargetMine = SetMine;
            Action<Vector2> OnSetPosition = SetPosition;
            Func<bool> onUpdateMap = UpdateMap;
            Action onReachHome = OnReachHome;

            fsm.AddState<SearchingCloserMineState>((int)CommonStates.SearchingMine,
               () => (new object[5] { position, voronoidGenerator, onGetMineOnPos, onSetTargetMine, onUpdateMap }));

            fsm.AddState<GoingToMineState>((int)CommonStates.GoingToMine,
               () => (new object[4] { OnSetPosition, position, GetSpeed(), deltaTime }),
               () => (new object[3] { grid.GetCloserTileToPosition(position), grid.GetTile(targetMine.Tile.x, targetMine.Tile.y), pathfinder }));

            fsm.AddState<ReturningToHome>((int)CommonStates.ReturningToHome,
               () => (new object[5] { OnSetPosition, position, GetSpeed(), deltaTime, outOfMines }),
               () => (new object[4] { grid.GetCloserTileToPosition(position), grid.GetTile(urbanCenter.Tile.x, urbanCenter.Tile.y), pathfinder, onReachHome }));

            fsm.AddState<IdleState>((int)CommonStates.Idle, () => (new object[1] { 0 }));

            fsm.SetCurrentStateForced((int)CommonStates.SearchingMine);
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
        }
        #endregion

        #region PROTECTED_METHODS
        protected void SetPosition(Vector2 pos)
        {
            position = pos;
        }

        protected bool UpdateMap()
        {
            Vector2[] minersPositions = GetPositionsOfInterest();

            if (minersPositions.Length == 0)
            {
                outOfMines = true;
                return false;
            }

            voronoidGenerator.Configure(minersPositions, new Vector2(grid.RealWidth, grid.RealHeight), weights);
            return true;
        }
        #endregion

        #region ABSTRACT_METHODS
        protected abstract int GetAmountOfStates();
        protected abstract int GetAmountOfFlags();
        protected abstract float GetSpeed();
        protected abstract void OnReachHome();
        protected abstract Vector2[] GetPositionsOfInterest();
        #endregion
    }
}