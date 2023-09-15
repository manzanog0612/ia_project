using System.Collections.Generic;

using UnityEngine;

using IA.Voronoid.Entity;

namespace IA.Voronoid.Generator
{
    public class VoronoidGenerator : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private Transform[] positions = null;
        [SerializeField] private Vector2Int gridSize = default;
        [SerializeField] private bool onPoligon = false;//
        [SerializeField] private Transform checker;//
        [SerializeField] private int sectorToCheck = 0;//
        #endregion

        #region PRIVATE_FIELDS
        private List<Limit> limits = null;
        private List<Sector> sectors = new List<Sector>();
        private bool startCheck = false;//
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            InitLimits();

            sectors.Clear();

            for (int i = 0; i < positions.Length; i++)
            {
                sectors.Add(new Sector(positions[i].position));
                sectors[i].AddSegmentLimits(limits);
            }

            for (int i = 0; i < sectors.Count; i++)
            {
                for (int j = 0; j < sectors.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    sectors[i].AddSegment(sectors[i].Position, sectors[j].Position);
                }
            }

            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].SetIntersections(gridSize);
            }

            startCheck = true;
        }

        private void Update()
        {
            if (!startCheck)
            {
                return;
            }

            onPoligon = sectors[sectorToCheck].CheckPointInSector(checker.position);
        }

        private void OnDrawGizmos()
        {
            Draw();
        }
        #endregion

        #region PRIVATE_METHODS
        private void InitLimits()
        {
            limits = new List<Limit>();

            limits.Add(new Limit(Vector2.zero, DIRECTION.LEFT));
            limits.Add(new Limit(new Vector2(0f, gridSize.y - 1), DIRECTION.UP));
            limits.Add(new Limit(new Vector2(gridSize.x - 1, gridSize.y - 1), DIRECTION.RIGHT));
            limits.Add(new Limit(new Vector2(gridSize.x - 1, 0f), DIRECTION.DOWN));
        }

        private void Draw()
        {
            if (sectors == null) return;

            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].DrawSector();
                sectors[i].DrawSegments();
            }
        }
        #endregion
    }
}