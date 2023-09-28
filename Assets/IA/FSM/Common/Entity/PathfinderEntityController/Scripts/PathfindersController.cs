using System.Collections.Concurrent;
using System.Threading.Tasks;

using UnityEngine;

using IA.Game.Entity.UrbanCenterController;

using Grid = IA.Pathfinding.Grid;

namespace IA.FSM.Common.Entity.PathfinderEntityController
{
    public class PathfindersController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private GameObject pathfinderPrefab = null;
        [SerializeField] private Transform pathfindersHolder = null;
        #endregion

        #region PRIVATE_FIELDS
        int entitiesAmount = 0;
        protected ConcurrentBag<PathfinderEntity> pathfindersFsm = new ConcurrentBag<PathfinderEntity>();
        protected PathfinderEntity[] pathfinders = null;
        #endregion

        #region PUBLIC_METHODS
        public virtual void Init(int entitiesAmount, Grid grid, UrbanCenter urbanCenter, params object[] paramenters)
        {
            this.entitiesAmount = entitiesAmount;
            pathfinders = new PathfinderEntity[entitiesAmount];

            for (int i = 0; i < entitiesAmount; i++)
            {
                pathfinders[i] = Instantiate(pathfinderPrefab, pathfindersHolder).GetComponent<PathfinderEntity>();
                pathfinders[i].Init(urbanCenter, grid);
                pathfindersFsm.Add(pathfinders[i]);
            }

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = entitiesAmount };
        }

        public void SetPanic(bool status)
        {
            for (int i = 0; i < pathfinders.Length; i++)
            {
                pathfinders[i].SetPanic(status);
            }
        }

        public void UpdateBehaviours()
        {
            Parallel.ForEach(pathfindersFsm,
                pathfinder =>
                {
                    pathfinder.PathfinderBehaviour.UpdateFsm();
                });

            for (int i = 0; i < pathfindersFsm.Count; i++)
            {
                pathfinders[i].UpdateBehaviour();
            }
        }
        #endregion
    }
}

