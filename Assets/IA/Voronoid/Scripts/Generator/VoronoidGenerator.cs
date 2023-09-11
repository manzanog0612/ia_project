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
        #endregion

        #region PRIVATE_FIELDS
        private List<Vector3> gridLimits = new List<Vector3>();
        private List<Vector2> voronoiLines = new List<Vector2>();

        private List<Limit> limits = null;
        private List<Sector> sectors = new List<Sector>();
        #endregion

        private void Start()
        {
            SetLimits(gridSize);
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
        }

        private void OnDrawGizmos()
        {
            Draw();
        }

        private void SetLimits(Vector2Int gridSize)
        {
            gridLimits.Add(new Vector2(0, 0)); //left down
            gridLimits.Add(new Vector2(0, gridSize.y)); //left up
            gridLimits.Add(new Vector2(gridSize.x, gridSize.y)); //right up
            gridLimits.Add(new Vector2(gridSize.x, 0)); //right down
        }

        private void InitLimits()
        {
            limits = new List<Limit>();

            //Vector2 offset = new Vector2(NodeUtils.offset.x, NodeUtils.offset.y) / 2f;

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
    }
}