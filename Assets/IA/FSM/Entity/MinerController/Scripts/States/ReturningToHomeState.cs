using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinerController.Enums;
using IA.Pathfinding;

namespace IA.FSM.Entity.MinerController.States
{
    public class ReturningToHome : State
    {
        private List<Vector2> path = null;
        private int indexOfMovement = 0;
        Action onReachHome = null;

        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Action<Vector2> onSetPosition = parameters[0] as Action<Vector2>;
            Vector2 position = (Vector2)parameters[1];
            float speed = (float)parameters[2];
            float deltaTime = (float)parameters[3];
            bool outOfMines = (bool)parameters[4];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                Vector2 targetPos = path[indexOfMovement];
                Vector2 newPos = position + ((targetPos - position).normalized * speed * deltaTime);
                onSetPosition.Invoke(newPos);

                if (Vector3.Distance(newPos, targetPos) < 0.01f)
                {
                    indexOfMovement++;

                    if (indexOfMovement == path.Count)
                    {
                        onReachHome.Invoke();

                        if (outOfMines)
                        {
                            Transition((int)Flags.OnFinishJob);
                        }
                        else
                        {
                            Transition((int)Flags.OnReachHome);
                        }
                    }
                }
            });
            behaviours.Add(() => Debug.Log("RETURNING TO HOME"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
            Tile startTile = parameters[0] as Tile;
            Tile targetTile = parameters[1] as Tile;
            Pathfinder pathfinder = parameters[2] as Pathfinder;
            onReachHome = parameters[3] as Action;

            path = pathfinder.FindPath(startTile, targetTile);
            indexOfMovement = 0;

            List<Action> enterBehaviours = new List<Action>();
            return enterBehaviours;
        }

        public override List<Action> GetOnExitBehaviours(params object[] parameters)
        {
            List<Action> exitBehaviours = new List<Action>();

            return exitBehaviours;
        }
    }
}
