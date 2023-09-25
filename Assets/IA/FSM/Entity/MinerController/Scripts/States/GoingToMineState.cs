using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinerController.Enums;
using IA.Pathfinding;

namespace IA.FSM.Entity.MinerController.States
{
    public class GoingToMineState : State
    {
        private List<Vector2> path = null;
        private int indexOfMovement = 0;

        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Action<Vector3> onSetPosition = parameters[0] as Action<Vector3>;
            Vector2 position = (Vector2)parameters[1];
            float speed = (float)parameters[2];
            float deltaTime = (float)parameters[3];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                Vector2 targetPos = path[indexOfMovement];
                Vector2 newPos = position + ((targetPos - position).normalized * speed * deltaTime);
                onSetPosition.Invoke(newPos);

                if (Vector2.Distance(newPos, targetPos) < 0.01f)
                {
                    indexOfMovement++;

                    if (indexOfMovement == path.Count)
                    { 
                        Transition((int)Flags.OnReachMine); 
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
