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

        private void Start()
        {
            minerals = Random.Range(10, 20);
            txtMineralsAmount.text = minerals.ToString();
        }

        public void Extract()
        {
            minerals--;
            txtMineralsAmount.text = minerals.ToString();
        }
    }
}
