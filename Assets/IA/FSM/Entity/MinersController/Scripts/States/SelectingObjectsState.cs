using System;
using System.Collections.Generic;

using UnityEngine;

using IA.FSM.Entity.MinersController.Enums;

namespace IA.FSM.Entity.MinersController.States
{
    public class SelectingObjectsState : State
    {
        public override List<Action> GetBehaviours(params object[] parameters)
        {
            LayerMask layerMask = (LayerMask)parameters[0];
            List<MinerController.Miner> selectedMiners = parameters[1] as List<MinerController.Miner>;
            List<MineController.Mine> selectedMines = parameters[2] as List<MineController.Mine>;
            string minerTag = (string)parameters[3];
            string mineTag = (string)parameters[4];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Start Arrangement:\n" + "Selected Miners: " + selectedMiners.Count + " - Selected Mines: " + selectedMines.Count);
                    Transition((int)Flags.OnStartArragement);
                }
                else 
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        GameObject go = hit.collider.gameObject;
                        //SelectableObject selectableObject = go.GetComponent<SelectableObject>();

                        if (layerMask == (layerMask | (1 << go.layer)))
                        {
                            if (CheckObjectSelected(go, minerTag, selectedMiners) ||
                                CheckObjectSelected(go, mineTag, selectedMines))
                            {
                                //selectableObject.Select();
                            }
                        }
                    }
                }

                bool CheckObjectSelected<T>(GameObject go, string tag, List<T> selectedObjs)
                {
                    if (go.CompareTag(tag))
                    {
                        T obj = go.GetComponent<T>();

                        if (!selectedObjs.Contains(obj))
                        {
                            selectedObjs.Add(obj);
                            return true;
                        }
                    }

                    return false;
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
            List<MinerController.Miner> selectedMiners = parameters[0] as List<MinerController.Miner>;
            List<MineController.Mine> selectedMines = parameters[1] as List<MineController.Mine>;

            List<Action> exitBehaviours = new List<Action>();

            exitBehaviours.Add(() =>
            {
                //for (int i = 0; i < selectedMiners.Count; i++)
                //{
                //    selectedMiners[i].Deselect();
                //}
                //
                //for (int i = 0; i < selectedMines.Count; i++)
                //{
                //    selectedMines[i].Deselect();
                //}
            });

            return exitBehaviours;
        }
    }
}
