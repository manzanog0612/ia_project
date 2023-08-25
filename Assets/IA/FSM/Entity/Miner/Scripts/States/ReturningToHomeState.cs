using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.Miner.Enums;

namespace IA.FSM.Entity.Miner.States
{
    public class ReturningToHome : State
    {
        Action onReachHome = null;

        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Action<Vector3> onSetPosition = parameters[0] as Action<Vector3>;
            Vector3 position = (Vector3)parameters[1];
            Vector3 homePosition = (Vector3)parameters[2];
            float speed = (float)parameters[3];
            float deltaTime = (float)parameters[4];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                Vector3 newPos = position + ((homePosition - position).normalized * speed * deltaTime);
                onSetPosition.Invoke(newPos);

                if (Vector3.Distance(newPos, homePosition) < 0.1f)
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
