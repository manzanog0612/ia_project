using UnityEngine;

using TMPro;

namespace IA.Game.Entity.UrbanCenterController
{
    public class UrbanCenter : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TextMeshProUGUI txtMineralsAmount = null;
        #endregion

        #region PRIVATE_FIELDS
        private int minerals = 0;
        #endregion

        #region PROPERTIES
        public Vector2Int Tile { get; private set; }
        public Vector2 Position { get; private set; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Vector2Int tile, Vector2 position)
        {
            Tile = tile;
            Position = position;
            transform.position = position;  
        }

        public void UpdateText()
        {
            txtMineralsAmount.text = minerals.ToString();
        }

        public void PlaceMinerals(int minerals)
        {
            this.minerals += minerals;
        }
        #endregion
    }
}
