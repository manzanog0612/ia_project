using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MineController;
using IA.FSM.Entity.MinerController.Enums;
using IA.Voronoid.Generator;

namespace IA.FSM.Entity.MinerController.States
{
    public class SearchingCloserMineState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            Vector2 actualPos = (Vector2)parameters[0];
            VoronoidGenerator voronoidGenerator = (VoronoidGenerator)parameters[1];
            Func<Vector2, Mine> onGetMineOnPos = (Func<Vector2, Mine>)parameters[2];
            Action<Mine> onSetTargetMine = (Action<Mine>)parameters[3];

            List <Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                Vector2 minePos = voronoidGenerator.GetSectorCloserToPosition(actualPos);
                Mine closerMine = onGetMineOnPos.Invoke(minePos);
                onSetTargetMine.Invoke(closerMine);
                Transition((int)Flags.OnSetMine);
            });
            behaviours.Add(() => Debug.Log("SEARCHING CLOSER MINE"));

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
