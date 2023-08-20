using System;
using System.Collections.Generic;

using UnityEngine;

namespace IA.FSM.Entity.Miner.Enum
{
    public class ReturningToHome : State
    {
        Action onReachHome = null;

        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Transform transform = parameters[0] as Transform;
            Transform home = parameters[1] as Transform;
            float speed = (float)parameters[2];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                transform.position += (home.position - transform.position).normalized * speed * Time.deltaTime;

                if (Vector3.Distance(transform.position, home.position) < 0.1f)
                {
                    onReachHome.Invoke();
                    Transition((int)Flags.OnReachHome);
                }
            });
            behaviours.Add(() => Debug.Log("RETURNING TO HOME"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(params object[] parameters)
        {
            onReachHome = parameters[0] as Action;

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
