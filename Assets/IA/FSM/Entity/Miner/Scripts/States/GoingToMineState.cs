using System;
using System.Collections.Generic;

using UnityEngine;

namespace IA.FSM.Entity.Miner.Enum
{
    public class GoingToMineState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Transform transform = parameters[0] as Transform;
            Transform mine = parameters[1] as Transform;
            float speed = (float)parameters[2];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                transform.position += (mine.position - transform.position).normalized * speed * Time.deltaTime;

                if (Vector3.Distance(transform.position, mine.position) < 0.1f)
                {
                    Transition((int)Flags.OnReachMine);
                }
            });
            behaviours.Add(() => Debug.Log("GOING TO MINE"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
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
