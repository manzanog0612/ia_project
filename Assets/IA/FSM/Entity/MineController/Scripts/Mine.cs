using UnityEngine;

using TMPro;

namespace IA.FSM.Entity.MineController
{
    public class Mine : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private TextMeshProUGUI txtMineralsAmount = null;
        #endregion

        #region PRIVATE_FIELDS
        private int minerals = 0;
        #endregion

        #region PROPERTIES
        public int Minerals { get => minerals; }
        public Vector2Int Tile { get; private set; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Vector2Int tile)
        {
            minerals = Random.Range(10, 20);

            transform.position = new Vector2(tile.x, tile.y);
            Tile = tile;
        }

        public void UpdateText()
        {
            txtMineralsAmount.text = minerals.ToString();
        }

        public void Extract()
        {
            minerals--;
        }
        #endregion        
    }
}
