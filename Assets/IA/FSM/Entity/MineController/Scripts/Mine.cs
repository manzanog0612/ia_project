using UnityEngine;

using TMPro;
using UnityEngine.UIElements;

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
        public Vector2 Position { get; private set; }
        #endregion

        #region PUBLIC_METHODS
        public void Init(Vector2Int tile, Vector2 position)
        {
            minerals = Random.Range(10, 20);

            Position = position;
            transform.position = position;
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
