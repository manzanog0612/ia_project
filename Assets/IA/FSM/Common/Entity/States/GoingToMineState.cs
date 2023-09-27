using System;
using System.Collections.Generic;

using UnityEngine;

using IA.Pathfinding;
using IA.FSM.Common.Enums;

namespace IA.FSM.Common.States
{
    public class GoingToMineState : State
    {
        private List<Vector2> path = null;
        private int indexOfMovement = 0;
        Vector2 targetPos = Vector2.zero;

        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Action<Vector2> onSetPosition = parameters[0] as Action<Vector2>;
            Vector2 position = (Vector2)parameters[1];
            float speed = (float)parameters[2];
            float deltaTime = (float)parameters[3];
            Func<bool> onInterruptToGoToOtherMineCheck = (Func<bool>)parameters[4];
            Func<bool> onInterruptToGoToHomeCheck = (Func<bool>)parameters[5];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (onInterruptToGoToHomeCheck.Invoke())
                {
                    Transition((int)CommonFlags.OnInterruptToGoToHome);
                }
                else if (onInterruptToGoToOtherMineCheck.Invoke())
                {
                    Transition((int)CommonFlags.OnInterruptToGoToMine);
                }
                else if (path.Count == 0)
                {
                    onSetPosition.Invoke(targetPos);
                    Transition((int)CommonFlags.OnReachMine);
                }
                else
                {
                    Vector2 targetPos = path[indexOfMovement];
                    Vector2 newPos = position + ((targetPos - position).normalized * speed * deltaTime);
                    onSetPosition.Invoke(newPos);

                    if (Vector2.Distance(newPos, targetPos) < 0.01f)
                    {
                        onSetPosition.Invoke(targetPos);

                        indexOfMovement++;

                        if (indexOfMovement == path.Count)
                        {
                            Transition((int)CommonFlags.OnReachMine);
                        }
                    }
                }                
            });
            behaviours.Add(() => Debug.Log("GOING TO MINE"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
            Tile startTile = parameters[0] as Tile;
            Tile targetTile = parameters[1] as Tile;
            Pathfinder pathfinder = parameters[2] as Pathfinder;

            targetPos = targetTile.pos;
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
