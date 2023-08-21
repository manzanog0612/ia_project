using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinersController.Enums;

namespace IA.FSM.Entity.MinersController.States
{
    public class IdleState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            LayerMask layerMask = (LayerMask)parameters[0];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (layerMask == (layerMask | (1 << hit.collider.gameObject.layer)))
                        {
                            Transition((int)Flags.OnSelectObject);
                        }
                    }
                }
            });

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
