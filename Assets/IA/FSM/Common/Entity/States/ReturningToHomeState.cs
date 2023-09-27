using System;
using System.Collections.Generic;

using UnityEngine;

using IA.Pathfinding;
using IA.FSM.Common.Enums;

namespace IA.FSM.Common.States
{
    public class ReturningToHome : State
    {
        private List<Vector2> path = null;
        private int indexOfMovement = 0;
        Action onReachHome = null;
        bool panicBefore = false;

        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Action<Vector2> onSetPosition = parameters[0] as Action<Vector2>;
            Vector2 position = (Vector2)parameters[1];
            float speed = (float)parameters[2];
            float deltaTime = (float)parameters[3];
            bool outOfMines = (bool)parameters[4];
            Func<bool> onInterruptToGoToMineCheck = (Func<bool>)parameters[5];
            bool panic = (bool)parameters[6];

            List <Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (panicBefore && !panic)
                {
                    Transition((int)CommonFlags.OnResumeAfterPanic);
                }
                else
                if (onInterruptToGoToMineCheck.Invoke())
                {
                    Transition((int)CommonFlags.OnInterruptToGoToMine);
                }
                else
                if (path.Count == 0 || Vector3.Distance(path[path.Count - 1], position) < 0.01f)
                {
                    OnReachHome();
                    Debug.Log("AT HOME");
                }
                else
                {
                    Vector2 targetPos = path[indexOfMovement];
                    Vector2 newPos = position + ((targetPos - position).normalized * speed * deltaTime);
                    onSetPosition.Invoke(newPos);

                    if (Vector3.Distance(newPos, targetPos) < 0.01f)
                    {
                        onSetPosition.Invoke(targetPos);
                        indexOfMovement++;

                        if (indexOfMovement == path.Count)
                        {
                            OnReachHome();
                        }
                    }

                    Debug.Log("RETURNING TO HOME");
                }

                panicBefore = panic;
            });

            void OnReachHome()
            {
                onReachHome.Invoke();

                if (outOfMines)
                {
                    Transition((int)CommonFlags.OnFinishJob);
                }
                else
                {
                    Transition((int)CommonFlags.OnReachHome);
                }
            }

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
