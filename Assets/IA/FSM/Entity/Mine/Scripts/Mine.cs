using UnityEngine;

using IA.Common.Entity.SelectableObject;

using TMPro;

namespace IA.FSM.Entity.Mine
{
    public class Mine : SelectableObject
    {
        [Header("Mine")]
        [SerializeField] private TextMeshProUGUI txtMineralsAmount = null;
        
        private int minerals = 0;
        
        public int Minerals { get => minerals; }
        public Vector3 Position { get; private set; }
        public Vector2Int Tile { get; private set; }

        private void Start()
        {            
            minerals = Random.Range(10, 20);
        }

        public void Init(Vector3 position)
        {
            transform.position = position;
            Position = position;
            Tile = new Vector2Int((int)position.x, (int)position.z);
        }

        public void Extract()
        {
            minerals--;
        }

        private void Update()
        {
            txtMineralsAmount.text = minerals.ToString();
        }
    }
}
