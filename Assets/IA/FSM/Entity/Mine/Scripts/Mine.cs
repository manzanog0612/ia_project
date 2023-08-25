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

        private void Start()
        {
            Position = transform.position;
            minerals = Random.Range(10, 20);
            //txtMineralsAmount.text = minerals.ToString();
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
