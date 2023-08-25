using UnityEngine;

using TMPro;

namespace IA.Background.Entity.Home
{
    public class Home : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtMineralsAmount = null;

        private int minerals = 0;

        public Vector3 Position { get; private set; }

        private void Start()
        {
            Position = transform.position;
        }

        public void PlaceMinerals(int minerals)
        {
            this.minerals += minerals;
        }

        private void Update()
        {
            txtMineralsAmount.text = this.minerals.ToString();            
        }
    }
}
